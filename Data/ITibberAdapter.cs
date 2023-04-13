using Tibber.Sdk;

namespace TeslaTibberCharger.Data;

public interface ITibberAdapter
{
    Task<IObservable<RealTimeMeasurement>> ListenToCurrentHomerPowerAsync();
    Task StopListeningAsync();
}