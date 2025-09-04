using AutoMapper;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.ValueObjects;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class UpdateProductCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper) : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new ArgumentException($"Product with ID {request.Id} not found");

            // Verify category exists
            bool categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
            if (!categoryExists)
            {
                throw new ArgumentException($"Category with ID {request.CategoryId} does not exist");
            }

            Money price = new(request.Price, request.Currency);
            product.Update(request.Name, request.Description, price, request.CategoryId);

            Product updatedProduct = await _productRepository.UpdateAsync(product, cancellationToken);
            // Get the product with category for mapping
            Product? productWithCategory = await _productRepository.GetByIdAsync(updatedProduct.Id, cancellationToken);
            return _mapper.Map<ProductDto>(productWithCategory);
        }
    }
}

