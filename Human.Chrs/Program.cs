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

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
builder.Services.AddHttpContextAccessor();

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<ChrsConfig>(configuration.GetSection("HumanConfig"));
var connectionString = builder.Configuration.GetConnectionString("EIP");
builder.Services.AddDbContext<HumanChrsContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddScoped<IDbConnection, SqlConnection>(serviceProvider => {
//    SqlConnection conn = new SqlConnection();
//    conn.ConnectionString = connectionString;
//    return conn;
//});
// 保持屬性名稱不變
builder.Services.AddMvc().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);

// Swagger
var environmentName = builder.Configuration.GetSection("HumanConfig")["EnvironmentName"];

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

builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));
builder.Services.AddHttpClient();
builder.Services.AddSingleton(provider =>
{
    var httpClientFactory = provider.GetService<IHttpClientFactory>();
    return new RestClient(httpClientFactory.CreateClient());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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