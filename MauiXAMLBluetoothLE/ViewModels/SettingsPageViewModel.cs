namespace MauiXAMLBluetoothLE.ViewModels
{
    public partial class SettingsPageViewModel : BaseViewModel
    {
        public BluetoothLEService BluetoothLEService { get; private set; }
        public SettingsPageViewModel(BluetoothLEService bluetoothLEService)
        {
            Title = $"Settings";

            BluetoothLEService = bluetoothLEService;
        }
    }
}
