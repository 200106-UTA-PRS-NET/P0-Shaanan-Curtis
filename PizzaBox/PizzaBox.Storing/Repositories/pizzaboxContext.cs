using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PizzaBox.Storing.Repositories
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
        public virtual DbSet<Ordi> Ordi { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=0Shaan;database=pizzabox", x => x.ServerVersion("5.5.62-mysql"));
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

            modelBuilder.Entity<Ordi>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("ordi");

                entity.HasIndex(e => e.OrderId)
                    .HasName("OrderID")
                    .IsUnique();

                entity.HasIndex(e => e.StoreId)
                    .HasName("ORDI_STORE_FK");

                entity.HasIndex(e => e.User)
                    .HasName("ORDI_USER_FK");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Custom)
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Od)
                    .IsRequired()
                    .HasColumnName("OD")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Ot)
                    .IsRequired()
                    .HasColumnName("OT")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Pizzas).HasColumnType("mediumint(9)");

                entity.Property(e => e.Preset)
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.StoreId)
                    .HasColumnName("StoreID")
                    .HasColumnType("mediumint(9)");

                entity.Property(e => e.User)
                    .IsRequired()
                    .HasColumnType("char(30)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Ordi)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ORDI_STORE_FK");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Ordi)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ORDI_USER_FK");
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
