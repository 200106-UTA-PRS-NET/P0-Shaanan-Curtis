using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PizzaBox.Storing.Entities
{
    public partial class pizzaboxContext : DbContext
    {
        public pizzaboxContext()
        {
        }

        public pizzaboxContext(DbContextOptions<pizzaboxContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Ordertype> Ordertype { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.StoreId)
                    .HasName("PRIMARY");

                entity.ToTable("inventory");

                entity.HasIndex(e => e.StoreId)
                    .HasName("StoreID")
                    .IsUnique();

                entity.Property(e => e.StoreId)
                    .HasColumnName("StoreID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.Custom).HasColumnType("mediumint(9)");

                entity.Property(e => e.Preset).HasColumnType("mediumint(9)");

                entity.HasOne(d => d.Store)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(d => d.StoreId)
                    .HasConstraintName("INV_STORE_FK");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("orders");

                entity.HasIndex(e => e.OrderId)
                    .HasName("OrderID")
                    .IsUnique();

                entity.HasIndex(e => e.StoreId)
                    .HasName("ORDERS_U");

                entity.HasIndex(e => e.Username)
                    .HasName("ORDERS_S");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.StoreId)
                    .HasColumnName("StoreID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("char(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ORDERS_U");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ORDERS_S");
            });

            modelBuilder.Entity<Ordertype>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("ordertype");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.Custom)
                    .IsRequired()
                    .HasColumnType("char(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Dt)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Preset)
                    .IsRequired()
                    .HasColumnType("char(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Tm)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Order)
                    .WithOne(p => p.Ordertype)
                    .HasForeignKey<Ordertype>(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("OT_O_FK");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("store");

                entity.HasIndex(e => e.StoreId)
                    .HasName("StoreID")
                    .IsUnique();

                entity.HasIndex(e => new { e.City, e.State })
                    .HasName("Location")
                    .IsUnique();

                entity.Property(e => e.StoreId)
                    .HasColumnName("StoreID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("char(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnType("char(2)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasColumnType("char(10)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PRIMARY");

                entity.ToTable("user");

                entity.HasIndex(e => e.Username)
                    .HasName("Username")
                    .IsUnique();

                entity.Property(e => e.Username)
                    .HasColumnType("char(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("char(60)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasColumnType("char(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.SessionLive).HasColumnType("tinyint(4)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
