using System.Text.Json.Serialization;

namespace TeslaTibberCharger.Data;

public class VehicleDrivingStateResponse
{
    [JsonPropertyName("response")]
    public VehicleDrivingState Response { get; set; } = null!;
}

public class VehicleDrivingState
{
    [JsonPropertyName("shift_state")]
    public string? ShiftState { get; set; }

    [JsonPropertyName("speed")]
    public decimal? Speed { get; set; }

    [JsonPropertyName("power")]
    public decimal Power { get; set; }

    [JsonPropertyName("latitude")]
    public decimal Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public decimal Longitude { get; set; }

    [JsonPropertyName("heading")]
    public int Heading { get; set; }
}