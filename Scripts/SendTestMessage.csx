#r "nuget: MQTTnet, 3.1.2"
using System.Globalization;
using System.Threading;
using MQTTnet;
using MQTTnet.Client.Options;
var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .WithClientId("TrackItTest")
    .Build();
await mqttClient.ConnectAsync(options, CancellationToken.None);
string trackerCode = "tt";
var latitutde = 41.89025F;
var longitude = 12.49237F;

Console.WriteLine($"Sending coordinates {latitutde.ToString(CultureInfo.InvariantCulture)} {longitude.ToString(CultureInfo.InvariantCulture)} for tracker with code {trackerCode}");
var message = new MqttApplicationMessageBuilder()
    .WithTopic("TrackIt")
    .WithPayload(Encoding.ASCII.GetBytes(trackerCode).Concat(new byte[] { 0 }).Concat(BitConverter.GetBytes(latitutde)).Concat(BitConverter.GetBytes(longitude)).ToArray())
    .WithAtLeastOnceQoS()
    .Build();
 
await mqttClient.PublishAsync(message, CancellationToken.None);
