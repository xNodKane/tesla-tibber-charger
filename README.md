# Tesla Tibber Charger

This package will offer you an option to charge your car if enough solar power is available

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
curentHomePower.Subscribe(teslaObserver);
```

### Please contribute to improving this experience
