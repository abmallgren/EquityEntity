using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace EntityEquity.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Property>? Properties { get; set; }
        public DbSet<PropertyManager>? PropertyManagers { get; set; }
        public DbSet<Offering>? Offerings { get; set; }
        public DbSet<OfferingManager>? OfferingManagers { get; set; }
        public DbSet<PropertyOfferingMapping>? PropertyOfferingMappings { get; set; }
        public DbSet<Inventory>? Inventories { get; set; }
        public DbSet<InventoryManager>? InventoryManagers { get; set; }
        public DbSet<InventoryItem>? InventoryItems { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderItem>? OrderItems { get; set; }
        public DbSet<Invoice>? Invoices { get; set; }
        public DbSet<InvoiceItem>? InvoiceItems { get; set; }
        public DbSet<EquityOffer>? EquityOffers { get; set; }
        public DbSet<EquityTransaction>? EquityTransactions { get; set; }
        public DbSet<PhotoUrl>? PhotoUrls { get; set; }
        public DbSet<OfferingPhotoUrlMapping>? OfferingPhotoUrlMappings { get; set; }
        public DbSet<PaymentTransaction>? PaymentTransactions { get; set; }
        public DbSet<PaymentTransactionError>? PaymentTransactionErrors { get; set; }
        public DbSet<BillingAddress>? BillingAddresses { get; set; }
        public DbSet<ShippingAddress>? ShippingAddresses { get; set; }
        public DbSet<LedgerEntry>? LedgerEntries { get; set; }
        public DbSet<PendingTransaction>? PendingTransactions { get; set; }
        public DbSet<Temporary>? Temporaries { get; set; }
        public DbSet<Email>? Emails { get; set; }
        public DbSet<AppSetting>? AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquityTransaction>(et =>
            {
                et.HasOne("EntityEquity.Data.Property", "Property")
                        .WithMany()
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                et.HasOne("EntityEquity.Data.EquityOffer", "EquityOffer")
                        .WithMany()
                        .HasForeignKey("EquityOfferId")
                        .OnDelete(DeleteBehavior.NoAction);

                et.Navigation("Property");
            });
            modelBuilder.Entity<Inventory>(i =>
            {
                i.Property(ie => ie.CreatedAt).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<PaymentTransaction>(pt =>
            {
                pt.Property(ptp => ptp.OccurredAt).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<PaymentTransactionError>(pte =>
            {
                pte.Property(ptep => ptep.OccurredAt).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<LedgerEntry>(le =>
            {
                le.Property(lep => lep.OccurredAt).HasDefaultValueSql("getutcdate()");
            });

            modelBuilder.Entity<PendingTransaction>(t =>
            {
                t.Property(tp => tp.OccurredAt).HasDefaultValueSql("getutcdate()");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
    public class AppSetting
    {
        public int AppSettingId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [Serializable]
    public class Property
    {
        public Property()
        {
            Name = "";
            Slug = "";
        }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string NotificationEmailAddress { get; set; }
        public string Slug { get; set; }
        public int Shares { get; set; }
        public bool AllowEquityOffers { get; set; }
        public bool ShowPublicInsights { get; set; }
        public string OwnerUserId { get; set; }
        public bool Deactivated { get; set; }
    }
    [Serializable]
    public class PropertyManager
    {
        public PropertyManager()
        {
            Property = new();
        }
        public int PropertyManagerId { get; set; }
        public Property Property { get; set; }
        public string? UserId { get; set; }
        public PropertyManagerRoles Role { get; set; }
    }
    [Serializable]
    public enum PropertyManagerRoles { Administrator }
    [Serializable]
    public class Offering
    {
        public Offering()
        {
            Name = "";
            Slug = "";
            Description = "";
            InventoryItem = new();
        }
        public int OfferingId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExtendedDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool MustShip { get; set; }
        public bool MustPayWithECheck { get; set; }
        public bool Deactivated { get; set; }
    }
    [Serializable]
    public class OfferingManager
    {
        public OfferingManager()
        {
            Offering = new();
        }
        public int OfferingManagerId { get; set; }
        public Offering Offering { get; set; }
        public string? UserId { get; set; }
        public OfferingManagerRoles Role { get; set; }
    }
    [Serializable]
    public class PhotoUrl
    {
        public PhotoUrl()
        {
            Url = "";
        }
        public int PhotoUrlId { get; set; }
        public string Url { get; set; }
    }
    [Serializable]
    public class OfferingPhotoUrlMapping
    {
        public OfferingPhotoUrlMapping()
        {
            Offering = new();
            PhotoUrl = new();
        }
        public int OfferingPhotoUrlMappingId { get; set; }
        public Offering Offering { get; set; }
        public PhotoUrl PhotoUrl { get; set; }
    }
    [Serializable]
    public class PropertyOfferingMapping
    {
        public int PropertyOfferingMappingId { get; set; }
        public Property? Property { get; set; }
        public Offering? Offering { get; set; }
    }
    [Serializable]
    public enum OfferingManagerRoles { Administrator }
    [Serializable]
    public class Inventory
    {
        public int InventoryId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deactivated { get; set; }
    }
    [Serializable]
    public class InventoryItem
    {
        public InventoryItem()
        {
            Inventory = new();
            Name = "";
            SKU = "";
        }
        public int InventoryItemId { get; set; }
        public Inventory Inventory { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        public bool Deactivated { get; set; }
    }
    [Serializable]
    public class InventoryManager
    {
        public InventoryManager()
        {
            UserId = "";
            Inventory = new();
        }
        public int InventoryManagerId { get; set; }
        public string UserId { get; set; }
        public Inventory Inventory { get; set; }
        public InventoryManagerRoles Role { get; set; }
    }
    [Serializable]
    public enum InventoryManagerRoles { Administrator };

    public class Order
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public OrderState State { get; set; }
        public BillingAddress? BillingAddress { get; set; }
        public ShippingAddress? ShippingAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
    public enum OrderState { Incomplete, Complete }
    public enum PaymentMethod { CreditCard, eCheck }
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public Order? Order { get; set; }
        public Property? Property { get; set; }
        public Offering? Offering { get; set; }
        public int Quantity { get; set; }
    }
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string UserId { get; set; }
        public Property? Property { get; set; }
        public Order? Order { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public Invoice? Invoice { get; set; }
        public string? Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class EquityOffer
    {
        public int EquityOfferId { get; set; }
        public Property Property { get; set; }
        public string UserId { get; set; }
        public int Shares { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public bool MustPurchaseAll { get; set; }
    }
    public class EquityTransaction
    {
        public int EquityTransactionId { get; set; }
        public EquityOffer? EquityOffer { get; set; }
        public Property Property { get; set; }
        public string SellerUserId { get; set; }
        public string BuyerUserId { get; set; }
        public int Shares { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
    public class PaymentTransaction
    {
        public int PaymentTransactionId { get; set; }
        public Order Order { get; set; }
        public string TransactionId { get; set; }
        public string ResponseCode { get; set; }
        public string MessageCode { get; set; }
        public string Description { get; set; }
        public string AuthorizationCode { get; set; }
        public DateTime OccurredAt { get; set; }
    }
    public class PaymentTransactionError
    {
        public int PaymentTransactionErrorId { get; set; }
        public Order Order { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime OccurredAt { get; set; }
    }
    public class BillingAddress
    {
        public int BillingAddressId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
    public class ShippingAddress
    {
        public int ShippingAddressId { get; set; }
        public string UserId { get; set; }
        public bool SameAsBillingAddress { get; set; }
        public string? Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
    }
    public class LedgerEntry
    {
        public int LedgerEntryId { get; set; }
        public string UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime OccurredAt { get; set; }
    }
    public class PendingTransaction
    {
        public int PendingTransactionId { get; set; }
        public string OriginatorId { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public bool AddedToLedger { get; set; }
        public DateTime OccurredAt { get; set; }
    }
    public enum TransactionStatus { Complete, InProgress, Denied }
    public class Temporary
    {
        public int TemporaryId { get; set; }
        public string Content { get; set; }
    }
    public class Email
    {
        public int EmailId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}