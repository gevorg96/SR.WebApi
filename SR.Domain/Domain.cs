using System;
using System.Collections.Generic;

namespace SR.Domain
{
    public abstract class Identity
    {
        public long Id { get; set; }
    }

    public abstract class IdentityName : Identity
    {
        public string Name { get; set; } = default!;
    }
    
    public class User : Identity
    {
        public string Login { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string? MiddleName { get; set; }
        public string Email { get; set; } = default!;
    }

    public class Business: IdentityName
    {
        public string Telephone { get; set; } = default!;

        public IEnumerable<Shop> Shops { get; set; } = new List<Shop>();
    }

    public class Shop: IdentityName
    {
        public string Address { get; set; } = default!;
        public long BusinessId { get; set; }
    }

    public class Supplier : IdentityName
    {
        public string Organization { get; set; } = default!;
        public string? Address { get; set; }
        public string? Telephone { get; set; }
    }

    public class UnitOfMeasure : IdentityName { }

    public class Expense : Identity
    {
        public DateTimeOffset ReportDate { get; set; }
        public decimal Amount { get; set; }
        public long BusinessId { get; set; }
        public long ShopId { get; set; }
        
        public Business Business { get; set; } = default!;
        public Shop Shop { get; set; } = default!;

        public IEnumerable<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
    }
    
    public class ExpenseType: IdentityName {}

    public class ExpenseItem : Identity
    {
        public decimal Amount { get; set; }
        public long ExpenseId { get; set; }
        public long ExpenseTypeId { get; set; }
        
        public ExpenseType ExpenseType { get; set; } = default!;
    }

    public class Folder : IdentityName
    {
        public Folder Parent { get; set; } = default!;
        public Business Business { get; set; } = default!;
    }

    public class Product : IdentityName
    {
        public long BusinessId { get; set; }
        public long? SupplierId { get; set; }
        
        public Business Business { get; set; } = default!;
        public Supplier? Supplier { get; set; }

        public IEnumerable<ProductProperty> ProductProperties { get; set; } = new List<ProductProperty>();
    }

    public class ProductProperty : Identity
    {
        public long ProductId { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public long UoMId { get; set; }
        public long? FolderId { get; set; }
        
        public UnitOfMeasure UoM { get; set; } = default!;
        public Folder? Folder { get; set; }
    }

    public class Image
    {
        public Guid Uid { get; set; }
        public string Type { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string TempUrl { get; set; } = default!;
        public string Path { get; set; } = default!;
    }

    public class Order : Identity
    {
        public DateTimeOffset ReportDate { get; set; }
        public Shop Shop { get; set; } = default!;
        public bool IsOrder { get; set; }
    }

    public class OrderItem : Identity
    {
        public decimal Count { get; set; }
        public decimal Cost { get; set; }
        public Order Order { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }

    public class Bill : Identity
    {
        public long BillNumber { get; set; }
        public DateTimeOffset ReportDate { get; set; }
        public decimal Amount { get; set; }
        public Shop Shop { get; set; } = default!;
    }

    public class BillItem : Identity
    {
        public decimal Amount { get; set; }
        public decimal Count { get; set; }
        public decimal? Profit { get; set; }
        public decimal Price { get; set; }
        public Bill Bill { get; set; } = default!;
        public UnitOfMeasure UoM { get; set; } = default!;
        public OrderItem OrderItem { get; set; } = default!;
    }

    public class Stock : Identity
    {
        public decimal Count { get; set; }
        public Product Product { get; set; } = default!;
        public Shop Shop { get; set; } = default!;
    }

    public class OrderStock : Identity
    {
        public decimal CurrentStock { get; set; }
        public OrderItem OrderItem { get; set; } = default!;
        public Shop Shop { get; set; } = default!;
        public Product Product { get; set; } = default!;
    }
}