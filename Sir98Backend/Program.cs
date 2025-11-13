using Sir98Backend.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ActivityRepo>(new ActivityRepo());

builder.Services.AddSingleton<InstructorRepo>(new InstructorRepo());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
