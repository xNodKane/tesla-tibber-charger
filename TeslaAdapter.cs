using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TeslaTibberCharger.Data;

namespace TeslaTibberCharger;


public class TeslaAdapter : ITeslaAdapter
{
    private readonly HttpClient _httpClient;
    private readonly decimal? _homeLatitude;
    private readonly decimal? _homeLongitude;
    private readonly string _baseUrl = "https://owner-api.teslamotors.com";

    public TeslaAdapter(HttpClient httpClient, string accessToken)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    /// <summary>
    /// Initialize api
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="accessToken">Token from your tesla account</param>
    /// <param name="homeLatitude">provide the gps latitude of your home. So its only charging at home</param>
    /// <param name="homeLongitude">provide the gps longitude of your home. So its only charging at home</param>
    public TeslaAdapter(HttpClient httpClient, string accessToken, decimal homeLatitude, decimal homeLongitude)
    {
        _httpClient = httpClient;
        _homeLatitude = homeLatitude;
        _homeLongitude = homeLongitude;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<Vehicle> GetVehicleAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/1/vehicles");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not get vehicle infos");
        }
        var jsonString = await response.Content.ReadAsStringAsync();
        var vehicleResponse = JsonSerializer.Deserialize<VehicleResponse>(jsonString);
        return vehicleResponse!.Response.First();
    }

    public async Task<VehicleDrivingState> GetVehicleDrivingStateAsync()
    {
        var vehicle = await WakeOrGetVehicleAsync();
        var repsonse = await _httpClient.GetAsync($"{_baseUrl}/api/1/vehicles/{vehicle.Id}/");
        if (!repsonse.IsSuccessStatusCode)
        {
            throw new Exception("can not get data");
        }
        var jsonString = await repsonse.Content.ReadAsStringAsync();
        var vehicleChargingResponse = JsonSerializer.Deserialize<VehicleDataResponse>(jsonString);
        return vehicleChargingResponse!.Response.VehicleDrivingState;
    }

    private async Task<bool> IsAtHomeAsync()
    {
        if (_homeLongitude is null && _homeLatitude is null)
        {
            return true;
        }

        var state = await GetVehicleDrivingStateAsync();
        var teslaLatiude = Math.Round(state.Latitude, 2);
        var teslaLonitude = Math.Round(state.Longitude, 2);

        return teslaLatiude == _homeLatitude && teslaLonitude == _homeLongitude;
    }

    public async Task VehicleChargerPortOpenAsync()
    {
        if (await IsAtHomeAsync() == false)
        {
            return;
        }
        var vehicle = await WakeOrGetVehicleAsync();
        var message = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/command/charge_port_door_open");
        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not open charger port");
        }
    }

    public async Task VehicleChargerPortCloseAsync()
    {
        if (await IsAtHomeAsync() == false)
        {
            return;
        }
        var vehicle = await WakeOrGetVehicleAsync();
        var message = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/command/charge_port_door_close");
        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not open charger port");
        }
    }

    public async Task<int> SetChargingLimitAsync(int amps)
    {
        if (await IsAtHomeAsync() == false)
        {
            return 0;
        }
        amps = Math.Clamp(amps, 0, 16);
        var vehicle = await WakeOrGetVehicleAsync();
        Dictionary<string, object> body = new()
        {
            { "charging_amps", amps },
        };
        var requestMessage = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/command/set_charging_amps", body: body);
        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not set charging amps");
        }

        return amps;
    }

    public async Task ChargingStartAsync()
    {
        if (await IsAtHomeAsync() == false)
        {
            return;
        }
        var vehicle = await WakeOrGetVehicleAsync();
        var message = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/command/charge_start");
        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not start charging");
        }
    }

    public async Task ChargingStopAsync()
    {
        if (await IsAtHomeAsync() == false)
        {
            return;
        }
        var vehicle = await WakeOrGetVehicleAsync();
        var message = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/command/charge_stop");
        var response = await _httpClient.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("can not stop charging");
        }
    }

    private async Task<Vehicle> WakeOrGetVehicleAsync()
    {
        var vehicle = await GetVehicleAsync();
        if (vehicle.State == "asleep" || vehicle.State == "offline")
        {
            var message = BuildRequest(HttpMethod.Post, $"{_baseUrl}/api/1/vehicles/{vehicle.Id}/wake_up");
            var response = await _httpClient.SendAsync(message);
            await Task.Delay(15000);
            vehicle = await GetVehicleAsync();
        }
        return vehicle;
    }

    private static HttpRequestMessage BuildRequest(HttpMethod method, string url, Dictionary<string, string>? headers = null, Dictionary<string, object>? body = null)
    {
        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(url),
        };

        request.Headers.Add("Accept", "application/json");
        if (headers != null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (body != null)
        {
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        }

        return request;
    }
}