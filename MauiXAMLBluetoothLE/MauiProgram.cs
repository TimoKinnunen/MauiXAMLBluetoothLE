namespace MauiXAMLBluetoothLE;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

        // Initialise the toolkit
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa_solid.ttf", "FontAwesome");
				fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
            });

        builder.Services.AddSingleton<BluetoothLEService>();

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMap>(Map.Default);

        builder.Services.AddSingleton<HomePageViewModel>();
        builder.Services.AddSingleton<HomePage>();

        builder.Services.AddSingleton<HeartRatePageViewModel>();
        builder.Services.AddSingleton<HeartRatePage>();

        builder.Services.AddSingleton<SettingsPageViewModel>();
        builder.Services.AddSingleton<SettingsPage>();

        builder.Services.AddSingleton<InstructionsPageViewModel>();
        builder.Services.AddSingleton<InstructionsPage>();

        builder.Services.AddSingleton<AboutPageViewModel>();
        builder.Services.AddSingleton<AboutPage>();

        builder.Services.AddSingleton<PrivacyStatementPageViewModel>();
        builder.Services.AddSingleton<PrivacyStatementPage>();

        builder.Services.AddSingleton<BatteryLevelPageViewModel>();
        builder.Services.AddSingleton<BatteryLevelPage>();

        return builder.Build();
	}
}
