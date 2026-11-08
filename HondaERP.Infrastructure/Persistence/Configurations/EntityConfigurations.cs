using HondaERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HondaERP.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.HasData(
            new Branch
            {
                Id = 1,
                Name = "Honda Cantt Peshawar",
                Address = "Bara Gate, Peshawar",
                IsActive = true
            },
            new Branch
            {
                Id = 2,
                Name = "Tariq Honda",
                Address = "Lahori Gate, Peshawar",
                IsActive = true
            });
    }
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Mobile)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.HasIndex(x => x.Mobile);
    }
}

public class MotorcycleConfiguration
    : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.Property(x => x.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RegistrationNumber)
            .HasMaxLength(30);

        builder.Property(x => x.ChassisNumber)
            .HasMaxLength(100);

        builder.Property(x => x.EngineNumber)
            .HasMaxLength(100);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Motorcycles)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductCategoryConfiguration
    : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.ProductCategory)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductAliasConfiguration
    : IEntityTypeConfiguration<ProductAlias>
{
    public void Configure(EntityTypeBuilder<ProductAlias> builder)
    {
        builder.Property(x => x.Alias)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => new { x.ProductId, x.Alias })
            .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Aliases)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BranchStockConfiguration
    : IEntityTypeConfiguration<BranchStock>
{
    public void Configure(EntityTypeBuilder<BranchStock> builder)
    {
        // Har branch/product ka sirf aik stock record.
        builder.HasIndex(x => new { x.BranchId, x.ProductId })
            .IsUnique();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.MinimumQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.AverageCost)
            .HasPrecision(18, 4);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasOne(x => x.Branch)
            .WithMany(x => x.Stocks)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Stocks)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockMovementConfiguration
    : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.Property(x => x.QuantityChange)
            .HasPrecision(18, 3);

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 4);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedByUserId)
            .HasMaxLength(450);

        builder.HasIndex(x => new
        {
            x.BranchId,
            x.ProductId,
            x.CreatedAtUtc
        });

        builder.HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}