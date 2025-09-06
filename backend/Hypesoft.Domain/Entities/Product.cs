using Hypesoft.Domain.Common;
using Hypesoft.Domain.ValueObjects;

namespace Hypesoft.Domain.Entities
{
    public class Product : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public string CategoryId { get; private set; } = string.Empty;
        public int StockQuantity { get; private set; }
        public bool IsLowStock => StockQuantity < 10;

        // Navigation property
        public Category Category { get; private set; } = null!;

        private Product() : base() { } // EF Core

        public Product(string name, string description, Money price, string categoryId, int stockQuantity)
            : base()
        {
            SetName(name);
            SetDescription(description);
            SetPrice(price);
            SetCategoryId(categoryId);
            SetStockQuantity(stockQuantity);
        }

        public void Update(string name, string description, Money price, string categoryId, int stockQuantity)
        {
            SetName(name);
            SetDescription(description);
            SetPrice(price);
            SetCategoryId(categoryId);
            SetStockQuantity(stockQuantity);
            SetUpdatedAt();
        }

        public void PartialUpdate(
            string? name = null,
            string? description = null,
            Money? price = null,
            string? categoryId = null,
            int? stockQuantity = null)
        {
            bool hasChanges = false;

            if (name != null && name != Name)
            {
                SetName(name);
                hasChanges = true;
            }

            if (description != null && description != Description)
            {
                SetDescription(description);
                hasChanges = true;
            }

            if (price != null && !price.Equals(Price))
            {
                SetPrice(price);
                hasChanges = true;
            }

            if (categoryId != null && categoryId != CategoryId)
            {
                SetCategoryId(categoryId);
                hasChanges = true;
            }

            if (stockQuantity.HasValue && stockQuantity.Value != StockQuantity)
            {
                SetStockQuantity(stockQuantity.Value);
                hasChanges = true;
            }

            if (hasChanges)
            {
                SetUpdatedAt();
            }
        }

        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));
            }

            StockQuantity = quantity;
            SetUpdatedAt();
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity to add must be positive", nameof(quantity));
            }

            StockQuantity += quantity;
            SetUpdatedAt();
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity to remove must be positive", nameof(quantity));
            }

            if (StockQuantity < quantity)
            {
                throw new InvalidOperationException("Insufficient stock");
            }

            StockQuantity -= quantity;
            SetUpdatedAt();
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is required", nameof(name));
            }

            if (name.Length > 200)
            {
                throw new ArgumentException("Product name cannot exceed 200 characters", nameof(name));
            }

            Name = name.Trim();
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Product description is required", nameof(description));
            }

            Description = description.Trim();
        }

        private void SetPrice(Money price)
        {
            Price = price ?? throw new ArgumentNullException(nameof(price));
        }

        private void SetCategoryId(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                throw new ArgumentException("Category ID is required", nameof(categoryId));
            }

            CategoryId = categoryId;
        }

        private void SetStockQuantity(int stockQuantity)
        {
            if (stockQuantity < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
            }

            StockQuantity = stockQuantity;
        }
    }
}
