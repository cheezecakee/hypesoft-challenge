using AutoMapper;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Application.Queries.Products;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class GetProductsQueryHandler(
            IProductRepository productRepository,
            IMapper mapper
            ) : IRequestHandler<GetProductsQuery, (IEnumerable<ProductDto> Products, int TotalCount)>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(request.SearchName))
            {
                IEnumerable<Product> searchResults = await _productRepository.SearchByNameAsync(request.SearchName, cancellationToken);
                products = searchResults.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                totalCount = searchResults.Count();
            }
            else if (!string.IsNullOrWhiteSpace(request.CategoryId))
            {
                IEnumerable<Product> categoryResults = await _productRepository.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
                products = categoryResults.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                totalCount = categoryResults.Count();
            }
            else
            {
                (IEnumerable<Product> pagedProducts, int pagedTotalCount) =
                    await _productRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
                products = pagedProducts;
                totalCount = pagedTotalCount;
            }

            IEnumerable<ProductDto> productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return (productDtos, totalCount);
        }
    }
}

