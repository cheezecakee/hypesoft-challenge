using AutoMapper;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class GetLowStockProductsQueryHandler(
            IProductRepository productRepository,
            IMapper mapper
            ) : IRequestHandler<GetLowStockProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _productRepository.GetLowStockProductsAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
