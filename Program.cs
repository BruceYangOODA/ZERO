using Microsoft.EntityFrameworkCore;
using ZERO.Database;
using ZERO.Models.JWT;
using ZERO.Sevice;
using ZERO.Sevice.IService;
using ZERO.Repository;
using ZERO.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

// Add services to the container.
builder.Services.AddTransient<IStockInfoService, StockInfoService>();
builder.Services.AddTransient<IStockInfoRepository, StockInfoRepository>();

builder.Services.AddControllers();

//°t¸mDB³s½u My_SQL
if (builder.Configuration.GetConnectionString("SqlType").Equals("MySQL"))
{
    builder.Services.AddDbContext<StockInfoContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("StockInfoMysql")));
}


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
