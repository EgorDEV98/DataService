using AutoMapper;
using DataService.Integration.Models;
using DataService.Integration.Tinkoff.Extensions;
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

        CreateMap<HistoricCandle, CandleDto>(MemberList.Destination)
            .ForMember(d => d.Close, o => o.MapFrom(s => s.Close))
            .ForMember(d => d.Open, o => o.MapFrom(s => s.Open))
            .ForMember(d => d.Volume, o => o.MapFrom(s => s.Volume))
            .ForMember(d => d.High, o => o.MapFrom(s => s.High))
            .ForMember(d => d.Low, o => o.MapFrom(s => s.Low))
            .ForMember(d => d.Time, o => o.MapFrom(s => s.Time.ToDateTimeOffset()))
            .ForMember(d => d.Figi, o => o.Ignore());
        
        CreateMap<Candle, CandleDto>(MemberList.Destination)
            .ForMember(d => d.Close, o => o.MapFrom(s => s.Close))
            .ForMember(d => d.Open, o => o.MapFrom(s => s.Open))
            .ForMember(d => d.Volume, o => o.MapFrom(s => s.Volume))
            .ForMember(d => d.High, o => o.MapFrom(s => s.High))
            .ForMember(d => d.Low, o => o.MapFrom(s => s.Low))
            .ForMember(d => d.Time, o => o.MapFrom(s => s.Time.ToDateTimeOffset()))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
            .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));
        
        CreateMap<GetOrderBookResponse, OrderBookDto>()
            .ForMember(d => d.Asks, o => o.MapFrom(s => s.Asks))
            .ForMember(d => d.Bids, o => o.MapFrom(s => s.Bids))
            .ForMember(d => d.Depth, o => o.MapFrom(s => s.Depth))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi));
    }
}