namespace MauiXAMLBluetoothLE.ViewModels
{
    public partial class PrivacyStatementPageViewModel : BaseViewModel
    {
        public BluetoothLEService BluetoothLEService { get; private set; }
        public PrivacyStatementPageViewModel(BluetoothLEService bluetoothLEService)
        {
            Title = $"Privacy statement";

            BluetoothLEService = bluetoothLEService;
        }
    }
}
