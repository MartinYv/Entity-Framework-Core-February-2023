using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //string inputXml = File.ReadAllText("../../../Datasets/Sales.xml");

            Console.WriteLine(GetTotalSalesByCustomer(context));
        }



        // Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));



            XmlRootAttribute root = new XmlRootAttribute("Suppliers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), root);

            StringReader reader = new StringReader(inputXml);

            ImportSupplierDto[] supplierDtos = (ImportSupplierDto[]) xmlSerializer.Deserialize(reader);


            var validSuppliers = new HashSet<Supplier>();
            foreach (var supp in supplierDtos)
            {
                if (string.IsNullOrEmpty(supp.Name))
                {
                    continue;
                }

               var supplier =  mapper.Map<Supplier>(supp);

                validSuppliers.Add(supplier);
            }

            context.AddRange(validSuppliers);
            context.SaveChanges();


            return $"Successfully imported {validSuppliers.Count}";


        }

        // 10. Import Parts

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            XmlRootAttribute root = new XmlRootAttribute("Parts");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), root);

            StringReader reader = new StringReader(inputXml);

            ImportPartDto[] partsDto = (ImportPartDto[])xmlSerializer.Deserialize(reader);

            var validParts = new HashSet<Part>();

            foreach (var part in partsDto)
            {
                if (!context.Suppliers.Any(s=>s.Id == part.SupplierId))
                {
                    continue;
                }

                Part validPart = mapper.Map<Part>(part);
                validParts.Add(validPart);
            }

         
            context.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        // 11. Import Cars

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("Cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), root);

            StringReader reader = new StringReader(inputXml);

            ImportCarDto[] carDtos = (ImportCarDto[])xmlSerializer.Deserialize(reader);

            var validCars = new HashSet<Car>();
            foreach (var carDto in carDtos)
            {
                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };
                
                validCars.Add(car);

                foreach (var part in carDto.Parts.DistinctBy(p=>p.PartId))
                {
                    PartCar partCar = new PartCar
                    {
                        Car = car,
                        PartId = part.PartId
                    };

                    context.AddRange(partCar);
                }
            }

            context.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count()}";
        }

        // 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            XmlRootAttribute root = new XmlRootAttribute("Customers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), root);

            StringReader reader = new StringReader(inputXml);

            ImportCustomerDto[] customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(reader);


            var validCustomers = new HashSet<Customer>();
            foreach (var cDto in customerDtos)
            {
                if (string.IsNullOrEmpty(cDto.Name))
                {
                    continue;
                }

                Customer customer = mapper.Map<Customer>(cDto);
                validCustomers.Add(customer);
            }

            context.AddRange(validCustomers);
            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}";
        }

        // 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute root = new XmlRootAttribute("Sales");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), root);

            StringReader reader = new StringReader(inputXml);

            ImportSalesDto[] salesDtos = (ImportSalesDto[])xmlSerializer.Deserialize(reader);

            var validSales = new HashSet<Sale>();
            foreach (var saleDto in salesDtos)
            {
                if (!context.Cars.Any(c=>c.Id == saleDto.CarId))
                {
                    continue;
                }

                Sale sale = new Sale()
                {
                    CarId = saleDto.CarId,
                    CustomerId = saleDto.CustomerId,
                    Discount = saleDto.Discount
                };

                validSales.Add(sale);
            }

            context.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}";
        }

        // Query 14. Export Cars With Distance

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder output = new StringBuilder();
            using StringWriter writer = new StringWriter(output);

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            var cars = context.Cars.Where(c => c.TraveledDistance > 2_000_000)
                .OrderBy(c => c.Make).ThenBy(c => c.Model).Take(10)
                .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider).ToArray();

            XmlRootAttribute root = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarDto[]), root);


            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);


            xmlSerializer.Serialize(writer, cars, namespaces);


            return output.ToString().TrimEnd();
        }

        // Query 15. Export Cars from Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);


            ExportBmwCarsDto[] bmwCars = context.Cars.Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model).ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportBmwCarsDto>(mapper.ConfigurationProvider)
                .ToArray();


            XmlRootAttribute root = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBmwCarsDto[]), root);


            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, bmwCars, namespaces);


            return output.ToString().TrimEnd();
        }

        // 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            var localSuppliersDtos = context.Suppliers.Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSupplierDto>(mapper.ConfigurationProvider)
                .ToArray();

            XmlRootAttribute root = new XmlRootAttribute("suppliers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportLocalSupplierDto[]), root);


            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, localSuppliersDtos, namespaces);

            return output.ToString().TrimEnd();
        }


        // 17. Export Cars With Their List Of Parts

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);


            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            var carAndItsParts = context.Cars
                .OrderByDescending(c=>c.TraveledDistance).ThenBy(x=>x.Model).Take(5)
                .ProjectTo<ExportCarAndItsPartsDto>(mapper.ConfigurationProvider).ToArray();

            XmlRootAttribute root = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarAndItsPartsDto[]), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(writer, carAndItsParts, namespaces);

            return output.ToString().TrimEnd();
        }


        // Query 18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));


            var carAndItsParts = context.Cars 
                .ProjectTo<ExportSalesByCustomerDto>(mapper.ConfigurationProvider).ToArray();



            return "asd";
        }
    }
}