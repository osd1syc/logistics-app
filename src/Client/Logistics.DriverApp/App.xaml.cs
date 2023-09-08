﻿using IdentityModel.OidcClient;
using Logistics.DriverApp.Services;
using Logistics.DriverApp.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Syncfusion.Licensing;

namespace Logistics.DriverApp;

public partial class App
{
    public App()
    {
        InitializeComponent();

        var configuration = BuildConfiguration();
        Services = ConfigureServices(configuration);
        MainPage = new AppShell();
    }

    public new static App Current => (App)Application.Current!;
    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices(IConfiguration configuration)
    {
        SyncfusionLicenseProvider.RegisterLicense(configuration.GetValue<string>("SyncfusionKey"));
        var services = new ServiceCollection();
        var oidcOptions = configuration.GetSection("OidcClient").Get<OidcClientOptions>() 
                          ?? throw new NullReferenceException("Could not get OidcClient form the appsettings.json file");
        services.AddSingleton(oidcOptions);

        services.AddTransient<AppShellViewModel>();
        services.AddTransient<ActiveLoadsPageViewModel>();
        services.AddTransient<AccountPageViewModel>();
        services.AddTransient<LoginPageViewModel>();
        services.AddTransient<ChangeOrganizationPageVideModel>();
        services.AddScoped<IdentityModel.OidcClient.Browser.IBrowser, WebBrowserAuthenticator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenStorage, TokenStorage>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddSingleton<IMapsService, GoogleMapsService>();
        services.AddWebApiClient(configuration);
        return services.BuildServiceProvider();
    }

    private static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder();

        TryAddConfig(builder, "appsettings.json");
#if !DEBUG
        TryAddConfig(builder, "appsettings.secrets.json");
#endif
        return builder.Build();
    }

    private static void TryAddConfig(IConfigurationBuilder builder, string configFile)
    {
        try
        {
            var config = typeof(App).Assembly.GetManifestResourceStream($"Logistics.DriverApp.{configFile}");

            if (config == null)
                return;
            
            builder.AddJsonStream(config);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
