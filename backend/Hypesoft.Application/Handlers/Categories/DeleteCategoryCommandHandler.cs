using Hypesoft.Application.Commands.Categories;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Categories
{
    public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            bool categoryExists = await _categoryRepository.ExistsAsync(request.Id, cancellationToken);
            if (!categoryExists)
            {
                return false;
            }

            // Check if category has products
            bool hasProducts = await _categoryRepository.HasProductsAsync(request.Id, cancellationToken);
            if (hasProducts)
            {
                throw new InvalidOperationException($"Cannot delete category with ID {request.Id} because it has associated products");
            }

            await _categoryRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
    }
}
