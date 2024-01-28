using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            //User
            CreateMap<ImportUserDto, User>();

            //Product
            CreateMap<ImportProductsDto, Product>();

            CreateMap<Product, ExportProductsDto>()
                .ForMember(d=>d.ProductName, opt=> opt.MapFrom(s=>s.Name))
                .ForMember(d=>d.ProductPrice, opt=> opt.MapFrom(s=>s.Price))
                .ForMember(d=>d.Seller , opt=> opt.MapFrom(s=> $"{s.Seller.FirstName} {s.Seller.LastName}"));

            CreateMap<ImportCategoryDto, Category>();

            CreateMap<ImportCategoriesProductsDto, CategoryProduct>();
        }
    }
}
