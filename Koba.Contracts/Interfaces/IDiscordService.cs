using Koba.Contracts.Payloads;

namespace Koba.Contracts.Interfaces;

public interface IDiscordService
{
    public Task<GetDiscordSocketUrlResponse> GetDiscordSocketUrlAsync(CancellationToken cancellationToken);
}