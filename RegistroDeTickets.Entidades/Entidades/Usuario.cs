using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // <-- AÑADIR ESTE USING

namespace RegistroDeTickets.Data.Entidades;

// AÑADIR HERENCIA: IdentityUser<int>
public partial class Usuario : IdentityUser<int>
{
    // --- PROPIEDADES ELIMINADAS (porque ya vienen de IdentityUser) ---
    // public int Id { get; set; }
    // public string Username { get; set; } = null!;
    // public string Email { get; set; } = null!;
    // public string PasswordHash { get; set; } = null!;

    // --- TUS PROPIEDADES PERSONALIZADAS (SE QUEDAN) ---
    public string? TokenHashRecuperacion { get; set; }
    public DateTime? TokenHashRecuperacionExpiracion { get; set; }
    public string Estado { get; set; } = null!;

    // --- TUS NAVEGACIONES (SE QUEDAN) ---
    public virtual Administrador? Administrador { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual Tecnico? Tecnico { get; set; }
}