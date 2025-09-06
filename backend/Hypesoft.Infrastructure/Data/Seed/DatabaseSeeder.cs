using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Hypesoft.Infrastructure.Data.Seed
{
    public class DatabaseSeeder(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        ILogger<DatabaseSeeder> logger)
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ILogger<DatabaseSeeder> _logger = logger;

        public async Task SeedAsync()
        {
            try
            {
                Log.StartingSeeding(_logger);

                await SeedCategoriesAsync();
                await SeedProductsAsync();

                Log.SeedingCompleted(_logger);
            }
            catch (Exception ex)
            {
                Log.SeedingError(_logger, ex);
                throw;
            }
        }

        private async Task SeedCategoriesAsync()
        {
            // Fixed: Add explicit cast to List<Category>
            List<Category> existingCategories = (await _categoryRepository.GetAllAsync()).ToList();
            if (existingCategories.Count > 0)
            {
                Log.CategoriesExist(_logger);
                return;
            }

            List<Category> demoCategories = GetDemoCategories();
            foreach (Category category in demoCategories)
            {
                // Fixed: Store the result to avoid unused expression warning
                Category createdCategory = await _categoryRepository.CreateAsync(category);
                Log.SeededCategory(_logger, category.Name, category.Id);
            }

            Log.CategoriesSeeded(_logger, demoCategories.Count);
        }

        private async Task SeedProductsAsync()
        {
            // Fixed: Add explicit cast to List<Product>
            List<Product> existingProducts = (await _productRepository.GetAllAsync()).ToList();
            if (existingProducts.Count > 0)
            {
                Log.ProductsExist(_logger);
                return;
            }

            List<Category> categories = (await _categoryRepository.GetAllAsync()).ToList();
            if (categories.Count == 0)
            {
                Log.NoCategoriesFound(_logger);
                return;
            }

            // Fixed: Use static lambdas for better performance
            Category? electronicsCategory = categories.FirstOrDefault(static c => c.Name == "Electronics");
            Category? booksCategory = categories.FirstOrDefault(static c => c.Name == "Books");
            Category? clothingCategory = categories.FirstOrDefault(static c => c.Name == "Clothing");
            Category? homeGardenCategory = categories.FirstOrDefault(static c => c.Name == "Home & Garden");
            Category? sportsCategory = categories.FirstOrDefault(static c => c.Name == "Sports & Outdoors");

            List<Product> demoProducts = GetDemoProducts(
                electronicsCategory?.Id,
                booksCategory?.Id,
                clothingCategory?.Id,
                homeGardenCategory?.Id,
                sportsCategory?.Id
            );

            foreach (Product product in demoProducts)
            {
                // Fixed: Store the result to avoid unused expression warning
                Product createdProduct = await _productRepository.CreateAsync(product);
                Log.SeededProduct(_logger, product.Name, product.Id);
            }

            Log.ProductsSeeded(_logger, demoProducts.Count);
        }

        // Fixed: Use block body for method and simplified collection initialization
        private static List<Category> GetDemoCategories()
        {
            return
            [
                new("Electronics", "Electronic devices, gadgets, and tech accessories"),
                new("Books", "Books, educational materials, and literature"),
                new("Clothing", "Apparel, fashion items, and accessories"),
                new("Home & Garden", "Home improvement, furniture, and gardening supplies"),
                new("Sports & Outdoors", "Sporting goods, outdoor equipment, and fitness gear")
            ];
        }

        private static List<Product> GetDemoProducts(
            string? electronicsId,
            string? booksId,
            string? clothingId,
            string? homeGardenId,
            string? sportsId)
        {
            // Fixed: Simplified collection initialization
            List<Product> products = [];

            if (!string.IsNullOrEmpty(electronicsId))
            {
                // Fixed: Simplified collection initialization
                products.AddRange(
                [
                    new Product(
                        "Wireless Bluetooth Headphones",
                        "High-quality wireless headphones with active noise cancellation and 30-hour battery life",
                        new Money(129.99m, "USD"),
                        electronicsId,
                        25
                    ),
                    new Product(
                        "4K Webcam",
                        "Ultra HD webcam perfect for streaming and video conferences",
                        new Money(89.99m, "USD"),
                        electronicsId,
                        15
                    ),
                    new Product(
                        "Mechanical Gaming Keyboard",
                        "RGB backlit mechanical keyboard with customizable keys",
                        new Money(159.99m, "USD"),
                        electronicsId,
                        8
                    )
                ]);
            }

            if (!string.IsNullOrEmpty(booksId))
            {
                products.AddRange(
                [
                    new Product(
                        "Clean Code: A Handbook of Agile Software Craftsmanship",
                        "Essential reading for software developers on writing clean, maintainable code",
                        new Money(42.99m, "USD"),
                        booksId,
                        20
                    ),
                    new Product(
                        "System Design Interview",
                        "Comprehensive guide to system design interviews for software engineers",
                        new Money(38.99m, "USD"),
                        booksId,
                        12
                    )
                ]);
            }

            if (!string.IsNullOrEmpty(clothingId))
            {
                products.AddRange(
                [
                    new Product(
                        "Premium Cotton T-Shirt",
                        "Comfortable 100% organic cotton t-shirt in various colors",
                        new Money(24.99m, "USD"),
                        clothingId,
                        50
                    ),
                    new Product(
                        "Denim Jeans",
                        "Classic fit denim jeans made from sustainable materials",
                        new Money(79.99m, "USD"),
                        clothingId,
                        30
                    )
                ]);
            }

            if (!string.IsNullOrEmpty(homeGardenId))
            {
                products.AddRange(
                [
                    new Product(
                        "Smart Home Security Camera",
                        "WiFi-enabled security camera with motion detection and night vision",
                        new Money(199.99m, "USD"),
                        homeGardenId,
                        18
                    ),
                    new Product(
                        "Indoor Plant Starter Kit",
                        "Everything needed to start your indoor garden including pots, soil, and seeds",
                        new Money(34.99m, "USD"),
                        homeGardenId,
                        5
                    )
                ]);
            }

            if (!string.IsNullOrEmpty(sportsId))
            {
                products.AddRange(
                [
                    new Product(
                        "Yoga Mat Premium",
                        "Non-slip yoga mat made from eco-friendly materials",
                        new Money(49.99m, "USD"),
                        sportsId,
                        35
                    ),
                    new Product(
                        "Camping Backpack 40L",
                        "Waterproof hiking backpack with multiple compartments",
                        new Money(89.99m, "USD"),
                        sportsId,
                        12
                    )
                ]);
            }

            return products;
        }
    }

    internal static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Starting database seeding...")]
        public static partial void StartingSeeding(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Database seeding completed successfully!")]
        public static partial void SeedingCompleted(ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while seeding the database")]
        public static partial void SeedingError(ILogger logger, Exception ex);

        [LoggerMessage(Level = LogLevel.Information, Message = "Categories already exist, skipping category seeding")]
        public static partial void CategoriesExist(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Seeded category: {CategoryName} with ID: {CategoryId}")]
        public static partial void SeededCategory(ILogger logger, string categoryName, string categoryId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Seeded {CategoryCount} categories")]
        public static partial void CategoriesSeeded(ILogger logger, int categoryCount);

        [LoggerMessage(Level = LogLevel.Information, Message = "Products already exist, skipping product seeding")]
        public static partial void ProductsExist(ILogger logger);

        [LoggerMessage(Level = LogLevel.Warning, Message = "No categories found. Products cannot be seeded without categories.")]
        public static partial void NoCategoriesFound(ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, Message = "Seeded product: {ProductName} with ID: {ProductId}")]
        public static partial void SeededProduct(ILogger logger, string productName, string productId);

        [LoggerMessage(Level = LogLevel.Information, Message = "Seeded {ProductCount} products")]
        public static partial void ProductsSeeded(ILogger logger, int productCount);
    }
}
