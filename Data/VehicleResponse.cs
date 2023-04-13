using System.Text.Json.Serialization;

namespace TeslaTibberCharger.Data;

public class VehicleResponse
{
    [JsonPropertyName("response")]
    public List<Vehicle> Response { get; set; } = null!;
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class Vehicle
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("vehicle_id")]
    public long VehicleId { get; set; }
    [JsonPropertyName("vin")]
    public string State { get; set; } = default!;
    [JsonPropertyName("in_service")]
    public bool InService { get; set; }
    [JsonPropertyName("calendar_enabled")]
    public bool CalendarEnabled { get; set; }
    [JsonPropertyName("api_version")]
    public int ApiVersion { get; set; }
}