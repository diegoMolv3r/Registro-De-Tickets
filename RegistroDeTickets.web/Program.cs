using DotNetEnv;
using Google;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegistroDeTickets.Data.Entidades; 
using RegistroDeTickets.Repository;
using RegistroDeTickets.Service;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


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

/* NO SE PORQUE ANI LO TIENE ASI 
 builder.Services.AddDbContext<RegistroDeTicketsPw3Context>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("RegistroDeTickets.Data")));
 */

builder.Services.AddIdentityCore<Usuario>().AddRoles<IdentityRole<int>>() // Soporte para roles con clave 'int'
.AddEntityFrameworkStores<RegistroDeTicketsPw3Context>();

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

builder.Services.AddDataProtection();
builder.Services.AddSingleton<TokenService>();


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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        RoleClaimType = ClaimTypes.Role, // Configurar el tipo de reclamo para roles
        ClockSkew = TimeSpan.Zero
    };

    // Para leer la cookie "jwt"
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = tokenService.DecryptToken(token);
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PuedeEliminar", policy =>
    {
        policy.RequireClaim("Permiso", "Acciones_de_Alto_Riesgo");
    });
});

//uilder.Services.AddSingleton(new TokenService(builder.Configuration["Jwt:Key"]));


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

app.UseHttpsRedirection(); 

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();//jwt
app.UseAuthorization(); // Etiquetas Autorize

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=IniciarSesion}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();