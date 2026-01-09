using Microsoft.Extensions.Configuration;

namespace Playwright_ReqRoll;

/// <summary>
/// Static class for accessing application configuration settings.
/// Loads configuration from appsettings.json and environment variables.
/// </summary>
public static class Config
{
    #region Constants

    /// <summary>
    /// Default viewport width in pixels.
    /// </summary>
    private const int DefaultViewportWidth = 1920;

    /// <summary>
    /// Default viewport height in pixels.
    /// </summary>
    private const int DefaultViewportHeight = 1080;

    /// <summary>
    /// Default delay in milliseconds to wait for video file writing.
    /// </summary>
    private const int DefaultVideoWriteDelayMs = 2000;

    /// <summary>
    /// Default browser type.
    /// </summary>
    private const string DefaultBrowserType = "chromium";

    #endregion

    private static readonly IConfigurationRoot Configuration;

    /// <summary>
    /// Initializes configuration from appsettings.json.
    /// </summary>
    static Config()
    {
        var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configFilePath)!)
            .AddJsonFile(Path.GetFileName(configFilePath), true, true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    #region Browser Settings

    /// <summary>
    /// Gets the browser type for Playwright (chromium, firefox, webkit).
    /// </summary>
    /// <value>Browser type. Default: "chromium".</value>
    public static string BrowserType => Configuration["Browser:Type"] ?? DefaultBrowserType;

    /// <summary>
    /// Gets whether the browser should run in headless mode.
    /// </summary>
    /// <value>True for headless, false for visual mode. Default: false.</value>
    public static bool Headless => bool.TryParse(Configuration["Browser:Headless"], out var result) && result;

    /// <summary>
    /// Gets the delay in milliseconds between browser actions (slow motion).
    /// </summary>
    /// <value>Delay in ms. Default: 0.</value>
    public static int SlowMo => int.TryParse(Configuration["Browser:SlowMo"], out var result) ? result : 0;

    #endregion

    #region Emulation Settings

    /// <summary>
    /// Gets the device type for emulation (Desktop, iPhone 13, Pixel 5, etc.).
    /// Use "Desktop" for desktop browser or a Playwright device name for mobile emulation.
    /// </summary>
    /// <value>Device name. Default: "Desktop".</value>
    public static string Device => Configuration["Emulation:Device"] ?? "Desktop";

    /// <summary>
    /// Gets whether mobile emulation is enabled.
    /// Automatically inferred from Device - true if Device is not "Desktop".
    /// </summary>
    /// <value>True for mobile, false for desktop.</value>
    public static bool IsMobile => !Device.Equals("Desktop", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the viewport width for browser emulation.
    /// </summary>
    /// <value>Width in pixels. Default: 1920.</value>
    public static int ViewportWidth => int.TryParse(Configuration["Emulation:Viewport:Width"], out var result) ? result : DefaultViewportWidth;

    /// <summary>
    /// Gets the viewport height for browser emulation.
    /// </summary>
    /// <value>Height in pixels. Default: 1080.</value>
    public static int ViewportHeight => int.TryParse(Configuration["Emulation:Viewport:Height"], out var result) ? result : DefaultViewportHeight;

    /// <summary>
    /// Gets the locale for emulation (e.g., "pt-BR", "en-US").
    /// </summary>
    /// <value>Locale string. Default: "pt-BR".</value>
    public static string Locale => Configuration["Emulation:Locale"] ?? "pt-BR";

    /// <summary>
    /// Gets the timezone ID for emulation (e.g., "America/Sao_Paulo").
    /// </summary>
    /// <value>Timezone ID. Default: "America/Sao_Paulo".</value>
    public static string TimezoneId => Configuration["Emulation:TimezoneId"] ?? "America/Sao_Paulo";

    /// <summary>
    /// Gets whether geolocation emulation is enabled.
    /// </summary>
    /// <value>True to enable geolocation, false otherwise. Default: false.</value>
    public static bool GeolocationEnabled => bool.TryParse(Configuration["Emulation:Geolocation:Enabled"], out var result) && result;

    /// <summary>
    /// Gets the geolocation latitude for emulation.
    /// </summary>
    /// <value>Latitude coordinate. Default: -23.5505 (São Paulo).</value>
    public static double GeolocationLatitude => double.TryParse(Configuration["Emulation:Geolocation:Latitude"], out var result) ? result : -23.5505;

    /// <summary>
    /// Gets the geolocation longitude for emulation.
    /// </summary>
    /// <value>Longitude coordinate. Default: -46.6333 (São Paulo).</value>
    public static double GeolocationLongitude => double.TryParse(Configuration["Emulation:Geolocation:Longitude"], out var result) ? result : -46.6333;

    /// <summary>
    /// Gets the color scheme for emulation (light, dark, or no-preference).
    /// </summary>
    /// <value>Color scheme. Default: "light".</value>
    public static string ColorSchemeString => Configuration["Emulation:ColorScheme"] ?? "light";

    #endregion

    #region Tracing Settings

    /// <summary>
    /// Gets whether to save traces for passed tests.
    /// </summary>
    /// <value>True to save, false to skip. Default: false.</value>
    public static bool SaveTracesOnPass => !bool.TryParse(Configuration["Tracing:SaveOnPass"], out var result) || result;

    /// <summary>
    /// Gets the relative directory where traces are saved.
    /// </summary>
    /// <value>Directory name. Default: "Playwright-Traces".</value>
    private static string TracesPath => Configuration["Tracing:TracesPath"] ?? "Playwright-Traces";

    /// <summary>
    /// Gets the full path to the traces directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and TracesPath.</value>
    public static string FullTracesPath => Path.Combine(ReportsPath, TracesPath);

    #endregion

    #region Video Settings

    /// <summary>
    /// Gets whether to record videos for scenarios.
    /// </summary>
    /// <value>True to record, false to skip. Default: false.</value>
    public static bool RecordVideo => bool.TryParse(Configuration["Video:Record"], out var result) && result;

    /// <summary>
    /// Gets the relative directory where videos are saved.
    /// </summary>
    /// <value>Directory name. Default: "Playwright-Videos".</value>
    private static string VideoDir => Configuration["Video:Dir"] ?? "Playwright-Videos";

    /// <summary>
    /// Gets the full path to the videos directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and VideoDir.</value>
    public static string FullVideoPath => Path.Combine(ReportsPath, VideoDir);

    /// <summary>
    /// Gets the delay in ms to wait for video file writing.
    /// </summary>
    /// <value>Delay in ms. Default: 2000.</value>
    public static int VideoWriteDelayMs => int.TryParse(Configuration["Video:WriteDelayMs"], out var result) ? result : DefaultVideoWriteDelayMs;

    #endregion

    #region Screenshot Settings

    /// <summary>
    /// Gets whether to capture screenshots on successful tests.
    /// </summary>
    /// <value>True to capture, false to skip. Default: false.</value>
    public static bool ScreenshotOnSuccess => bool.TryParse(Configuration["Screenshots:OnSuccess"], out var result) && result;

    /// <summary>
    /// Gets whether to capture screenshots on failed tests.
    /// </summary>
    /// <value>True to capture, false to skip. Default: true.</value>
    public static bool ScreenshotOnFailure => !bool.TryParse(Configuration["Screenshots:OnFailure"], out var result) || result;

    /// <summary>
    /// Gets the relative directory where screenshots are saved.
    /// </summary>
    /// <value>Directory name. Default: "Screenshots".</value>
    private static string ScreenshotsDir => Configuration["Screenshots:Dir"] ?? "Screenshots";

    /// <summary>
    /// Gets the full path to the screenshots directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and ScreenshotsDir.</value>
    public static string FullScreenshotsPath => Path.Combine(ReportsPath, ScreenshotsDir);

    #endregion

    #region Reporting Settings

    /// <summary>
    /// Gets the base path for saving reports (traces, videos, screenshots).
    /// </summary>
    /// <value>Directory path. Default: current execution directory.</value>
    private static string ReportsPath => Configuration["Reports:Path"] ?? Directory.GetCurrentDirectory();

    /// <summary>
    /// Gets the directory where browser downloads are saved.
    /// </summary>
    /// <value>Directory path. Default: "Downloads".</value>
    private static string DownloadsPath => Configuration["Downloads:Path"] ?? "Downloads";

    /// <summary>
    /// Gets the full path to the downloads directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and DownloadsPath.</value>
    public static string FullDownloadsPath => Path.Combine(ReportsPath, DownloadsPath);

    #endregion
}