# Tesla Tibber Charger

This package will offer you an option to charge your car if enough solar power is available.
We will use the TibberSDK to get your current home power usage trough Tibber Pulse. 
Tibber Pulse is installed on your energy meter.
If we detect a negative power consumption above 'MinProductionPower' in W we will try to start charging if your cable is conneted to the car and your are at your home position.
If the connection to tibber fails we will reconnect the realtime subscription.

## Usage
``` csharp
var httpClient = new HttpClient();
var teslaAccessToken = "";
var homeLatitude = "";
var homeLongitude = "";
var tibberToken = "";
var teslaProvider = new TeslaAdapter(httpClient, teslaAccessToken, homeLatitude, homeLongitude);
var tibberRrovider = new TibberAdapter(tibberToken);
var curentHomePower = await tibberRrovider.ListenToCurrentHomerPowerAsync();
var teslaObserver = new TeslaRealtimeTibberCharger(teslaProvider);
teslaObserver.OnConnectionClosed += TeslaObserver_OnConnectionIssue;

curentHomePower.Subscribe(teslaObserver);

void TeslaObserver_OnConnectionIssue()
{
    curentHomePower = tibberProvider.ListenToCurrentHomerPowerAsync().Result;
    curentHomePower.Subscribe(teslaObserver);
}
```

## Contribution
Feel free to help to make it better