using System.Security.Cryptography.X509Certificates;
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

        var certificate = new X509Certificate2(_mqttConfiguration.Value.CertificatePath!, _mqttConfiguration.Value.CertificatePassword!, X509KeyStorageFlags.Exportable);

        var options = new MqttServerOptionsBuilder()
            .WithoutDefaultEndpoint()
            .WithEncryptedEndpoint()
            .WithEncryptedEndpointPort(_mqttConfiguration.Value.BrokerPort)
            .WithEncryptionCertificate(certificate)
            .WithEncryptionSslProtocol(SslProtocols.Tls12)
            .Build();

        var mqttServer = factory.CreateMqttServer();
        await mqttServer.StartAsync(options);
    }
}