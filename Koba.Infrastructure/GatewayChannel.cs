using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace Koba.Infrastructure;

public class GatewayChannel
{
    public ChannelReader<string> gatewayOutput { get; private set; }
    public ChannelWriter<string> gatewayInput { get; private set; }

    public static GatewayChannel Create(ChannelWriter<string> gatewayInput, ChannelReader<string> gatewayOutput)
    {
        GatewayChannel channel = new GatewayChannel();
        
        channel.gatewayInput = gatewayInput;
        channel.gatewayOutput = gatewayOutput;
        
        return channel;
    }
        
    
}