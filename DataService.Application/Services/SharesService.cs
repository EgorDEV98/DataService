using AutoMapper;
using DataService.Application.Models;
using DataService.Contracts.Models.Response;
using DataService.Data;
using DataService.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataService.Application.Services;

public class SharesService(PostgresDbContext context, IMapper mapper)
{
    public async Task<IReadOnlyCollection<ShareResponse>> GetSharesAsync(GetSharesParams param, CancellationToken ct)
    {
        var shares = await context.Shares
            .AsNoTracking()
            .WhereIf(param.IsEnabled.HasValue, x => x.IsEnableToLoad == param.IsEnabled)
            .ToArrayAsync(ct);
        return mapper.Map<IReadOnlyCollection<ShareResponse>>(shares);
    }
}