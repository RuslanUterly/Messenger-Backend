using Messenger.Api.Context;
using Messenger.Api.Features.Auth.Jwt;
using Messenger.Api.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);

// Регистрация DbContext (PostgreSQL) + Wolverine
builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(
        builder.Configuration.GetConnectionString("MessengerDatabase")!, "wolverine");

    opts.UseEntityFrameworkCoreTransactions();

    opts.Policies.OnException<PostgresException>()
        .RetryWithCooldown(TimeSpan.FromMilliseconds(50), TimeSpan.FromSeconds(1));
});

builder.Services.AddDbContext<MessengerContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MessengerDatabase"));
},
// DbContextOptions как Singleton - значимая оптимизация для Wolverine
optionsLifetime: ServiceLifetime.Singleton);

// Регистрация Email сервиса
builder.Services
    .AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Аутентификация и авторизация
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<TokenProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt?.Issuer,
        ValidAudience = jwt?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt!.SecretKey))
    };
});

builder.Services.AddAuthorization();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
