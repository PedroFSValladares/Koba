using Koba.Contracts.Payloads;

namespace Koba.Contracts.Dtos;

public record GatewayInitializationParams
{
    public SessionStartLimit sessionStartInfo { get; set; }
    public CancellationToken cancellationToken { get; set; }
}