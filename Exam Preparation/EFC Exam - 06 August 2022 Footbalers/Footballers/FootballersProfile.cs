namespace Footballers
{
    using AutoMapper;
    using Footballers.Data.Models;
    using Footballers.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
    using System.Globalization;

    // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
    public class FootballersProfile : Profile
    {
        public FootballersProfile()
        {
            CreateMap<Team, ExportTeamDto>()
                .ForMember(d=>d.Footballers, opt=>opt.MapFrom(d=>d.TeamsFootballers));

            CreateMap<Coach, ExportTeamDto>();
              //  .ForMember(d=>d.FootballersCount, opt=>opt.MapFrom(s=>s.Footballers.Count()));


        }
    }
}
