using Hypesoft.Application.DTOs.Categories;
using MediatR;

namespace Hypesoft.Application.Commands.Categories
{
    public record UpdateCategoryCommand(
        string Id,
        string? Name = null,
        string? Description = null
    ) : IRequest<CategoryDto>;
}

