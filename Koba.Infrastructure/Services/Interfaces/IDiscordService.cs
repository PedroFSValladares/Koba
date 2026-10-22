using Koba.Infrastructure.Services.Dtos;

namespace Koba.Infrastructure.Services.Interfaces;

public interface IDiscordService
{
    public Task<GetDiscordSocketUrlResponse> GetDiscordSocketUrlAsync(CancellationToken cancellationToken);
}