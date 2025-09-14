using AutoMapper;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Categories
{
    public class GetCategoriesQueryHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper
    ) : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            var productCounts = await _categoryRepository.GetAllWithProductCountAsync(cancellationToken) ?? new Dictionary<string, int>();
            
            return categories.Select(category =>
            {
                var dto = _mapper.Map<CategoryDto>(category);
                var productCount = productCounts.GetValueOrDefault(category.Id, 0);
                return dto with { ProductCount = productCount };
            });
        }
    }
}
