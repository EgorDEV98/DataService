using AutoMapper;
using DataService.Data.Entities;
using DataService.Integration.Models;

namespace DataService.Application.Mappers;

public class ApplicationMapperProfile : Profile
{
    public ApplicationMapperProfile()
    {
        CreateMap<ShareDto, Share>(MemberList.Destination)
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi))
            .ForMember(d => d.First1DayCandleDate, o => o.MapFrom(s => s.First1DayCandleDate))
            .ForMember(d => d.First1MinCandleDate, o => o.MapFrom(s => s.First1MinCandleDate))
            .ForMember(d => d.Ticker, o => o.MapFrom(s => s.Ticker))
            .ForMember(d => d.IsEnableToLoad, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore());
    }
}