using Cinema.Application.DTO.Auth;
using Cinema.Application.Services.Interfaces;
using Cinema.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapPost("/auth/register",
    async (RegisterRequest request, IAuthService authService) =>
    {
        var result = await authService.RegisterAsync(request);
        return Results.Ok(result);
    })
    .WithName("Register")
    .WithTags("Auth");

app.MapPost("/auth/login",
    async (LoginRequest request, IAuthService authService) =>
    {
        var result = await authService.LoginAsync(request);
        return Results.Ok(result);
    })
    .WithName("Login")
    .WithTags("Auth");
app.Run();
