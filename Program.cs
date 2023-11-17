using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");

var db = redis.GetDatabase();
var ft = db.FT();
var json = db.JSON();

var UGV_tel = new {
    xPos = 69.6969,
    yPos = 120.473,
    manual = false,
    misc = "etc"
};


// json.Set("UGV_[timestamp]", "$", UGV_tel);
// Console.WriteLine(json.Get("UGV_[timestamp]")); // prints json

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
 app.UseWebSockets();

app.UseAuthorization();

app.MapControllers();

app.Run();

