using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
           
            CreateMap< ImportSupplierDto, Supplier >();
            CreateMap<ImportPartDto, Part>();

            CreateMap<ImportCarDto, Car>().ForSourceMember(s=>s.Parts, opt=>opt.DoNotValidate());

            CreateMap<ImportCustomerDto, Customer>();

            CreateMap<Car, ExportCarDto>();
            CreateMap<Car, ExportBmwCarsDto>();

            CreateMap<Supplier, ExportLocalSupplierDto>()
                .ForMember(d => d.PartsCount, opt => opt.MapFrom(s => s.Parts.Count()));

            CreateMap<PartCar, ExportCarAndItsPartsDto>();


            CreateMap<Part, ExportCarPartsDto>();
            CreateMap<Car, ExportCarAndItsPartsDto>()
                .ForMember(d=>d.Parts, opt=>opt.MapFrom(s=>s.PartsCars.Select(pc=>pc.Part).OrderByDescending(p=>p.Price)));

           

            CreateMap<Customer, ExportSalesByCustomerDto>()
                .ForMember(d=>d.CountOfCars, opt=>opt.MapFrom(s=>s.Sales.Count))
                .ForMember(d=>d.CustomerName, opt=>opt.MapFrom(s=>s.Name));

            CreateMap<Part, ExportSalesByCustomerDto>()
                .ForMember(d => d.MoneySpent, opt => opt.MapFrom(s=>s.PartsCars.Sum(x=>x.Part.Price)));

            CreateMap<Sale, ExportSalesByCustomerDto>()
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer.Name))
                .ForMember(d => d.CountOfCars, opt => opt.MapFrom(s => s.Customer.Sales.Count))
                .ForMember(d => d.MoneySpent, opt => opt.MapFrom(s => s.Car.PartsCars.Sum(cp => cp.Part.Price)));


           
        }

      
    }
}
