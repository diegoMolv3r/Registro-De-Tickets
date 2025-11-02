using RegistroDeTickets.Data.Entidades; 
using RegistroDeTickets.Service;
using Microsoft.EntityFrameworkCore;
using RegistroDeTickets.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("EFCoreContext");
builder.Services.AddDbContext<RegistroDeTicketsPw3Context>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();


builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=IniciarSesion}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();