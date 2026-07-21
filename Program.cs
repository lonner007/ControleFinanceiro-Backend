using System.Text;
using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Repositorios;
using ControleFinanceiro_Backend.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtChave = builder.Configuration["Jwt:Chave"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtChave)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Emissor"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// Swagger com botão de autenticação JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositórios e Serviços
builder.Services.AddScoped<JwtServico>();
builder.Services.AddScoped<AuthRepositorio>();
builder.Services.AddScoped<ContaRepositorio>();
builder.Services.AddScoped<CategoriaRepositorio>();
builder.Services.AddScoped<MetaRepositorio>();
builder.Services.AddScoped<TransacaoRepositorio>();

// CORS — só permite o frontend
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyMethod()
         .AllowAnyHeader()));

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication(); // Deve vir ANTES de UseAuthorization
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
