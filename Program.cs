using Microsoft.EntityFrameworkCore;
using ZERO.Database;
using ZERO.Models.JWT;
using ZERO.Sevice;
using ZERO.Sevice.IService;
using ZERO.Repository;
using ZERO.Repository.IRepository;
using static Dapper.SqlMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

// Add services to the container.
builder.Services.AddTransient<IStockInfoService, StockInfoService>();
builder.Services.AddTransient<IStockInfoRepository, StockInfoRepository>();

builder.Services.AddControllers();

/*
builder.Services.AddControllers(options =>
{
    // 添加全局過濾器
    //options.Filters.Add<PermissionFilter>();

    //可以讓Body吃純string
    options.InputFormatters.Insert(0, new PlainTextInputFormatter());
});
*/
/*#region 註冊CORS的Policy
builder.Services.AddCors(options =>
{
    // Policy名稱是自訂的，可以自己改
    options.AddPolicy("AllowCors", policy =>
    {
        //設定允許跨域的來源，有多個的話可以用 ',' 隔開
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Value.Split(","))
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
    });
});
#endregion*/
//配置DB連線 My_SQL
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
