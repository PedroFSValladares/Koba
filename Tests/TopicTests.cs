using System.Threading.Channels;
using Koba.Infrastructure.Services.Messaging;
using Xunit.Abstractions;

namespace Tests;

public class TopicTests
{
    
    private readonly ITestOutputHelper _output;

    public TopicTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact(DisplayName = "Testa adição de um consumidor, se não falhar deve obter sucesso")]
    public void TestaAdicaçoDeUmConsumidor_DeveObterSucesso()
    {
        Topic<string> topic = new Topic<string>(new TestLogger<Topic<string>>(_output));
        
        topic.AddConsumer(x => Task.CompletedTask);
    }
    
    [Fact(DisplayName = "Testa adição de dois consumidorres, deve falhar na segunda adição")]
    public void TestaAdicaçoDeDoisConsumidores_DeveFalharNaSegundaAdicao()
    {
        Topic<string> topic = new Topic<string>(new TestLogger<Topic<string>>(_output));
        
        topic.AddConsumer(x => Task.CompletedTask);
        Exception e = Assert.Throws<InvalidOperationException>(() => topic.AddConsumer(x => Task.CompletedTask));
        Assert.Equal("Já existe um consumidor para esse tópico", e.Message);
    }
    
    [Fact(DisplayName = "Testa adição de mensagens sem consumidor, deve obter sucesso")]
    public async Task TestaAdicaoDeMensagens_DeveObterSucesso()
    {
        Topic<string> topic = new Topic<string>(new TestLogger<Topic<string>>(_output));
        bool messageReceived = false;
        
        await topic.AddMessageAsync("Hello World!", CancellationToken.None);
        topic.AddConsumer(x =>
        {
            messageReceived = true;
            return Task.CompletedTask;
        });
        await Task.Delay(TimeSpan.FromSeconds(1));
        Assert.True(messageReceived);
    }
    
    [Fact(DisplayName = "Testa adição de mensagens sem em canal fechado, deve falhar")]
    public async Task TestaAdicaoDeMensagensEmCanalFechado_DeveFalhar()
    {
        Topic<string> topic = new Topic<string>(new TestLogger<Topic<string>>(_output));
        await topic.DisposeAsync();
        await Assert.ThrowsAsync<ChannelClosedException>(async () => await topic.AddMessageAsync("Hello World!", CancellationToken.None));
    }

    [Fact(DisplayName = "Testa realizar multiplos encerramentos, deve obter sucesso")]
    public async Task TestaMultiposEncerramento_DeveObterSucesso()
    {
        Topic<string> topic = new Topic<string>(new TestLogger<Topic<string>>(_output));

        try
        {
            await topic.DisposeAsync();
            await topic.DisposeAsync();
        }
        catch (Exception e)
        {
            Assert.Fail("Uma das chamadas de cancelamento não obteve sucesso.");
        }
    }
}