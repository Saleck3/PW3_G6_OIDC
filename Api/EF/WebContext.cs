﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.EF;

public partial class WebContext : DbContext
{
    public WebContext()
    {
    }

    public WebContext(DbContextOptions<WebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:web");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ingreso__3214EC27764B63D6");

            entity.ToTable("ingreso");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Ingresos)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ingreso__USER_ID__5165187F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3214EC07D3F548E2");

            entity.ToTable("roles");

            entity.Property(e => e.Nombre)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__usuario__3213E83F69B914CC");

            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(151)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Passwordhash).HasMaxLength(100);
            entity.Property(e => e.Passwordsalt)
                .HasMaxLength(150)
                .HasColumnName("passwordsalt");
            entity.Property(e => e.Refreshtoken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("refreshtoken");
            entity.Property(e => e.Rol).HasColumnName("rol");
            entity.Property(e => e.Tokencreated).HasColumnType("datetime");
            entity.Property(e => e.Tokenexpires).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.RolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Rol)
                .HasConstraintName("FK__usuario__rol__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
