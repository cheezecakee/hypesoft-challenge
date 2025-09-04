using AutoMapper;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Entities;
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
            IEnumerable<Category> categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
}

