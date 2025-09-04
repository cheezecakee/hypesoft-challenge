using AutoMapper;
using Hypesoft.Application.Commands.Products;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class UpdateProductStockCommandHandler(
            IProductRepository productRepository,
            IMapper mapper
            ) : IRequestHandler<UpdateProductStockCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ProductDto> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
        {
            Product product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new ArgumentException($"Product with ID {request.Id} not found");

            product.UpdateStock(request.StockQuantity);

            Product updatedProduct = await _productRepository.UpdateAsync(product, cancellationToken);
            // Get the product with category for mapping
            Product? productWithCategory = await _productRepository.GetByIdAsync(updatedProduct.Id, cancellationToken);
            return _mapper.Map<ProductDto>(productWithCategory);
        }
    }
}

