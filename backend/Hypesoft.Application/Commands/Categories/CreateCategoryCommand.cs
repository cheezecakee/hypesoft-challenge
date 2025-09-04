using Hypesoft.Application.DTOs.Categories;
using MediatR;

namespace Hypesoft.Application.Commands.Categories
{
    public record CreateCategoryCommand(
        string Name,
        string Description
    ) : IRequest<CategoryDto>;
}
