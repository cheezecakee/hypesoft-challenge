using AutoMapper;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
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
            // Normalize input: empty or whitespace -> null
            string? searchTerm = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : request.SearchTerm.Trim();
            string? categoryId = string.IsNullOrWhiteSpace(request.CategoryId) ? null : request.CategoryId.Trim();

            // Call repository
            (IEnumerable<Product> products, int totalCount) = await _productRepository.SearchAsync(
                searchTerm,
                categoryId,
                request.Page,
                request.PageSize,
                cancellationToken);

            // Map to DTOs
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
