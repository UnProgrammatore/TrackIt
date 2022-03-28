using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using TrackItAPI.Configuration;

namespace TrackItAPI.QueueManagement;

public class MQTTServer
{
    private readonly IOptions<MQTTConfiguration> _mqttConfiguration;

    public MQTTServer(IOptions<MQTTConfiguration> mqttConfiguration)
    {
        _mqttConfiguration = mqttConfiguration;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new MqttFactory();

        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(_mqttConfiguration.Value.BrokerPort)
            .Build();

        var mqttServer = factory.CreateMqttServer();
        await mqttServer.StartAsync(options);
    }
}