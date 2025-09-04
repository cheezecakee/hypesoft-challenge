using AutoMapper;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.Queries.Categories;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Entities;
using MediatR;

namespace Hypesoft.Application.Handlers.Categories
{
    public class GetCategoryByIdQueryHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper
    ) : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            Category? category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }
    }
}
