using CryptoSimulator.DataContext.Context;
using CryptoSimulator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Connection String
builder.Services.AddDbContext<CryptoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CryptoSimulatorContext")));

// Services
builder.Services.AddScoped<ICryptoCurrencyService, CryptoCurrencyService>();
builder.Services.AddScoped<IUserService, UserService>();

// Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
