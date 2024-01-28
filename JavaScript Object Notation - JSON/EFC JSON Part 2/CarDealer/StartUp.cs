using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Migrations;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext dbContext = new CarDealerContext();

            //string inputJson = File.ReadAllText(@"../../../Datasets/customers.Json");

            Console.WriteLine(GetSalesWithAppliedDiscount(dbContext));

        }

        // 09. Import Suppliers

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }
            ));


            ImportSuppliersDto[] suppliersDto = JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(suppliersDto);

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliersDto.Length}.";
        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            ImportPartsDto[] partsDto = JsonConvert.DeserializeObject<ImportPartsDto[]>(inputJson);

            var validParts = new HashSet<Part>();

            foreach (var part in partsDto)
            {

                if (!context.Suppliers.Any(s => s.Id == part.SupplierId))
                {
                    continue;
                }

                Part validPart = mapper.Map<Part>(part);
                validParts.Add(validPart);
            }


            context.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        // 11. Import Cars

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            var carAndPartsDto = JsonConvert.DeserializeObject<ImportCarsDto[]>(inputJson);


            foreach (var carAndParts in carAndPartsDto)
            {
                Car car = new Car
                {
                    Make = carAndParts.Make,
                    Model = carAndParts.Model,
                    TravelledDistance = carAndParts.TravelledDistance
                };
                context.Cars.AddRange(car);

                foreach (var part in carAndParts.PartId.Distinct())
                {
                    PartCar parts = new PartCar
                    {
                        Car = car,
                        PartId = part
                    };

                    context.PartsCars.AddRange(parts);
                }
            };

            context.SaveChanges();

            return $"Successfully imported {carAndPartsDto.Length}.";
        }

        // 12. Import Customers

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            ImportCustomersDto[] customersDtos = JsonConvert.DeserializeObject<ImportCustomersDto[]>(inputJson);

            Customer[] customers = mapper.Map<Customer[]>(customersDtos);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customersDtos.Length}.";
        }

        // 13. Import Sales

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            ImportSalesDto[] salesDtos = JsonConvert.DeserializeObject<ImportSalesDto[]>(inputJson);
            Sale[] sales = mapper.Map<Sale[]>(salesDtos);


            context.AddRange(sales);
            context.SaveChanges();


            return $"Successfully imported {sales.Length}.";
        }

        // 14. Export Ordered Customers

        public static string GetOrderedCustomers(CarDealerContext context)
        {

            var customers = context.Customers.OrderBy(c => c.BirthDate).ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                }).AsNoTracking().ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);

        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsWithMakeToyota = context.Cars.Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    TraveledDistance = c.TravelledDistance // грешка в условието 
                })
                .OrderBy(c => c.Model).ThenByDescending(c => c.TraveledDistance)
                .AsNoTracking().ToArray();


            return JsonConvert.SerializeObject(carsWithMakeToyota, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {


            var localSuppliers = context.Suppliers.Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                });

            return JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            var cars = context.Cars.Select(c => new
            {
                car = new
                {
                    c.Make,
                    c.Model,
                    TraveledDistance = c.TravelledDistance
                }
              ,
                parts = c.PartsCars.Select(p => new
                {
                    p.Part.Name,
                    Price = p.Part.Price.ToString("f2")
                })
            }).ToArray();


            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }


        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            return "asd"; 
        }

        // Query 19. Export Sales with Applied Discount

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var cars = context.Sales.Take(10).Select(c => new
            {
                car = new
                {
                    c.Car.Make,
                    c.Car.Model,
                    TraveledDistance = c.Car.TravelledDistance
                }
              ,

                customerName = c.Customer.Name,
                discount = c.Discount.ToString(),
                price = c.Car.PartsCars.Sum(p => p.Part.Price).ToString("f2"),
                priceWithDiscount = (c.Car.PartsCars.Sum(p => p.Part.Price) - c.Car.PartsCars.Sum(p => p.Part.Price) *  c.Discount / 100).ToString("f2")
            }).ToArray();


            return JsonConvert.SerializeObject(cars, Formatting.Indented);
            }
    }
}