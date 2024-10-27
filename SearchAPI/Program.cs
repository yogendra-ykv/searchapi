using AspNetCoreRateLimit;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchAPI;
using SearchAPI.Middleware;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SearchAPI",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header,
      },
      new List < string > ()
    }
  });
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddSingleton(new TokenService(
builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Audience"], builder.Configuration["Jwt:Key"]));
builder.Services.AddInMemoryRateLimiting();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule> {
    new RateLimitRule {
      Endpoint = "*",
      Limit = 100,
      Period = "1s"
    }
  };
});
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitConfiguration,
RateLimitConfiguration>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchAPI V1");
});

app.UseIpRateLimiting();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseHangfireDashboard();
app.MapGet("/enqueue-job", () =>
{
    BackgroundJob.Enqueue(() => Console.WriteLine("Hello from Hangfire!"));
    return Results.Ok("Job has been enqueued.");
});

app.Run();
