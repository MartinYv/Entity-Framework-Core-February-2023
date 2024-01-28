using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Security.Cryptography.X509Certificates;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.Json");

            Console.WriteLine(GetProductsInRange(context));

        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cnf =>
            {
                cnf.AddProfile<ProductShopProfile>();
            }
            ));

            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new HashSet<User>();

            foreach (ImportUserDto userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);

            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }


        // 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }
            ));


            ImportProductsDto[] productsDto = JsonConvert.DeserializeObject<ImportProductsDto[]>(inputJson);

            Product[] products = mapper.Map<Product[]>(productsDto);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";

        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }
            ));


            ImportCategoryDto[] categoriesDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var currCategDto in categoriesDtos)
            {
                if (currCategDto.Name == null)
                {
                    continue;
                }

                Category category = mapper.Map<Category>(currCategDto);
                validCategories.Add(category);
            }



            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }
            ));


            ImportCategoriesProductsDto[] categoryAndProductsDto =
                JsonConvert.DeserializeObject<ImportCategoriesProductsDto[]>(inputJson);


            var validEntities = new HashSet<CategoryProduct>();
            foreach (var ctdto in categoryAndProductsDto)
            {
                if (!context.Products.Any(x => x.Id == ctdto.ProductId ||
                      !context.Categories.Any(x => x.Id == ctdto.CategoryId)))
                {
                    continue;
                }

                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(ctdto);
                validEntities.Add(categoryProduct);
            }


            context.CategoriesProducts.AddRange(validEntities);
            context.SaveChanges();

            return $"Successfully imported {validEntities.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));



         //var products = context.Products.Where(x => x.Price >= 500 && x.Price <= 1000)
         //    .OrderBy(pr => pr.Price)
         //    .Select(p => new
         //    {
         //        name = p.Name,
         //        price = p.Price,
         //        seller = $"{p.Seller.FirstName} + {p.Seller.LastName}"
         //    }).AsNoTracking()
         //    .ToArray();

            //return JsonConvert.SerializeObject(products, Formatting.Indented);

            ExportProductsDto[] products = context.Products.Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ProjectTo<ExportProductsDto>(mapper.ConfigurationProvider)
                .ToArray();
           
            return JsonConvert.SerializeObject(products, Formatting.Indented);

        }

        // 6. Export Sold Products

        public static string GetSoldProducts(ProductShopContext context)
        {

            var usersOrdered = context.Users.Where(u => u.ProductsSold.Any(b => b.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(p => p.Buyer != null).Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName

                    })
                    .ToArray()

                }).OrderBy(u => u.lastName).ThenBy(u => u.firstName)
                .AsNoTracking()
                .ToArray();


            return JsonConvert.SerializeObject(usersOrdered, Formatting.Indented);
        }

        // 07. Export Categories By Products Count

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {

            var categorieProducts = context.Categories.OrderByDescending(c => c.CategoriesProducts.Count).Select(c => new
            {
                category = c.Name,
                productsCount = c.CategoriesProducts.Count,
                averagePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("f2"),
                totalRevenue = Math.Round(c.CategoriesProducts.Sum(p => p.Product.Price), 2).ToString("f2"),
            })
            .AsNoTracking()
            .ToArray();

            return JsonConvert.SerializeObject(categorieProducts, Formatting.Indented);

        }

        // 08. Export Users and Products

        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var usersWithProducts = context
                           .Users
                           .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                           .OrderByDescending(u => u.ProductsSold.Where(p => p.Buyer != null).Count())
                           .Select(u => new

                           {
                               firstName = u.FirstName,
                               lastName = u.LastName,
                               age = u.Age,
                               soldProducts = new

                               {
                                   count = u.ProductsSold.Count(p => p.Buyer != null),
                                   products = u.ProductsSold.Where(p => p.Buyer != null)
                                             .Select(p => new

                                             {
                                                 name = p.Name,
                                                 price = p.Price

                                             })
                               }
                           }).AsNoTracking().ToArray();

            var usersDto = new
            {
                usersCount = usersWithProducts.Count(),
                users = usersWithProducts
            };

            return JsonConvert.SerializeObject(usersDto, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
    }
}