using Human.Repository.EF;
using Human.Chrs.Domain.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using RestSharp;
using Human.Chrs.Infra.Swagger;
using Human.Repository.AutoMapper;
using Human.Chrs.Domain.IRepository;
using Human.Repository.Repository;
using Human.Chrs.Infrastructure.Repositories;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain;
using Quartz;
using Human.Chrs.ScheduleJob;
using Human.Chrs.Domain.Websocket;
using Human.Chrs.Infra.Middleware;
using Human.Repository.SubscribeTableDependencies;
using SignalR_SqlTableDependency.MiddlewareExtensions;
using SendGrid;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddQuartz(quartz =>
{
    quartz.UseMicrosoftDependencyInjectionJobFactory();

    // 建立 Job
    var jobKey = new JobKey("UpdateStaffInfo", "UpdateStaffInfoGroup");
    quartz.AddJob<StaffInfoUpdateJob>(opts =>
    {
        opts.WithIdentity(jobKey);
        opts.StoreDurably();
    });

    // 建立觸發器，自動執行 Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey);
        opts.WithIdentity("UpdateStaffInfoTrigger", "UpdateStaffInfoGroup");
        opts.WithCronSchedule("0 15 1 * * ?");
        // 每天凌晨1:00執行
    });
});

var env = builder.Environment;
builder.Services.AddHttpContextAccessor();

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<ChrsConfig>(configuration.GetSection("HumanConfig"));
var connectionString = configuration.GetConnectionString("SqlConnection");

var sendGridClient = new SendGridClient(builder.Configuration["SendGridApiKey"]);
builder.Services.AddSingleton(sendGridClient);

builder.Services.AddSingleton<WebSocketHandler>();
//builder.Services.AddSingleton<SqlNotificationService>(sp =>
//{
//    var webSocketHandler = sp.GetRequiredService<WebSocketHandler>();
//    return new SqlNotificationService(connectionString, webSocketHandler);
//});

builder.Services.AddSingleton(sp =>
    new SubscribeNotificationLogsDependency(
        connectionString,
        sp.GetRequiredService<IServiceScopeFactory>(),
        sp.GetRequiredService<WebSocketHandler>()
    ));
builder.Services.AddSingleton(sp =>
    new SubscribeAdminNotificationLogsDependency(
        connectionString,
        sp.GetRequiredService<IServiceScopeFactory>(),
        sp.GetRequiredService<WebSocketHandler>()
    ));

builder.Services.AddDbContext<HumanChrsContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDbConnection, SqlConnection>(serviceProvider =>
{
    SqlConnection conn = new SqlConnection();
    conn.ConnectionString = connectionString;
    return conn;
});
// 保持屬性名稱不變
builder.Services.AddMvc().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

// Swagger
var environmentName = configuration.GetSection("HumanConfig")["EnvironmentName"];

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Title = $"{environmentName} Chrs API", Version = "v1" });
    swagger.OperationFilter<SwaggerFileOperationFilter>();
    swagger.OperationFilter<AddRequireHeaderParameter>();
    swagger.OperationFilter<SwaggerBodyTypeOperationFilter>();
    swagger.EnableAnnotations();
    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "HumanChrs.xml"); // xml註釋合併到swagger
    //swagger.IncludeXmlComments(filePath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "HumanChrs",
        builder =>
        {
            builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod(); // 允許任何來源請求API
        });
});

builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<GeocodingService>();

builder.Services.AddScoped<AdminDomain>();
builder.Services.AddScoped<CheckInAndOutDomain>();
builder.Services.AddScoped<StaffDomain>();
builder.Services.AddScoped<LoginDomain>();
builder.Services.AddScoped<SuperDomain>();
builder.Services.AddScoped<ScheduleDomain>();

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ICheckRecordsRepository, CheckRecordsRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyRuleRepository, CompanyRuleRepository>();
builder.Services.AddScoped<IOverTimeLogRepository, OverTimeLogRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IPersonalDetailRepository, PersonalDetailRepository>();
builder.Services.AddScoped<IVacationLogRepository, VacationLogRepository>();
builder.Services.AddScoped<IEventLogsRepository, EventLogsRepository>();
builder.Services.AddScoped<IIncomeLogsRepository, IncomeLogsRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ISalarySettingRepository, SalarySettingRepository>();
builder.Services.AddScoped<IAmendCheckRecordRepository, AmendCheckRecordRepository>();
builder.Services.AddScoped<IMeetLogRepository, MeetLogRepository>();
builder.Services.AddScoped<INotificationLogsRepository, NotificationLogsRepository>();
builder.Services.AddScoped<IReadLogsRepository, ReadLogsRepository>();
builder.Services.AddScoped<IAdminNotificationLogsRepository, AdminNotificationLogsRepository>();
builder.Services.AddScoped<IAdminReadLogsRepository, AdminReadLogsRepository>();
builder.Services.AddScoped<IResetPasswordLogsRepository, ResetPasswordLogsRepository>();
builder.Services.AddScoped<IContractTypeRepository, ContractTypeRepository>();
builder.Services.AddScoped<IShiftWorkListRepository, ShiftWorkListRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));
builder.Services.AddHttpClient();
builder.Services.AddSingleton(provider =>
{
    var httpClientFactory = provider.GetService<IHttpClientFactory>();
    return new RestClient(httpClientFactory.CreateClient());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint($"{builder.Configuration["PATHBASE"] ?? string.Empty}/swagger/v1/swagger.json", "IALW API v1");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// UseCors is now placed between UseRouting and UseAuthorization for correct ordering.
app.UseCors("HumanChrs");

app.UseAuthorization();
app.UseMiddleware<WebSocketsMiddleware>();

app.MapRazorPages();

// Modified the Swagger section to avoid 404 and grouped it with the app.UseEndpoints.
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSqlTableDependency<SubscribeNotificationLogsDependency>();
app.UseSqlTableDependency<SubscribeAdminNotificationLogsDependency>();

app.Run();