namespace TeslaTibberCharger.Data;

public interface ITeslaAdapter
{
    Task<Vehicle> GetVehicleAsync();
    Task<VehicleDrivingState> GetVehicleDrivingStateAsync();
    Task VehicleChargerPortOpenAsync();
    Task VehicleChargerPortCloseAsync();
    Task ChargingStartAsync();
    Task ChargingStopAsync();
    Task<int> SetChargingLimitAsync(int amps);
}