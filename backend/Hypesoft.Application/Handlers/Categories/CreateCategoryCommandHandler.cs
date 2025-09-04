using AutoMapper;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Categories
{
    public class CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IMapper mapper
    ) : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if category with same name already exists
            Category? existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingCategory != null)
            {
                throw new ArgumentException($"Category with name '{request.Name}' already exists");
            }

            Category category = new(request.Name, request.Description);
            Category createdCategory = await _categoryRepository.CreateAsync(category, cancellationToken);
            return _mapper.Map<CategoryDto>(createdCategory);
        }
    }
}
