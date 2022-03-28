using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using TrackItAPI.Configuration;
using TrackItAPI.Database.Repositories;

namespace TrackItAPI.QueueManagement;

public class QueueManager : IMqttApplicationMessageReceivedHandler
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IOptions<MQTTConfiguration> _mqttConfiguration;
    private readonly ILogger<QueueManager> _logger;
    public QueueManager(ITrackingRepository trackingRepository, IOptions<MQTTConfiguration> mqttConfiguration, ILogger<QueueManager> logger)
    {
        _trackingRepository = trackingRepository;
        _mqttConfiguration = mqttConfiguration;
        _logger = logger;
    }

    async Task IMqttApplicationMessageReceivedHandler.HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        var msg = eventArgs.ApplicationMessage;
        var msgContent = msg.Payload;
        TrackerMessage tm;
        try 
        {
            tm = TrackerMessage.FromMessageBytes(msgContent);
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "Received invalid message", msg);
            return;
        }

        var position = tm.ToPosition(DateTime.UtcNow);

        try 
        {
            await _trackingRepository.AddPositionAsync(tm.TrackerCode, position);
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "Failed to add position", position);
        }
        await eventArgs.AcknowledgeAsync(CancellationToken.None);
    }
    
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new MqttFactory();

        var clientOptions = new MqttClientOptionsBuilder()
            .WithClientId("TrackItServer")
            .WithTcpServer(_mqttConfiguration.Value.BrokerAddress, _mqttConfiguration.Value.BrokerPort)
            .Build();

        
        var client = factory.CreateMqttClient();

        client.ApplicationMessageReceivedHandler = this;

        await client.ConnectAsync(clientOptions, cancellationToken);

        var subscriberOptions = new MqttClientSubscribeOptions();
        subscriberOptions.TopicFilters.Add(
            new MqttTopicFilterBuilder()
                .WithTopic(_mqttConfiguration.Value.BrokerTopic)
                .Build()
        );
        
        await client.SubscribeAsync(subscriberOptions, cancellationToken);
    }
}