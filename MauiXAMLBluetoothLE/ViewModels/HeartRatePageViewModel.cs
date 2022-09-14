namespace MauiXAMLBluetoothLE.ViewModels;

public partial class HeartRatePageViewModel : BaseViewModel
{
    public BluetoothLEService BluetoothLEService { get; private set; }

    public IAsyncRelayCommand ConnectToDeviceCandidateAsyncCommand { get; }
    public IAsyncRelayCommand DisconnectFromDeviceAsyncCommand { get; }

    public IService HeartRateService { get; private set; }
    public ICharacteristic HeartRateMeasurementCharacteristic { get; private set; }
    public HeartRatePageViewModel(BluetoothLEService bluetoothLEService)
    {
        Title = $"Heart rate";

        BluetoothLEService = bluetoothLEService;

        ConnectToDeviceCandidateAsyncCommand = new AsyncRelayCommand(ConnectToDeviceCandidateAsync);

        DisconnectFromDeviceAsyncCommand = new AsyncRelayCommand(DisconnectFromDeviceAsync);
    }

    [ObservableProperty]
    ushort heartRateValue;

    [ObservableProperty]
    DateTimeOffset timestamp;

    private async Task ConnectToDeviceCandidateAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (BluetoothLEService.NewDeviceCandidateFromMainPage.Id.Equals(Guid.Empty))
        {
            await BluetoothLEService.ShowToastAsync($"Select a Bluetooth LE device first. Try again.");
            return;
        }

        if (!BluetoothLEService.BluetoothLE.IsOn)
        {
            await Shell.Current.DisplayAlert($"Bluetooth is not on", $"Please turn Bluetooth on and try again.", "OK");
            return;
        }

        if (BluetoothLEService.Adapter.IsScanning)
        {
            await BluetoothLEService.ShowToastAsync($"Bluetooth adapter is scanning. Try again.");
            return;
        }

        try
        {
            IsBusy = true;

            if (BluetoothLEService.Device != null)
            {
                if (BluetoothLEService.Device.State == DeviceState.Connected)
                {
                    if (BluetoothLEService.Device.Id.Equals(BluetoothLEService.NewDeviceCandidateFromMainPage.Id))
                    {
                        await BluetoothLEService.ShowToastAsync($"{BluetoothLEService.Device.Name} is already connected.");
                        return;
                    }

                    if (BluetoothLEService.NewDeviceCandidateFromMainPage != null)
                    {
                        #region another device
                        if (!BluetoothLEService.Device.Id.Equals(BluetoothLEService.NewDeviceCandidateFromMainPage.Id))
                        {
                            Title = $"{BluetoothLEService.NewDeviceCandidateFromMainPage.Name}";
                            await DisconnectFromDeviceAsync();
                            await BluetoothLEService.ShowToastAsync($"{BluetoothLEService.Device.Name} has been disconnected.");
                        }
                        #endregion another device
                    }
                }
            }

            BluetoothLEService.Device = await BluetoothLEService.Adapter.ConnectToKnownDeviceAsync(BluetoothLEService.NewDeviceCandidateFromMainPage.Id);

            if (BluetoothLEService.Device.State == DeviceState.Connected)
            {
                HeartRateService = await BluetoothLEService.Device.GetServiceAsync(HeartRateUuids.HeartRateServiceUuid);
                if (HeartRateService != null)
                {
                    HeartRateMeasurementCharacteristic = await HeartRateService.GetCharacteristicAsync(HeartRateUuids.HeartRateMeasurementCharacteristicUuid);
                    if (HeartRateMeasurementCharacteristic != null)
                    {
                        if (HeartRateMeasurementCharacteristic.CanUpdate)
                        {
                            Title = $"{BluetoothLEService.Device.Name}";

                            HeartRateMeasurementCharacteristic.ValueUpdated += HeartRateMeasurementCharacteristic_ValueUpdated;
                            await HeartRateMeasurementCharacteristic.StartUpdatesAsync();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to connect to {BluetoothLEService.NewDeviceCandidateFromMainPage.Name} {BluetoothLEService.NewDeviceCandidateFromMainPage.Id}: {ex.Message}.");
            await Shell.Current.DisplayAlert($"{BluetoothLEService.NewDeviceCandidateFromMainPage.Name}", $"Unable to connect to {BluetoothLEService.NewDeviceCandidateFromMainPage.Name}.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void HeartRateMeasurementCharacteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
    {
        var bytes = e.Characteristic.Value;
        const byte heartRateValueFormat = 0x01;

        byte flags = bytes[0];
        bool isHeartRateValueSizeLong = (flags & heartRateValueFormat) != 0;
        HeartRateValue = isHeartRateValueSizeLong ? BitConverter.ToUInt16(bytes, 1) : bytes[1];
        Timestamp = DateTimeOffset.Now.LocalDateTime;
    }

    private async Task DisconnectFromDeviceAsync()
    {
        if (IsBusy)
        {
            return;
        }

        if (BluetoothLEService.Device == null)
        {
            await BluetoothLEService.ShowToastAsync($"Nothing to do.");
            return;
        }

        if (!BluetoothLEService.BluetoothLE.IsOn)
        {
            await Shell.Current.DisplayAlert($"Bluetooth is not on", $"Please turn Bluetooth on and try again.", "OK");
            return;
        }

        if (BluetoothLEService.Adapter.IsScanning)
        {
            await BluetoothLEService.ShowToastAsync($"Bluetooth adapter is scanning. Try again.");
            return;
        }

        if (BluetoothLEService.Device.State == DeviceState.Disconnected)
        {
            await BluetoothLEService.ShowToastAsync($"{BluetoothLEService.Device.Name} is already disconnected.");
            return;
        }

        try
        {
            IsBusy = true;

            await HeartRateMeasurementCharacteristic.StopUpdatesAsync();

            await BluetoothLEService.Adapter.DisconnectDeviceAsync(BluetoothLEService.Device);

            HeartRateMeasurementCharacteristic.ValueUpdated -= HeartRateMeasurementCharacteristic_ValueUpdated;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to disconnect from {BluetoothLEService.Device.Name} {BluetoothLEService.Device.Id}: {ex.Message}.");
            await Shell.Current.DisplayAlert($"{BluetoothLEService.Device.Name}", $"Unable to disconnect from {BluetoothLEService.Device.Name}.", "OK");
        }
        finally
        {
            Title = "Heart rate";
            HeartRateValue = 0;
            Timestamp = DateTimeOffset.MinValue;
            IsBusy = false;
            BluetoothLEService.Device?.Dispose();
            BluetoothLEService.Device = null;
            await Shell.Current.GoToAsync("//HomePage", true);
        }
    }
}
