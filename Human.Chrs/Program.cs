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

    // �إ� Job
    var jobKey = new JobKey("UpdateStaffInfo", "UpdateStaffInfoGroup");
    quartz.AddJob<StaffInfoUpdateJob>(opts =>
    {
        opts.WithIdentity(jobKey);
        opts.StoreDurably();
    });

    // �إ�Ĳ�o���A�۰ʰ��� Job
    quartz.AddTrigger(opts =>
    {
        opts.ForJob(jobKey);
        opts.WithIdentity("UpdateStaffInfoTrigger", "UpdateStaffInfoGroup");
        opts.WithCronSchedule("0 15 1 * * ?");
        // �C�ѭ��1:00����
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
// �O���ݩʦW�٤���
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
    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "HumanChrs.xml"); // xml�����X�֨�swagger
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
            .AllowAnyMethod(); // ���\����ӷ��ШDAPI
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

// �ݭn�[�W�o�q�ϥ� swagger �ɤ~���|404
app.MapControllers();

app.Run();