global using AuthenBackend.Data;
global using AuthenBackend.models;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using AuthenBackend.DTOs.User;
global using Microsoft.AspNetCore.Mvc;
global using AutoMapper;
global using AuthenBackend.Services;
global using Microsoft.AspNetCore.Authorization;

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.OpenApi.Models;
global using Swashbuckle.AspNetCore.Filters;
using AuthenBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config => 
{
    config.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme //OAuth2 และ Bearer Token ซึ่งเป็นวิธีที่ทั่วไปในการรับรองสิทธิ์ API
    {
        Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """, //ใช้ token โดยใส่ "bearer {token}"
        In = ParameterLocation.Header,      //ระบุว่า token จะถูกส่งผ่าน header ของ HTTP request
        Name = "Authorization",             // ระบุชื่อของ header ที่จะถูกใช้สำหรับการรับรองสิทธิ์
        Type = SecuritySchemeType.ApiKey    //ระบุว่าวิธีการรับรองสิทธิ์ที่ใช้คือ API Key
    });
    config.OperationFilter<SecurityRequirementsOperationFilter>(); 
});
//AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly); //auto mapper AutoMapperProfile
//connect Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Add services 
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)), //null forgiving
                ValidateIssuer = false,
                ValidateAudience = false
            };
    });

builder.Services.AddHttpContextAccessor();



//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(b => b
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();