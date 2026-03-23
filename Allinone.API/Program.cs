using Allinone.API.Events;
using Allinone.API.Filters;
using Allinone.API.Middleware;
using Allinone.API.Services;
using Allinone.BLL;
using Allinone.BLL.Auditlogs;
using Allinone.BLL.Installments;
using Allinone.BLL.Diarys;
using Allinone.BLL.DS.Accounts;
using Allinone.BLL.DS.DSItems;
using Allinone.BLL.DS.Transactions;
using Allinone.BLL.Kanbans;
using Allinone.BLL.Members;
using Allinone.BLL.Shops;
using Allinone.BLL.Todolists;
using Allinone.BLL.Trips;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.DLL.UnitOfWork;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add AutoMapper to DI container
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<IMapModel, MapModel>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

#region Repositories_Services
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITodolistService, TodolistService>();
builder.Services.AddScoped<ITodolistDoneService, TodolistDoneService>();
builder.Services.AddScoped<IDSAccountService, DSAccountService>();
builder.Services.AddScoped<IDSItemService, DSItemService>();
builder.Services.AddScoped<IDSTransactionService, DSTransactionService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IDSItemSubService, DSItemSubService>();
builder.Services.AddScoped<IKanbanService, KanbanService>();
builder.Services.AddScoped<IShopTypeService, ShopTypeService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IShopDiaryService, ShopDiaryService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripDetailTypeService, TripDetailTypeService>();
builder.Services.AddScoped<ITripDetailService, TripDetailService>();
builder.Services.AddScoped<IDiaryActivityService, DiaryActivityService>();
builder.Services.AddScoped<IDiaryEmotionService, DiaryEmotionService>();
builder.Services.AddScoped<IDiaryFoodService, DiaryFoodService>();
builder.Services.AddScoped<IDiaryLocationService, DiaryLocationService>();
builder.Services.AddScoped<IDiaryBookService, DiaryBookService>();
builder.Services.AddScoped<IDiaryWeatherService, DiaryWeatherService>();
builder.Services.AddScoped<IDiaryService, DiaryService>();
builder.Services.AddScoped<IDiaryTypeService, DiaryTypeService>();
builder.Services.AddScoped<IDiaryDetailService, DiaryDetailService>();
builder.Services.AddScoped<IAuditlogService, AuditlogService>();
builder.Services.AddScoped<IInstallmentService, InstallmentService>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITodolistRepository, TodolistRepository>();
builder.Services.AddScoped<ITodolistDoneRepository, TodolistDoneRepository>();
builder.Services.AddScoped<IDSAccountRepository, DSAccountRepository>();
builder.Services.AddScoped<IDSItemRepository, DSItemRepository>();
builder.Services.AddScoped<IDSTransactionRepository, DSTransactionRepository>();
builder.Services.AddScoped<IDSItemSubRepository, DSItemSubRepository>();
builder.Services.AddScoped<IKanbanRepository, KanbanRepository>();
builder.Services.AddScoped<IShopTypeRepository, ShopTypeRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IShopDiaryRepository, ShopDiaryRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripDetailTypeRepository, TripDetailTypeRepository>();
builder.Services.AddScoped<ITripDetailRepository, TripDetailRepository>();
builder.Services.AddScoped<IDiaryActivityRepository, DiaryActivityRepository>();
builder.Services.AddScoped<IDiaryEmotionRepository, DiaryEmotionRepository>();
builder.Services.AddScoped<IDiaryFoodRepository, DiaryFoodRepository>();
builder.Services.AddScoped<IDiaryLocationRepository, DiaryLocationRepository>();
builder.Services.AddScoped<IDiaryBookRepository, DiaryBookRepository>();
builder.Services.AddScoped<IDiaryWeatherRepository, DiaryWeatherRepository>();
builder.Services.AddScoped<IDiaryRepository, DiaryRepository>();
builder.Services.AddScoped<IDiaryTypeRepository, DiaryTypeRepository>();
builder.Services.AddScoped<IDiaryDetailRepository, DiaryDetailRepository>();
builder.Services.AddScoped<IAuditlogRepository, AuditlogRepository>();
builder.Services.AddScoped<IInstallmentRepository, InstallmentRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();
#endregion

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<MemoryCacheHelper>();
builder.Services.AddScoped<IIdleTimeTrackingService, IdleTimeTrackingService>();

// Background service to clean up expired sessions
builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddDbContext<DSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DSConnection"),
    sql => sql.MigrationsAssembly("Allinone.DLL")));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Disable built-in validation
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<GlobalResponseFilter>();
    options.Filters.Add<ValidateModelFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Compress===================================================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>(); // Add Brotli first
    options.Providers.Add<GzipCompressionProvider>();   // Fallback to Gzip
});

// Optional: configure Brotli compression level
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal; // Or Fastest if you prefer speed
});

// Optional: configure Gzip too
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "yourIssuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your_super_secret_key_that_is_long_enough_123!"))
        };

        // Configure events with dependency injection
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                var serviceProvider = context.HttpContext.RequestServices;
                var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                var tokenBlacklistRepository = serviceProvider.GetRequiredService<ITokenBlacklistRepository>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var customEvents = new CustomBearerEvents(idleTimeService, tokenBlacklistRepository, configuration);
                await customEvents.AuthenticationFailed(context);
            },
            OnTokenValidated = async context =>
            {
                var serviceProvider = context.HttpContext.RequestServices;
                var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                var tokenBlacklistRepository = serviceProvider.GetRequiredService<ITokenBlacklistRepository>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var customEvents = new CustomBearerEvents(idleTimeService, tokenBlacklistRepository, configuration);
                await customEvents.TokenValidated(context);
            },
            OnMessageReceived = async context =>
            {
                var serviceProvider = context.HttpContext.RequestServices;
                var idleTimeService = serviceProvider.GetRequiredService<IIdleTimeTrackingService>();
                var tokenBlacklistRepository = serviceProvider.GetRequiredService<ITokenBlacklistRepository>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var customEvents = new CustomBearerEvents(idleTimeService, tokenBlacklistRepository, configuration);
                await customEvents.MessageReceived(context);
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

var app = builder.Build();

// Check database connection on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DSContext>();
    await CheckDatabaseConnection(dbContext);
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtDebugMiddleware>();
app.UseResponseCompression();

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger", permanent: false);
    return Task.CompletedTask;
});

app.Run();

static async Task CheckDatabaseConnection(DSContext context)
{
    try
    {
        var isConnected = await context.Database.CanConnectAsync();
        if (isConnected)
        {
            Console.WriteLine("Connection successful.");
        }
        else
        {
            Console.WriteLine("Unable to connect to the database.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
