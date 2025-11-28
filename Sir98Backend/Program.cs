using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sir98Backend.Repository;
using System.Text;
using System.Threading.RateLimiting;
using Sir98Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ActivityRepo>(new ActivityRepo());

builder.Services.AddSingleton<InstructorRepo>(new InstructorRepo());
builder.Services.AddSingleton<UserRepo>();

builder.Services.AddSingleton<ChangedActivityRepo>();

builder.Services.AddSingleton<ActivityOccurrenceService>();


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

string filepath = Environment.CurrentDirectory + "\\Keys\\JWToken key for signing.txt";
string keyForSigning = System.IO.File.ReadAllText(filepath);
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
    //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    //RateLimitPartition.GetTokenBucketLimiter(
    //    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
    //    factory: partition => new TokenBucketRateLimiterOptions
    //    {
    //        AutoReplenishment = true,
    //        TokenLimit = 3,
    //        TokensPerPeriod = 1,
    //        ReplenishmentPeriod = TimeSpan.FromSeconds(1),

    //    }));

    options.AddTokenBucketLimiter("userLoginRegisterForgot", options =>
    {
        options.AutoReplenishment = true;
        options.TokenLimit = 3;
        options.TokensPerPeriod = 1;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

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

var app = builder.Build();

// Configure the HTTP request pipeline.

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
