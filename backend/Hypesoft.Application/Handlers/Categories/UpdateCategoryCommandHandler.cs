using AutoMapper;
using Hypesoft.Application.Commands.Categories;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Entities;
using MediatR;

namespace Hypesoft.Application.Handlers.Categories
{
    public class UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper
            ) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            Category? category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new ArgumentException($"Category with ID {request.Id} not found");

            // Check if another category with same name already exists
            Category? existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingCategory != null && existingCategory.Id != request.Id)
            {
                throw new ArgumentException($"Category with name '{request.Name}' already exists");
            }

            category.Update(request.Name, request.Description);
            Category updatedCategory = await _categoryRepository.UpdateAsync(category, cancellationToken);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }
    }
}
