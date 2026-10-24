using Koba.Contracts.Payloads;

namespace Koba.Contracts.Dtos;

public record GatewayInitializationParams
{
    public string SocketUrl { get; set; }
    public int[] Shards { get; set; }
    public SessionStartLimit sessionStartInfo { get; set; }
    public CancellationToken cancellationToken { get; set; }
}