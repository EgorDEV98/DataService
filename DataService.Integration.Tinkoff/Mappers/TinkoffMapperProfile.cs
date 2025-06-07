using AutoMapper;
using DataService.Integration.Models;
using Tinkoff.InvestApi.V1;

namespace DataService.Integration.Tinkoff.Mappers;

public class TinkoffMapperProfile : Profile
{
    public TinkoffMapperProfile()
    {
        CreateMap<Share, ShareDto>(MemberList.Destination);
    }
}