namespace MauiXAMLBluetoothLE.Pages;

public partial class BatteryLevelPage : ContentPage
{
    public BluetoothLEService BluetoothLEService { get; private set; }

    public IService BatteryLevelService { get; private set; }
    public ICharacteristic BatteryLevelCharacteristic { get; private set; }


    public BatteryLevelPage(BatteryLevelPageViewModel viewModel, BluetoothLEService bluetoothLEService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        BluetoothLEService = bluetoothLEService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BluetoothLEService.Device != null)
        {
            if (BluetoothLEService.Device.State == DeviceState.Connected)
            {
                await ReadBatteryLevelAsync();
            }
        }
        else
        {
            DevicenameLabel.Text = $"Device is disconnected.";
            BatteryLevelLabel.Text = $"Battery level is 0 %";
            TimestampLabel.Text = $"Battery level at {DateTimeOffset.MinValue:H:mm:ss zzz}";
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    public async Task ReadBatteryLevelAsync()
    {
        try
        {
            BatteryLevelService = await BluetoothLEService.Device.GetServiceAsync(HeartRateUuids.BatteryLevelServiceUuid);
            if (BatteryLevelService != null)
            {
                BatteryLevelCharacteristic = await BatteryLevelService.GetCharacteristicAsync(HeartRateUuids.BatteryLevelCharacteristicUuid);
                if (BatteryLevelCharacteristic != null)
                {
                    if (BatteryLevelCharacteristic.CanRead)
                    {
                        DevicenameLabel.Text = $"{BluetoothLEService.Device.Name}";

                        byte[] data = await BatteryLevelCharacteristic.ReadAsync();
                        ushort batteryLevel = data[0];
                        BatteryLevelLabel.Text = $"Battery level is {batteryLevel} %";

                        var timestamp = DateTimeOffset.Now.LocalDateTime;
                        TimestampLabel.Text = $"Battery level at {timestamp:H:mm:ss zzz}";
                    }
                }
            }
        }
        catch
        {
        }
    }
}