using AutoMapper;
using DataService.Integration.Models;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Mappers;

public class TinkoffMapperProfile : Profile
{
    public TinkoffMapperProfile()
    {
        CreateMap<Share, ShareDto>(MemberList.Destination)
            .ForMember(d => d.Ticker, o => o.MapFrom(s => s.Ticker))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi))
            .ForMember(d => d.Currency, o => o.MapFrom(s => s.Currency))
            .ForMember(d => d.ForQualInvestor, o => o.MapFrom(s => s.ForQualInvestorFlag))
            .ForMember(d => d.ClassCode, o => o.MapFrom(s => s.ClassCode))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.CountryOfRisk, o => o.MapFrom(s => s.CountryOfRisk))
            .ForMember(d => d.First1MinCandleDate, o => o.MapFrom(s => s.First1MinCandleDate.ToDateTimeOffset()))
            .ForMember(d => d.First1DayCandleDate, o => o.MapFrom(s => s.First1DayCandleDate.ToDateTimeOffset()));
    }
}