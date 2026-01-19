using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Controllers;
using Sir98Backend.Data;
using Sir98Backend.Interfaces;
using Sir98Backend.Models;
using Sir98Backend.Repository;
using Sir98Backend.Services;
using Sir98Backend.Services.Notifications;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //We  have many-to-many relationships, this prevents circular references from potentially causing issues during serialization
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; //if a value is null, do not include it in the response
    });


//repositories
builder.Services.AddScoped<ActivityRepo>();
builder.Services.AddScoped<ActivitySubscriptionRepo>();
builder.Services.AddScoped<ChangedActivityRepo>();
builder.Services.AddScoped<InstructorRepo>();

//services
builder.Services.AddScoped<ActivityOccurrenceService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddSingleton<ActivityNotificationPayloadBuilder>();
builder.Services.AddScoped<IPushSubscriptionService, PushSubscriptionService>();
builder.Services.AddScoped<IPushSender, PushSender>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<TokenService>();



builder.Services.AddCors(options => //allow all for testing
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var keyForSigning = builder.Configuration["JwtSettings:SigningKey"];

//Configure Vapid settings used for push notifications
builder.Services.Configure<VapidConfig>(
    builder.Configuration.GetSection("Vapid"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyForSigning)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddRateLimiter(options =>
{
    //structure taken from https://stackoverflow.com/questions/76309904/net-7-rate-limiting-rate-limit-by-ip

    options.AddPolicy("userLoginRegisterForgot", 
        httpContext => RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: httpContext.Request.Path.ToString() + httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new()
            {
                AutoReplenishment = true,
                TokenLimit = 3,
                TokensPerPeriod = 1,
                ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            }
        )
    );

    options.OnRejected = async (context, cancellationToken) =>
    {
        // Custom rejection handling logic
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = "300";

        await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);

        // Optional logging
        //Console.WriteLine("Rate limit exceeded for IP: {IpAddress}", context.HttpContext.Connection.RemoteIpAddress);

    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    //options.UseSqlServer(builder.Configuration.GetConnectionString("CharlineConnection")));

builder.Services.AddSwaggerGen();

var app = builder.Build();

//Deletes and recreates database on startup. remove later.
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.EnsureDeleted();
//    db.Database.EnsureCreated();
//}


await DbSeeder.SeedAsync(app); //Seed the database

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//set response headers
app.Use(async (context, next) =>
{
    Console.WriteLine(context.Connection.RemoteIpAddress);
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'none';" +
                                                            "img-src 'self';" +
                                                            "style-src 'self' https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css;" +
                                                            "script-src 'self';" +
                                                            "connect-src 'none';" +

                                                            "form-action 'self';" +
                                                            "frame-ancestors 'none';" +
                                                            "upgrade-insecure-requests;" +
                                                            "base-uri 'self';");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=15724800; includeSubdomains; preload;");
    context.Response.Headers.Add("Cache-Control", "no-cache");
    await next();
});



app.UseAuthorization();

app.UseRateLimiter();

app.UseCors("AllowAll"); //DONT FORGET THIS LATER

app.MapControllers();

app.Run();