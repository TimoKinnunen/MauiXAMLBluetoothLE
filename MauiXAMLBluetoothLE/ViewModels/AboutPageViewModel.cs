namespace MauiXAMLBluetoothLE.ViewModels;

public partial class AboutPageViewModel : BaseViewModel
{
    public BluetoothLEService BluetoothLEService { get; private set; }
    public AboutPageViewModel(BluetoothLEService bluetoothLEService)
    {
        Title = $"About MauiXAMLBluetoothLE";

        BluetoothLEService = bluetoothLEService;

        Name = AppInfo.Current.Name;
        Version = AppInfo.Current.VersionString;
        Build = AppInfo.Current.BuildString;
    }

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string version;

    [ObservableProperty]
    string build;

    //string name = AppInfo.Current.Name;
    //string package = AppInfo.Current.PackageName;
    //string version = AppInfo.Current.VersionString;
    //string build = AppInfo.Current.BuildString;
}
