using Database.Handlers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<RabbitMqConsumer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(Options =>
    Options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4000") // Allow requests from your Vue.js frontend
           .AllowAnyHeader()
           .AllowAnyMethod();
});

app.UseWebSockets();
app.UseAuthorization();
app.MapControllers();
app.Run();
