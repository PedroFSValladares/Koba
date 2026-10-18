using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Tests;

public class TestLogger<T> : ILogger<T>
{
    
    private readonly ITestOutputHelper _output;
    
    public TestLogger(ITestOutputHelper output)
    {
        _output = output;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            _output.WriteLine($"[{logLevel}]: {formatter(state, exception)}");
        }
        catch (InvalidOperationException)
        {
            
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
}