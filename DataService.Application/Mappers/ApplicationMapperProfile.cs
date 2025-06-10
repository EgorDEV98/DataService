using AutoMapper;
using DataService.Contracts.Models.Response;
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
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Candles, o => o.Ignore());

        CreateMap<Candle, CandleResponse>()
            .ForMember(d => d.Close, o => o.MapFrom(s => s.Close))
            .ForMember(d => d.Open, o => o.MapFrom(s => s.Open))
            .ForMember(d => d.Volume, o => o.MapFrom(s => s.Volume))
            .ForMember(d => d.Time, o => o.MapFrom(s => s.Time))
            .ForMember(d => d.Low, o => o.MapFrom(s => s.Low))
            .ForMember(d => d.High, o => o.MapFrom(s => s.High));

        CreateMap<Share, ShareResponse>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi))
            .ForMember(d => d.Ticker, o => o.MapFrom(s => s.Ticker))
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id));

        CreateMap<OrderDto, OrderResponse>()
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
            .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity));
        
        CreateMap<OrderBookDto, OrderBookResponse>()
            .ForMember(d =>d.Asks, o => o.MapFrom(s => s.Asks))
            .ForMember(d => d.Bids, o => o.MapFrom(s => s.Bids))
            .ForMember(d => d.Figi, o => o.MapFrom(s => s.Figi))
            .ForMember(d => d.Depth, o => o.MapFrom(s => s.Depth));
    }
}