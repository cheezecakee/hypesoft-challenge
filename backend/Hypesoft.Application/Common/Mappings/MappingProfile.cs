using AutoMapper;
using Hypesoft.Application.DTOs.Categories;
using Hypesoft.Application.DTOs.Products;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Can be ignored normal for AutoMapper
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Price, static opt => opt.MapFrom(static src => src.Price.Amount))
                .ForMember(dest => dest.Currency, static opt => opt.MapFrom(static src => src.Price.Currency))
                .ForMember(dest => dest.CategoryName, static opt => opt.MapFrom(static src => src.Category.Name));

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, static opt => opt.MapFrom(static src => src.Products.Count));
        }
    }

}

