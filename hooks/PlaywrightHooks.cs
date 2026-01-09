using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Reqnroll;

namespace Playwright_ReqRoll.hooks;

/// <summary>
/// Manages Playwright lifecycle for Reqnroll scenarios.
/// Configures browser, context, tracing, screenshots, and videos.
/// </summary>
[Binding]
public partial class PlaywrightHooks
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static IBrowserContext? _context;
    private static readonly ILogger<PlaywrightHooks> Logger = CreateLogger();

    /// <summary>
    /// Gets the current page for the scenario.
    /// </summary>
    /// <exception cref="InvalidOperationException">If page was not initialized.</exception>
    public static IPage Page { get; private set; } = null!;

    /// <summary>
    /// Indicates whether Playwright resources have been initialized.
    /// </summary>
    private static bool IsInitialized => _playwright != null && _browser != null && _context != null;

    #region Lifecycle Hooks

    /// <summary>
    /// Initializes Playwright, browser, and shared context before all tests.
    /// </summary>
    [BeforeTestRun]
    public static async Task GlobalSetup()
    {
        try
        {
            Logger.LogInformation("Starting global Playwright setup...");

            _playwright = await Playwright.CreateAsync();
            var browserType = GetBrowserType();

            Directory.CreateDirectory(Config.FullDownloadsPath);

            _browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = Config.Headless,
                SlowMo = Config.SlowMo,
                DownloadsPath = Config.FullDownloadsPath
            });

            BrowserNewContextOptions contextOptions;

            if (Config.IsMobile)
            {
                var deviceDescriptor = _playwright.Devices[Config.Device];
                contextOptions = new BrowserNewContextOptions(deviceDescriptor);

                Logger.LogInformation("Using device preset: {Device}", Config.Device);
            }
            else
            {
                contextOptions = new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize
                    {
                        Width = Config.ViewportWidth,
                        Height = Config.ViewportHeight
                    },
                    IsMobile = false
                };
            }

            contextOptions.Locale = Config.Locale;
            contextOptions.TimezoneId = Config.TimezoneId;
            contextOptions.ColorScheme = ParseColorScheme(Config.ColorSchemeString);

            contextOptions.RecordVideoDir = Config.RecordVideo ? Config.FullVideoPath : null;
            contextOptions.RecordVideoSize = new RecordVideoSize
            {
                Width = Config.ViewportWidth,
                Height = Config.ViewportHeight
            };

            if (Config.GeolocationEnabled)
            {
                contextOptions.Geolocation = new Geolocation
                {
                    Latitude = (float)Config.GeolocationLatitude,
                    Longitude = (float)Config.GeolocationLongitude
                };
                contextOptions.Permissions = ["geolocation"];
            }

            _context = await _browser.NewContextAsync(contextOptions);

            Logger.LogInformation("Playwright configured successfully. " +
                                  "Browser: {BrowserType}, " +
                                  "Headless: {Headless}, " +
                                  "Viewport: {Width}x{Height}, " +
                                  "Device: {Device}, " +
                                  "Locale: {Locale}, " +
                                  "Timezone: {Timezone}",
                Config.BrowserType, 
                Config.Headless, 
                Config.ViewportWidth, 
                Config.ViewportHeight,
                Config.Device, 
                Config.Locale, 
                Config.TimezoneId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize Playwright");
            throw;
        }
    }

    /// <summary>
    /// Closes browser and disposes Playwright resources after all tests.
    /// </summary>
    [AfterTestRun]
    public static async Task GlobalTeardown()
    {
        try
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                Logger.LogInformation("Browser closed");
            }

            _playwright?.Dispose();
            Logger.LogInformation("Playwright disposed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing Playwright");
        }
    }

    /// <summary>
    /// Creates a new page and starts tracing before each scenario.
    /// </summary>
    [BeforeScenario]
    public async Task SetupScenario()
    {
        try
        {
            EnsureInitialized();

            CreateRequiredDirectories();

            await _context!.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            Page = await _context.NewPageAsync();
            Logger.LogInformation("Scenario started - new page created");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting up scenario");
            throw;
        }
    }

    /// <summary>
    /// Saves artifacts (traces, screenshots, videos) and closes page after each scenario.
    /// </summary>
    /// <param name="scenarioContext">Scenario context with error information.</param>
    [AfterScenario]
    public async Task TeardownScenario(ScenarioContext scenarioContext)
    {
        try
        {
            var scenarioTitle = SanitizeFilename(scenarioContext.ScenarioInfo.Title);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var testFailed = scenarioContext.TestError != null;
            var status = testFailed ? "failed" : "passed";

            await SaveScreenshotIfConfigured(scenarioTitle, timestamp, testFailed);
            await SaveTraceAsync(scenarioTitle, timestamp, testFailed);
            await SaveVideoIfConfigured(scenarioTitle, timestamp, status);

            Logger.LogInformation("Scenario completed: {Title} - {Status}", scenarioTitle, status);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during scenario teardown");
        }
        finally
        {
            await ClosePageSafely();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the browser type instance based on configuration.
    /// </summary>
    /// <returns>Browser type instance.</returns>
    private static IBrowserType GetBrowserType()
    {
        return Config.BrowserType.ToLowerInvariant() switch
        {
            "firefox" => _playwright!.Firefox,
            "webkit" => _playwright!.Webkit,
            _ => _playwright!.Chromium
        };
    }

    /// <summary>
    /// Parses color scheme string to ColorScheme enum.
    /// </summary>
    /// <param name="colorScheme">Color scheme string (light, dark, no-preference).</param>
    /// <returns>ColorScheme enum value.</returns>
    private static ColorScheme ParseColorScheme(string colorScheme)
    {
        return colorScheme.ToLowerInvariant() switch
        {
            "dark" => ColorScheme.Dark,
            "no-preference" => ColorScheme.NoPreference,
            _ => ColorScheme.Light
        };
    }

    /// <summary>
    /// Verifies that Playwright was initialized correctly.
    /// </summary>
    /// <exception cref="InvalidOperationException">If not initialized.</exception>
    private static void EnsureInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Playwright was not initialized. Verify that GlobalSetup was executed.");
    }

    /// <summary>
    /// Creates required directories for artifacts.
    /// </summary>
    private static void CreateRequiredDirectories()
    {
        if (Config.RecordVideo)
            Directory.CreateDirectory(Config.FullVideoPath);

        if (Config.ScreenshotOnSuccess || Config.ScreenshotOnFailure)
            Directory.CreateDirectory(Config.FullScreenshotsPath);

        Directory.CreateDirectory(Config.FullTracesPath);
    }

    /// <summary>
    /// Saves screenshot if configured for the test status.
    /// </summary>
    /// <param name="scenarioTitle">Sanitized scenario title.</param>
    /// <param name="timestamp">Timestamp for filename.</param>
    /// <param name="testFailed">Indicates if test failed.</param>
    private async Task SaveScreenshotIfConfigured(string scenarioTitle, string timestamp, bool testFailed)
    {
        var shouldSave = testFailed ? Config.ScreenshotOnFailure : Config.ScreenshotOnSuccess;
        if (!shouldSave) return;

        var status = testFailed ? "failed" : "passed";
        var screenshotPath = Path.Combine(Config.FullScreenshotsPath, $"{scenarioTitle}_{timestamp}_{status}.png");

        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true, Scale = ScreenshotScale.Device });
        Logger.LogInformation("Screenshot saved: {Path}", screenshotPath);
    }

    /// <summary>
    /// Saves the Playwright trace.
    /// </summary>
    /// <param name="scenarioTitle">Sanitized scenario title.</param>
    /// <param name="timestamp">Timestamp for filename.</param>
    /// <param name="testFailed">Indicates if test failed.</param>
    private static async Task SaveTraceAsync(string scenarioTitle, string timestamp, bool testFailed)
    {
        if (testFailed || Config.SaveTracesOnPass)
        {
            var status = testFailed ? "failed" : "passed";
            var tracePath = Path.Combine(Config.FullTracesPath, $"{scenarioTitle}_{timestamp}_{status}.zip");

            await _context!.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            Logger.LogInformation("Trace saved: {Path}", tracePath);
        }
        else
        {
            await _context!.Tracing.StopAsync();
            Logger.LogDebug("Trace discarded (test passed and SaveTracesOnPass=false)");
        }
    }

    /// <summary>
    /// Saves and renames video if configured.
    /// </summary>
    /// <param name="scenarioTitle">Sanitized scenario title.</param>
    /// <param name="timestamp">Timestamp for filename.</param>
    /// <param name="status">Test status (passed/failed).</param>
    private static async Task SaveVideoIfConfigured(string scenarioTitle, string timestamp, string status)
    {
        if (!Config.RecordVideo || Page.Video == null) return;

        try
        {
            var videoPath = await Page.Video.PathAsync();
            var videoDir = Path.GetDirectoryName(videoPath)!;
            var newVideoPath = Path.Combine(videoDir, $"{scenarioTitle}_{timestamp}_{status}.webm");

            await Page.CloseAsync();
            await Task.Delay(Config.VideoWriteDelayMs);

            if (File.Exists(videoPath))
            {
                File.Move(videoPath, newVideoPath);
                Logger.LogInformation("Video saved: {Path}", newVideoPath);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Could not save video");
        }
    }

    /// <summary>
    /// Closes the page safely.
    /// </summary>
    private static async Task ClosePageSafely()
    {
        try
        {
            if (Page is { IsClosed: false }) await Page.CloseAsync();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error closing page");
        }
    }

    /// <summary>
    /// Sanitizes string for use in filenames.
    /// </summary>
    /// <param name="filename">Name to sanitize.</param>
    /// <returns>Sanitized name with only alphanumeric characters and underscores.</returns>
    private static string SanitizeFilename(string filename)
    {
        return FilenameSanitizerRegex().Replace(filename, "_");
    }

    /// <summary>
    /// Creates logger instance for the class.
    /// </summary>
    /// <returns>Configured logger.</returns>
    private static ILogger<PlaywrightHooks> CreateLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        return loggerFactory.CreateLogger<PlaywrightHooks>();
    }

    /// <summary>
    /// Compiled regex for filename sanitization.
    /// Replaces non-alphanumeric characters with underscore.
    /// </summary>
    [GeneratedRegex("[^a-zA-Z0-9_]")]
    private static partial Regex FilenameSanitizerRegex();

    #endregion
}