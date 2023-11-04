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

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:web");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingreso>(entity =>
        {
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
                .HasConstraintName("user_id");
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
