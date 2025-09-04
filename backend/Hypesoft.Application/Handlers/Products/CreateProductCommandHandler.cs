using AutoMapper;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class CreateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper
            ) : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Verify category exists
            bool categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
            if (!categoryExists)
            {
                throw new ArgumentException($"Category with ID {request.CategoryId} does not exist");
            }

            Money price = new(request.Price, request.Currency);
            Product product = new(
                request.Name,
                request.Description,
                price,
                request.CategoryId,
                request.StockQuantity);

            Product createdProduct = await _productRepository.CreateAsync(product, cancellationToken);
            // Get the product with category for mapping
            Product? productWithCategory = await _productRepository.GetByIdAsync(createdProduct.Id, cancellationToken);
            return _mapper.Map<ProductDto>(productWithCategory);
        }
    }
}
