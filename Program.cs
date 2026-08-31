using System.Text;
using ControleFinanceiro_Backend.Data;
using ControleFinanceiro_Backend.Repositorios;
using ControleFinanceiro_Backend.Servicos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtChave = builder.Configuration["Jwt:Chave"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtChave)),
            ValidateIssuer = true, ValidIssuer = builder.Configuration["Jwt:Emissor"],
            ValidateAudience = true, ValidAudience = builder.Configuration["Jwt:Audiencia"],
            ValidateLifetime = true, ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<JwtServico>();
builder.Services.AddScoped<AuthRepositorio>();
builder.Services.AddScoped<ContaRepositorio>();
builder.Services.AddScoped<CategoriaRepositorio>();
builder.Services.AddScoped<MetaRepositorio>();
builder.Services.AddScoped<TransacaoRepositorio>();
builder.Services.AddScoped<OrcamentoRepositorio>();
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins(
                "http://localhost:4200",
                "http://localhost:8080",
                "http://localhost")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
