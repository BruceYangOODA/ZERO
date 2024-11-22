using Microsoft.EntityFrameworkCore;
using ZERO.Database;
using ZERO.Models.JWT;
using ZERO.Sevice;
using ZERO.Sevice.IService;
using ZERO.Repository;
using ZERO.Repository.IRepository;
using ZERO.Util;
using NLog;
using NLog.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    //刪除所有默認紀錄日誌提供程序
    //logging.ClearProviders();
    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    logging.AddConsole();
    logging.AddDebug();
    logging.AddEventSourceLogger();
    //啟用NLog作為日誌提供程序之一

    // 獲取環境名稱
    string environmentName = hostingContext.HostingEnvironment.EnvironmentName;

    // 自定義NLog配置
    var nlogConfigFileName = $"nlog.{environmentName}.config";

    var nlogConfigPath = Path.Combine(Directory.GetCurrentDirectory(), nlogConfigFileName);

    if (File.Exists(nlogConfigPath))
    {
        LogManager.Setup().LoadConfigurationFromFile(nlogConfigPath);
    }

    logging.AddNLog();
});

//builder.WebHost.UseUrls("http://0.0.0.0:5002"); // 允许绑定到所有 IP 地址

//配置DB連線 My_SQL
if (builder.Configuration.GetConnectionString("SqlType").Equals("MySQL"))
{
    builder.Services.AddDbContext<StockInfoContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("StockInfoMysql")));
}

//向 ASP.NET Core 應用程式的服務容器中註冊一個單例的 HTML 編碼器，該編碼器支援編碼所有 Unicode 字符。這樣，在應用程式中的任何地方都可以使用 HTML 編碼器來進行安全的 HTML 編碼。
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));


#region Dependency Injection
builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

builder.Services.AddTransient<IStockInfoService, StockInfoService>();
builder.Services.AddTransient<IStockInfoRepository, StockInfoRepository>();
builder.Services.AddTransient<IDayTradingService, DayTradingService>();
builder.Services.AddTransient<IDayTradingRepository, DayTradingRepository>();

#endregion

builder.Services.AddControllers();


builder.Services.AddControllers(options =>
{
    // 添加全局過濾器
    //options.Filters.Add<PermissionFilter>();

    //可以讓Body吃純string
    options.InputFormatters.Insert(0, new PlainTextInputFormatter());
});

#region 註冊CORS的Policy
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
#endregion


#region jwt配置
builder.Services.AddTransient<ITokenHelper, TokenHelper>();

//啟用JWT            
builder.Services.AddAuthentication(Options =>
{
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
    options.IncludeErrorDetails = true;
    //是否需要Https
    options.RequireHttpsMetadata = false;

    //保存Token
    options.SaveToken = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        //驗證Issuer (發行者) 是否有效
        ValidateIssuer = true,
        //設置有效的 Issuer，即預期的 Issuer
        ValidIssuer = builder.Configuration["JWTConfig:Issuer"],

        //驗證 Audience (受眾) 是否有效
        ValidateAudience = true,
        //設置有效的 Audience，即預期的 Audience
        ValidAudience = builder.Configuration["JWTConfig:Audience"],

        //驗證 Token 是否在有效期內
        ValidateLifetime = true,
        //設置時間偏移，可以讓 Token 在一定時間範圍內依然被視為有效，即使過期
        ClockSkew = TimeSpan.Zero,

        //驗證 Token 中包含的 Key，這通常用於檢查 Token 中的簽名金鑰
        ValidateIssuerSigningKey = true,
        //設置用於驗證簽名的金鑰
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTConfig:IssuerSigningKey"]))
    };

    //驗證事件捕捉
    options.Events = new JwtBearerEvents()
    {
        OnTokenValidated = context =>
        {
            //驗證成功後，後續再看要做甚麼事情...
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            //驗證失敗後，後續再看要做甚麼事情...
            return Task.CompletedTask;
        }
    };
});
#endregion


#region Swagger Configuration
//註冊Swagger生成器，定義一個或多個Swagger文件
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JpmedRWD API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });

    //if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Localtest"))
    //{
    //    //設置Swagger Json和UI的注釋路徑
    //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //    c.IncludeXmlComments(xmlPath);
    //}
});
#endregion


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// To serve PBF Files, we need to allow unknown filetypes 
// to be served by the Webserver:
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = (ctx) =>
    {
        var corsPolicyProvider = ctx.Context.RequestServices.GetRequiredService<ICorsPolicyProvider>();
        var corsService = ctx.Context.RequestServices.GetRequiredService<ICorsService>();
        var policy = corsPolicyProvider.GetPolicyAsync(ctx.Context, "AllowCors")
        .ConfigureAwait(false)
            .GetAwaiter().GetResult();

        var corsResult = corsService.EvaluatePolicy(ctx.Context, policy);
        corsService.ApplyResult(corsResult, ctx.Context.Response);
    }
});


//SwaggerUI中使用了HTML,Javascript和CSS等靜態文件,所以需要將其放在中間件UseStaticFile()之後, 啟用中間件
app.UseSwagger();

//啟用中間件Swagger()的UI服務，它需要與Swagger()配置在一起
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //Show more of the model by default
    c.DefaultModelExpandDepth(2);

    //Close all of the major nodes
    c.DocExpansion(DocExpansion.List);

    //Show the example by default
    c.DefaultModelRendering(ModelRendering.Example);

    //Turn on Try it by default
    c.EnableTryItOutByDefault();
    c.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
});

//允許使用URL存取靜態資源
//app.UseFileServer(new FileServerOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "shelves")),
//    RequestPath = "/shelves",
//    EnableDirectoryBrowsing = true //是否開啟目錄
//});

// Log response info (for response pipeline: after ExceptionMiddleware)
app.UseMiddleware<RequestLogMiddleware>();

app.UseRouting();
app.UseCors();

//啟用認證中介軟體，要寫在授權UseAuthorization()的前面
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ResponseLogMiddleware>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}



app.UseHttpsRedirection();
app.MapControllers();

app.Run();
