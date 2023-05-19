namespace TeslaTibberCharger.Data;

public interface ITeslaAdapter
{
    Task<Vehicle> GetVehicleAsync();
    Task<VehicleMetaData> GetVehicleDataAsync();
    Task ChargingStartAsync();
    Task ChargingStopAsync();
    Task<int> SetChargingLimitAsync(int amps);
}