using System;
using System.Collections.Generic;
using System.Text;

namespace HondaERP.Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<BranchStock> Stocks { get; set; }
            = new List<BranchStock>();
    }

    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string? Address { get; set; }

        public bool WhatsAppConsent { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<Motorcycle> Motorcycles { get; set; }
            = new List<Motorcycle>();
    }

    public class Motorcycle
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public string Model { get; set; } = "";

        public string? RegistrationNumber { get; set; }
        public string? ChassisNumber { get; set; }
        public string? EngineNumber { get; set; }

        public int? ModelYear { get; set; }
    }

    public class ProductCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }


    public class Product
    {
        public int Id { get; set; }

        public string Code { get; set; } = "";
        public string Name { get; set; } = "";

        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = null!;

        public string Unit { get; set; } = "Piece";

        public decimal SalePrice { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductAlias> Aliases { get; set; }
            = new List<ProductAlias>();

        public ICollection<BranchStock> Stocks { get; set; }
            = new List<BranchStock>();
    }

    public class ProductAlias
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Alias { get; set; } = "";
    }

    public class BranchStock
    {
        public int Id { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal Quantity { get; set; }
        public decimal MinimumQuantity { get; set; }

        public decimal AverageCost { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    public enum StockMovementType
    {
        Opening = 1,
        PurchaseReceived = 2,
        Sale = 3,
        CustomerReturn = 4,
        SupplierReturn = 5,
        TransferOut = 6,
        TransferIn = 7,
        Adjustment = 8
    }

    public class StockMovement
    {
        public long Id { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public StockMovementType Type { get; set; }

        // Positive = stock increase; negative = stock decrease.
        public decimal QuantityChange { get; set; }

        public decimal UnitCost { get; set; }

        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }

        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
