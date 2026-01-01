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
    public const int DefaultViewportWidth = 1920;

    /// <summary>
    /// Default viewport height in pixels.
    /// </summary>
    public const int DefaultViewportHeight = 1080;

    /// <summary>
    /// Default delay in milliseconds to wait for video file writing.
    /// </summary>
    public const int DefaultVideoWriteDelayMs = 2000;

    /// <summary>
    /// Default browser type.
    /// </summary>
    public const string DefaultBrowserType = "chromium";

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
    public static bool Headless => ParseBool(Configuration["Browser:Headless"], false);

    /// <summary>
    /// Gets the delay in milliseconds between browser actions (slow motion).
    /// </summary>
    /// <value>Delay in ms. Default: 0.</value>
    public static int SlowMo => ParseInt(Configuration["Browser:SlowMo"], 0);

    #endregion

    #region Base URLs

    /// <summary>
    /// Gets the login page URL.
    /// </summary>
    /// <value>Login page URL. Default: "https://example.com/login".</value>
    public static string LoginPageUrl => Configuration["BaseUrls:LoginPage"] ?? "https://example.com/login";

    #endregion

    #region Tracing Settings

    /// <summary>
    /// Gets whether to save traces for passed tests.
    /// </summary>
    /// <value>True to save, false to skip. Default: false.</value>
    public static bool SaveTracesOnPass => ParseBool(Configuration["Tracing:SaveOnPass"], false);

    /// <summary>
    /// Gets the relative directory where traces are saved.
    /// </summary>
    /// <value>Directory name. Default: "Playwright-Traces".</value>
    public static string TracesPath => Configuration["Tracing:TracesPath"] ?? "Playwright-Traces";

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
    public static bool RecordVideo => ParseBool(Configuration["Video:Record"], false);

    /// <summary>
    /// Gets the relative directory where videos are saved.
    /// </summary>
    /// <value>Directory name. Default: "Playwright-Videos".</value>
    public static string VideoDir => Configuration["Video:Dir"] ?? "Playwright-Videos";

    /// <summary>
    /// Gets the full path to the videos directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and VideoDir.</value>
    public static string FullVideoPath => Path.Combine(ReportsPath, VideoDir);

    /// <summary>
    /// Gets the delay in ms to wait for video file writing.
    /// </summary>
    /// <value>Delay in ms. Default: 2000.</value>
    public static int VideoWriteDelayMs => ParseInt(Configuration["Video:WriteDelayMs"], DefaultVideoWriteDelayMs);

    #endregion

    #region Screenshot Settings

    /// <summary>
    /// Gets whether to capture screenshots on successful tests.
    /// </summary>
    /// <value>True to capture, false to skip. Default: false.</value>
    public static bool ScreenshotOnSuccess => ParseBool(Configuration["Screenshots:OnSuccess"], false);

    /// <summary>
    /// Gets whether to capture screenshots on failed tests.
    /// </summary>
    /// <value>True to capture, false to skip. Default: true.</value>
    public static bool ScreenshotOnFailure => ParseBool(Configuration["Screenshots:OnFailure"], true);

    /// <summary>
    /// Gets the relative directory where screenshots are saved.
    /// </summary>
    /// <value>Directory name. Default: "Screenshots".</value>
    public static string ScreenshotsDir => Configuration["Screenshots:Dir"] ?? "Screenshots";

    /// <summary>
    /// Gets the full path to the screenshots directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and ScreenshotsDir.</value>
    public static string FullScreenshotsPath => Path.Combine(ReportsPath, ScreenshotsDir);

    #endregion

    #region Paths

    /// <summary>
    /// Gets the base path for saving reports (traces, videos, screenshots).
    /// </summary>
    /// <value>Directory path. Default: current execution directory.</value>
    public static string ReportsPath => Configuration["Reports:Path"] ?? Directory.GetCurrentDirectory();

    /// <summary>
    /// Gets the directory where browser downloads are saved.
    /// </summary>
    /// <value>Directory path. Default: "Downloads".</value>
    public static string DownloadsPath => Configuration["Downloads:Path"] ?? "Downloads";

    /// <summary>
    /// Gets the full path to the downloads directory.
    /// </summary>
    /// <value>Absolute path combining ReportsPath and DownloadsPath.</value>
    public static string FullDownloadsPath => Path.Combine(ReportsPath, DownloadsPath);

    #endregion

    #region Helper Methods

    /// <summary>
    /// Safely parses a string to bool with a default value.
    /// </summary>
    /// <param name="value">String to convert.</param>
    /// <param name="defaultValue">Default value if conversion fails.</param>
    /// <returns>Parsed boolean value or default.</returns>
    private static bool ParseBool(string? value, bool defaultValue)
    {
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Safely parses a string to int with a default value.
    /// </summary>
    /// <param name="value">String to convert.</param>
    /// <param name="defaultValue">Default value if conversion fails.</param>
    /// <returns>Parsed integer value or default.</returns>
    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    #endregion
}