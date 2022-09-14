namespace MauiXAMLBluetoothLE.ViewModels;

public partial class BatteryLevelPageViewModel : BaseViewModel
{
    public BluetoothLEService BluetoothLEService { get; private set; }
    public BatteryLevelPageViewModel(BluetoothLEService bluetoothLEService)
    {
        Title = $"Battery level";

        BluetoothLEService = bluetoothLEService;
    }
}
