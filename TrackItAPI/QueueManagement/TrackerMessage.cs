using System.Text;
using TrackItCommon;

namespace TrackItAPI.QueueManagement;

public class TrackerMessage 
{
    public string TrackerCode { get; }
    public float Latitude { get; }
    public float Longitude { get; }

    private TrackerMessage(string trackerCode, float latitude, float longitude)
    {
        TrackerCode = trackerCode;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static TrackerMessage FromMessageBytes(byte[] messageBytes)
    {
        if(messageBytes == null || messageBytes.Length < 10)
            throw new ArgumentException("The received message is invalid: message needs to be at least 10 bytes long.");
        
        var codeBuilder = new StringBuilder();
        int i;
        for(i = 0; i < messageBytes.Length && messageBytes[i] != '\0'; i++)
        {
            codeBuilder.Append((char)messageBytes[i]);
        }
        if(i >= messageBytes.Length)
            throw new ArgumentException("The received message is invalid: message is missing a null-terminated tracker code.");
        
        var trackerCode = codeBuilder.ToString();

        ++i;
        var latitude = BitConverter.ToSingle(messageBytes, i);
        i += 4;
        var longitude = BitConverter.ToSingle(messageBytes, i);

        return new TrackerMessage(trackerCode, latitude, longitude);
    }

    public Position ToPosition(DateTime collectDate)
    {
        return new Position(Latitude, Longitude, collectDate);
    }
}