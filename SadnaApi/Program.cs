using Microsoft.EntityFrameworkCore;
using SadnaApi.Data;
using Microsoft.AspNetCore.Identity;
using SadnaApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SadnaApi.Controllers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the database connection
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SadnaProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
});

// Configure Identity settings
builder.Services.Configure<IdentityOptions>(options =>
{
    // Disable password policy
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

// Add Identity
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

// Create roles in database if they don't exist
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!roleManager.RoleExistsAsync("user").Result)
    {
        var role = new IdentityRole("user");
        roleManager.CreateAsync(role).Wait();
    }
    if (!roleManager.RoleExistsAsync("admin").Result)
    {
        var role = new IdentityRole("admin");
        roleManager.CreateAsync(role).Wait();
    }
}

// adding JWT support
builder.Services.AddScoped<JwtUtils>();

// adding authentication for JWT support
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SomeSecretKeyBecauseImJustTooLazy")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Injected logger instance for the controller or service where you want to log
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UsersDBController>>();
                logger.LogInformation("Token validated successfully.");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // Injected logger instance for the controller or service where you want to log
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UsersDBController>>();
                logger.LogError(context.Exception, "Authentication failed.");

                var tokenValue = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(tokenValue))
                {
                    // Log the token content
                    logger.LogInformation($"JWT Token Content: {tokenValue}");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// use CORS for all request authorizing
app.UseCors(options =>
        options.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()

);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Use JWT authentication middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
