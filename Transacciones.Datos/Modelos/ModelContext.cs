using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Transacciones.Datos.Modelos;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cartera> Carteras { get; set; }

    public virtual DbSet<Historialsaldo> Historialsaldos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // => optionsBuilder.UseOracle("Data Source=localhost;User Id=C##transacciones_db;Password=admin;");
    }
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##TRANSACCIONES_DB")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Cartera>(entity =>
        {
            entity.HasKey(e => e.IdCartera).HasName("SYS_C008318");

            entity.ToTable("CARTERA");

            entity.Property(e => e.IdCartera)
                .HasDefaultValueSql("\"C##TRANSACCIONES_DB\".\"SEQ_AUTOI\".\"NEXTVAL\"")
                .HasColumnType("NUMBER")
                .HasColumnName("ID_CARTERA");
            entity.Property(e => e.NombreCuenta)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_CUENTA");
            entity.Property(e => e.SaldoActual)
                .HasDefaultValueSql("0\n")
                .HasColumnType("NUMBER")
                .HasColumnName("SALDO_ACTUAL");
        });

        modelBuilder.Entity<Historialsaldo>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("SYS_C008319");

            entity.ToTable("HISTORIALSALDO");

            entity.Property(e => e.IdHistorial)
                .HasDefaultValueSql("\"C##TRANSACCIONES_DB\".\"SEQ_AUTOI\".\"NEXTVAL\"")
                .HasColumnType("NUMBER")
                .HasColumnName("ID_HISTORIAL");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("FECHA");
            entity.Property(e => e.IdCartera)
                .HasColumnType("NUMBER")
                .HasColumnName("ID_CARTERA");
            entity.Property(e => e.MontoTransaccion)
                .HasColumnType("NUMBER")
                .HasColumnName("MONTO_TRANSACCION");
            entity.Property(e => e.SaldoPosterior)
                .HasColumnType("NUMBER")
                .HasColumnName("SALDO_POSTERIOR");
            entity.Property(e => e.TipoTransaccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TIPO_TRANSACCION");
            entity.Property(e => e.SaldoAnterior)
                .HasColumnType("NUMBER")
                .HasColumnName("SALDO_ANTERIOR");

            entity.HasOne(d => d.IdCarteraNavigation).WithMany(p => p.Historialsaldos)
                .HasForeignKey(d => d.IdCartera)
                .HasConstraintName("SYS_C008320");
        });
        modelBuilder.HasSequence("SEQ_AUTOI");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
