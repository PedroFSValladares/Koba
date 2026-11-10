using System.Text.Json;
using Koba.Contracts;
using Koba.Contracts.Dtos;
using Koba.Contracts.Events;
using Koba.Contracts.Interfaces;
using Koba.Infrastructure.Gateway;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Tests;

public class GatewayTests
{
    
    private readonly ITestOutputHelper _output;

    public GatewayTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    
}