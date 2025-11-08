using RegistroDeTickets.Data.Entidades; 
using RegistroDeTickets.Service;
using Microsoft.EntityFrameworkCore;
using RegistroDeTickets.Repository;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

Env.Load();


var builder = WebApplication.CreateBuilder(args);


var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
var googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
var connectionString = Environment.GetEnvironmentVariable("BASE_DE_DATOS");

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddDbContext<RegistroDeTicketsPw3Context>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();

// Agrego Application Insights para monitoreo y telemetria punto 5 del TP
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();

//jwt
var key = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "RegistroDeTickets.Web",
        ValidAudience = "RegistroDeTickets.Web",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddSingleton(new TokenService(builder.Configuration["Jwt:Key"]));


builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<
    Microsoft.AspNetCore.Identity.IPasswordHasher<RegistroDeTickets.Data.Entidades.Usuario>,
    Microsoft.AspNetCore.Identity.PasswordHasher<RegistroDeTickets.Data.Entidades.Usuario>
>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Este bloque de codigo es donde se desactiva el Debug en produccion punto 6.1 del TP
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Es bueno mantener la página de desarrollador para el modo de desarrollo
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection(); // Implementar Https Redirection punto 6.3 del TP
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();//jwt
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=IniciarSesion}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();