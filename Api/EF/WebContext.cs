using System;
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

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-2IDG3PAJ;Database=web;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.ToTable("ingreso");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("FECHA");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Passwordhash).HasMaxLength(100);
            entity.Property(e => e.Passwordsalt).HasMaxLength(100);
            entity.Property(e => e.Refreshtoken)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tokencreated).HasColumnType("datetime");
            entity.Property(e => e.Tokenexpires).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(151)
                .IsFixedLength()
                .HasColumnName("password");
            entity.Property(e => e.Passwordhash).HasMaxLength(100);
            entity.Property(e => e.Passwordsalt)
                .HasMaxLength(150)
                .HasColumnName("passwordsalt");
            entity.Property(e => e.Refreshtoken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("refreshtoken");
            entity.Property(e => e.Rol)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("rol");
            entity.Property(e => e.Tokencreated).HasColumnType("datetime");
            entity.Property(e => e.Tokenexpires).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
