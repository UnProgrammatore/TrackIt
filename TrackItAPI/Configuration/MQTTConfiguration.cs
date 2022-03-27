namespace TrackItAPI.Configuration;

public class MQTTConfiguration
{
    public string? BrokerAddress { get; set; }
    public ushort BrokerPort { get; set; }
    public string? BrokerTopic { get; set; }
}