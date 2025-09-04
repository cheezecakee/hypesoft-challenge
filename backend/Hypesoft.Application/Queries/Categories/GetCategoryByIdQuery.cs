using Hypesoft.Application.DTOs.Categories;
using MediatR;

namespace Hypesoft.Application.Queries.Categories
{
    public record GetCategoryByIdQuery(string Id) : IRequest<CategoryDto?>;
}
