using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace RegistroDeTickets.Data.Entidades;

public partial class RegistroDeTicketsPw3Context : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public RegistroDeTicketsPw3Context()
    {
    }

    public RegistroDeTicketsPw3Context(DbContextOptions<RegistroDeTicketsPw3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<ReporteTecnico> ReporteTecnicos { get; set; }

    public virtual DbSet<Tecnico> Tecnicos { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketEstado> TicketEstados { get; set; }

    public virtual DbSet<TicketPrioridad> TicketPrioridads { get; set; }
    //Entity ya lo maneja
    //public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("BASE_DE_DATOS"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 
        base.OnModelCreating(modelBuilder);
        //
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Admin");

            entity.ToTable("Administrador");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Administrador)
                .HasForeignKey<Administrador>(d => d.Id)
                .HasConstraintName("FK_Admin_Usuario");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.Id)
                .HasConstraintName("FK_Cliente_Usuario");
        });

        modelBuilder.Entity<ReporteTecnico>(entity =>
        {
            entity.HasKey(e => e.IdReporte);

            entity.ToTable("ReporteTecnico");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.ReporteTecnicos)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket");
        });

        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.ToTable("Tecnico");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Tecnico)
                .HasForeignKey<Tecnico>(d => d.Id)
                .HasConstraintName("FK_Tecnico_Usuario");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.Property(e => e.EstadoId).HasDefaultValue(1);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdCliente).HasColumnName("Id_cliente");
            entity.Property(e => e.IdTecnico).HasColumnName("Id_tecnico");
            entity.Property(e => e.Motivo).HasMaxLength(50);

            entity.HasOne(d => d.Estado).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Estado");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Cliente");

            entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdTecnico)
                .HasConstraintName("FK_Ticket_Tecnico");

            entity.HasOne(d => d.Prioridad).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.PrioridadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Prioridad");
        });

        modelBuilder.Entity<TicketEstado>(entity =>
        {
            entity.ToTable("TicketEstado");

            entity.HasIndex(e => e.Nombre, "UQ_TicketEstado_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<TicketPrioridad>(entity =>
        {
            entity.ToTable("TicketPrioridad");

            entity.HasIndex(e => e.Nombre, "UQ_TicketPrioridad_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        /* SE ELIMINA PARA USAR IDENTITY
         * modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_usuario");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Id).IsUnique();

            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(20);

            entity.Property(e => e.TokenHashRecuperacion).HasColumnType("NVARCHAR(MAX)");

            entity.Property(e => e.TokenHashRecuperacionExpiracion).HasColumnType("DATETIME2");
        });
        */

        modelBuilder.Entity<Usuario>(entity =>
        {
            // Identity  usa la tabla existente 'Usuario'
            entity.ToTable("Usuario");

            // Se mapea la propiedad 'UserName' (con 'N') de Identity...
            entity.Property(e => e.UserName)
                  .HasMaxLength(20)
                  .HasColumnName("Username"); // ...a tu columna 'Username' (con 'n')

            // Recreamos el índice único sobre la *propiedad* correcta
            entity.HasIndex(e => e.UserName).IsUnique();

            // Mapeamos las propiedades heredadas (Email)
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();

            // Mapeamos tus propiedades personalizadas (se quedan igual)
            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.TokenHashRecuperacion).HasColumnType("NVARCHAR(MAX)");
            entity.Property(e => e.TokenHashRecuperacionExpiracion).HasColumnType("DATETIME2");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
