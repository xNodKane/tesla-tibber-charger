using TeslaTibberCharger.Data;
using Tibber.Sdk;

namespace TeslaTibberCharger;

public class TibberAdapter : ITibberAdapter
{
    private readonly TibberApiClient _tibberClient;

    public TibberAdapter(string token)
    {
        _tibberClient = new TibberApiClient(token);
    }

    public async Task<IObservable<RealTimeMeasurement>> ListenToCurrentHomerPowerAsync()
    {
        var tibberHomes = await _tibberClient.GetHomes();
        var home = tibberHomes.Data.Viewer.Homes.First();
        return await _tibberClient.StartRealTimeMeasurementListener(home.Id!.Value);
    }

    public async Task StopListeningAsync()
    {
        var tibberHomes = await _tibberClient.GetHomes();
        var home = tibberHomes.Data.Viewer.Homes.First();
        await _tibberClient.StopRealTimeMeasurementListener(home.Id!.Value);
    }
}