using AutoMapper;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Entities;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class SearchProductsQueryHandler(
        IProductRepository productRepository,
        IMapper mapper) : IRequestHandler<SearchProductsQuery, PagedProductsDto>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedProductsDto> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            (IEnumerable<Product> products, int totalCount) = await _productRepository.SearchAsync(
                request.SearchTerm,
                request.CategoryId,
                request.Page,
                request.PageSize,
                cancellationToken);

            IEnumerable<ProductDto> productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PagedProductsDto(
                productDtos,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages);
        }
    }
}

