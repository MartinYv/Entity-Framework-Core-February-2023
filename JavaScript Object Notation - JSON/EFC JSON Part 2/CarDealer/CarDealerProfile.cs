﻿using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {

            CreateMap<ImportSuppliersDto, Supplier>();

            CreateMap<ImportPartsDto, Part>();

            CreateMap<ImportCarsDto, Car>();

            CreateMap<ImportCustomersDto, Customer>();

            CreateMap<ImportSalesDto, Sale>();
        }
    }
}
