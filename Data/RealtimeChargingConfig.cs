namespace TeslaTibberCharger.Data;

public class RealtimeChargingConfig
{
    /// <summary>
    /// Voltage of the power grid.
    /// Default=230V
    /// </summary>
    public decimal Voltage { get; set; } = 230m;
    /// <summary>
    /// Production power offset for calculating the charging amps.
    /// Default=25W
    /// </summary>
    public decimal SolarPowerOffset { get; set; } = 25m;
    /// <summary>
    /// Aleast this power is required to start charging.
    /// Default=230W
    /// </summary>
    public decimal MinProductionPower { get; set; } = 230m;
    /// <summary>
    /// After how many atempts the charging should be set to last amps-1 when the home power is > 0. 
    /// Default=1
    /// </summary>
    public int AttemptsBeforeAmpsReduction { get; set; } = 1;
    /// <summary>
    /// After how many atempts the charging should be stopped when the home power is > 0. 
    /// Default=20
    /// </summary>
    public int MaxAttemptsToStopCharge { get; set; } = 20;
}
