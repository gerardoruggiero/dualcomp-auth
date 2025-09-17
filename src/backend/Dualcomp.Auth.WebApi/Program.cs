using Dualcomp.Auth.DataAccess.EntityFramework.Configurations;
using DualComp.Infraestructure.Web.ExceptionHandling;
using DualComp.Infraestructure.Web.Authentication;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Mail.Extensions;
using Dualcomp.Auth.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddBaseInfraestructure(builder.Configuration);

// Add Mail Services
builder.Services.AddMailServices(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Security Services
builder.Services.Configure<PasswordValidationSettings>(builder.Configuration.GetSection("PasswordValidation"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Register configuration objects directly for dependency injection
builder.Services.AddSingleton(builder.Configuration.GetSection("PasswordValidation").Get<PasswordValidationSettings>()!);
builder.Services.AddSingleton(builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!);

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IPasswordValidator, PasswordValidator>();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Company Contact Service
builder.Services.AddScoped<Dualcomp.Auth.Application.Companies.ICompanyContactService, Dualcomp.Auth.Application.Companies.CompanyContactService>();

// Register Company Settings Service
builder.Services.AddScoped<Dualcomp.Auth.Application.Services.ICompanySettingsService, Dualcomp.Auth.Application.Services.CompanySettingsService>();

// Auto-register all application handlers (Commands and Queries)
builder.Services.AddApplicationHandlers();

const string corsPolicy = "CorsPolicy";
builder.Services.AddCors(o => o.AddPolicy(corsPolicy,
    b => b.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

var app = builder.Build();
app.UseCors(corsPolicy);

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseJwtAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
