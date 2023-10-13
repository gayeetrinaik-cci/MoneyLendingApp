using Application;
using Application.Interface;
using Hangfire;
using Hangfire.MySql;
//using Hangfire.MySql.Core;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Interface;
using Infrastructure.Services;
using Infrastructure.Services.BankingService;
using Infrastructure.Services.EmailService;
using Infrastructure.Services.EmailService.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using MoneyLendingApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DBConnectionString"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DBConnectionString"))));
builder.Services.AddHttpClient("HttpWrapper", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7010/api/");

    // using Microsoft.Net.Http.Headers;
    // The GitHub API requires two headers.
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/json");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.UserAgent, "MoneyLendingApp");
})
    .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }

    }); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<IRepository<Company>, Repository<Company>>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddTransient<IEmailService,EmailService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPaymentSceduleRepository, PaymentScheduleRepository>();
builder.Services.AddScoped<IPaymentScheduleService, PaymentScheduleService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IPenaltyRepository,PenaltyRepositor>();
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IHttpClientAgent, HttpClientAgent>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()));

builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    builder.Configuration.GetConnectionString("DBConnectionString"),
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire",
                    }
                )
            ));

builder.Services.AddHangfireServer(options => options.WorkerCount = 1);
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();
