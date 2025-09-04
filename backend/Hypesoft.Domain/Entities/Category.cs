using Hypesoft.Domain.Common;

namespace Hypesoft.Domain.Entities
{
    public class Category : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Navigation property
        public ICollection<Product> Products { get; private set; } = [];

        private Category() : base() { } // EF Core

        public Category(string name, string description) : base()
        {
            SetName(name);
            SetDescription(description);
        }

        public void Update(string name, string description)
        {
            SetName(name);
            SetDescription(description);
            SetUpdatedAt();
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name is required", nameof(name));
            }

            if (name.Length > 100)
            {
                throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));
            }

            Name = name.Trim();
        }

        private void SetDescription(string description)
        {
            Description = description?.Trim() ?? string.Empty;
        }
    }
}
