using TeslaTibberCharger.Data;
using Tibber.Sdk;

namespace TeslaTibberCharger;

public class TeslaRealtimeTibberCharger : IObserver<RealTimeMeasurement>
{
    private readonly ITeslaAdapter _teslaAdapter;
    private readonly RealtimeChargingConfig _config;
    private readonly TimeSpan _changeUpOffsetTime = new(0, 0, 10);
    private int _lastAmps;
    private bool _isCharging = false;
    private bool _teslaApiAttempt = false;
    private int _counterToStopCharge = 0;
    private DateTime _lastChangeTime = DateTime.MinValue;

    public TeslaRealtimeTibberCharger(ITeslaAdapter teslaAdapter)
    {
        _config = new RealtimeChargingConfig();
        _teslaAdapter = teslaAdapter;
    }

    public TeslaRealtimeTibberCharger(ITeslaAdapter teslaAdapter, RealtimeChargingConfig config)
    {
        _config = config;
        _teslaAdapter = teslaAdapter;
    }

    public void OnCompleted() => Console.WriteLine("Real time measurement stream has been terminated. ");
    public void OnError(Exception error) => Console.WriteLine($"An error occured: {error.Message}");

    public void OnNext(RealTimeMeasurement value)
    {
        var solarPower = value.PowerProduction.GetValueOrDefault(0);
        Console.WriteLine($"{DateTime.Now} | Solar power {solarPower}W | Home power: {value.Power}W");

        if (value.Power > 0 && _isCharging)
        {
            if (_config.MaxAttemptsToStopCharge > _counterToStopCharge)
            {
                _counterToStopCharge++;
                Console.WriteLine($"{DateTime.Now} | Stopping charge after {_config.MaxAttemptsToStopCharge - _counterToStopCharge} attempts");
            }

            if (_counterToStopCharge > _config.AttemptsBeforeAmpsReduction)
            {
                var reducedAmps = Math.Clamp(_lastAmps - 1, 0, 16);
                SetChargingAmps(reducedAmps);
            }
        }
        else
        {
            _counterToStopCharge = 0;
        }

        if (_config.MaxAttemptsToStopCharge <= _counterToStopCharge && _isCharging && _teslaApiAttempt == false)
        {
            Console.WriteLine($"{DateTime.Now} | Stopped charging");
            Task.Factory.StartNew(async () =>
            {
                _teslaApiAttempt = true;
                await _teslaAdapter.ChargingStopAsync();
                _teslaApiAttempt = false;
                _isCharging = false;
            });
        }

        if (solarPower <= _config.MinProductionPower)
        {
            return;
        }

        if (_isCharging == false && _teslaApiAttempt == false)
        {
            Console.WriteLine($"{DateTime.Now} | Attempt to start charging");
            Task.Factory.StartNew(async () =>
            {
                _teslaApiAttempt = true;
                await _teslaAdapter.ChargingStartAsync();
                await SetChargingAmps(_teslaAdapter, 1);
                _teslaApiAttempt = false;
                _isCharging = true;
            });
        }
        if (_isCharging == false)
        {
            return;
        }

        var availableAmps = GetChargingAmps(solarPower, _lastAmps);
        SetChargingAmps(availableAmps);
    }

    private int GetChargingAmps(decimal solarPower, int lastAmps)
    {
        var totalSolarPower = lastAmps * _config.Voltage + solarPower - _config.SolarPowerOffset;
        return Math.Clamp((int)Math.Floor(totalSolarPower / _config.Voltage), 0, 16);
    }

    private void SetChargingAmps(int ampsToSet)
    {
        if (_lastAmps != ampsToSet)
        {
            if (_lastAmps < ampsToSet && _lastChangeTime.Add(_changeUpOffsetTime) > DateTime.Now)
            {
                return;
            }

            Task.Factory.StartNew(async () =>
            {
                _lastChangeTime = DateTime.Now;
                Interlocked.Exchange(ref _lastAmps, ampsToSet);
                await SetChargingAmps(_teslaAdapter, ampsToSet);
            });
        }
    }

    static Task<int> SetChargingAmps(ITeslaAdapter teslaProvider, int availableAmps)
    {
        Console.WriteLine($"{DateTime.Now} | Charging amps set to {availableAmps}A");
        return teslaProvider.SetChargingLimitAsync(availableAmps);
    }

}

