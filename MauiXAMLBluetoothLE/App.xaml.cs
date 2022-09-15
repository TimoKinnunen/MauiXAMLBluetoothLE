namespace MauiXAMLBluetoothLE;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override void OnSleep()
	{
		base.OnSleep();
	}

	protected override void OnResume()
	{
		base.OnResume();
	}
}
