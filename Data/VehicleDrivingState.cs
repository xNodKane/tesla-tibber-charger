using System.Text.Json.Serialization;

namespace TeslaTibberCharger.Data;

public class VehicleDataResponse
{
    [JsonPropertyName("response")]
    public VehicleMetaData Response { get; set; } = null!;
}

public class VehicleMetaData
{
    [JsonPropertyName("drive_state")]
    public VehicleDrivingState VehicleDrivingState { get; set; } = null!;
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