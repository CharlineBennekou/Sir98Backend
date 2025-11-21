using Sir98Backend.Controllers;
using Sir98Backend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ActivityRepo>(new ActivityRepo());

builder.Services.AddSingleton<InstructorRepo>(new InstructorRepo());

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll"); //DONT FORGET THIS LATER

app.MapControllers();

app.Run();
