using System.Threading.Channels;

namespace Koba.Infrastructure.Socket;

public class GatewayChannel
{
    private ChannelReader<string> gatewayOutput { get; set; }
    private ChannelWriter<string> gatewayInput { get; set; }

    public static GatewayChannel Create(ChannelWriter<string> gatewayInput, ChannelReader<string> gatewayOutput)
    {
        GatewayChannel channel = new GatewayChannel();
        
        channel.gatewayInput = gatewayInput;
        channel.gatewayOutput = gatewayOutput;
        
        return channel;
    }
    
    public async Task<string> GetMessageAsync() => await gatewayOutput.ReadAsync();
    
    public async Task<string> GetMessageAsync(CancellationToken token) => await gatewayOutput.ReadAsync(token);
    

    public async Task<IEnumerable<string>> GetAllMessagesAsync()
    {
        List<string> messages = [];
        await foreach (var message in gatewayOutput.ReadAllAsync())
        {
            messages.Add(message);
        }
        return messages;
    }
    
    public async Task<IEnumerable<string>> GetAllMessagesAsync(CancellationToken token)
    {
        List<string> messages = [];
        await foreach (var message in gatewayOutput.ReadAllAsync(token))
        {
            messages.Add(message);
        }
        return messages;
    }
    
    public async Task SendMessageAsync(string message) => await gatewayInput.WriteAsync(message);
    
    public async Task SendMessageAsync(string message, CancellationToken token) => await gatewayInput.WriteAsync(message, token);
    
        
    public void Complete()
    {
        gatewayInput.Complete();
        gatewayInput.Complete();
    }
}