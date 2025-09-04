using AutoMapper;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class GetProductByIdQueryHandler(
            IProductRepository productRepository,
            IMapper mapper
            ) : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            Product? product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }
    }
}
