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
using LineTag.Infrastructure.Repositories;
using Human.Chrs.Domain.Services;
using Human.Chrs.Domain;
using Quartz;
using Human.Chrs.ScheduleJob;

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("HumanChrs");
app.UseAuthorization();

app.MapRazorPages();

// 需要加上這段使用 swagger 時才不會404
app.MapControllers();

app.Run();