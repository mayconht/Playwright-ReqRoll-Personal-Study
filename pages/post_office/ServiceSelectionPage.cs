using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Service Selection panel.
/// Appears on the Parcel Size page after selecting an item type.
/// </summary>
public class ServiceSelectionPage : BasePage
{
    /// <summary>
    /// Initializes a new instance of the ServiceSelectionPage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public ServiceSelectionPage(IPage page) : base(page)
    {
    }

    private ILocator UkLetterOption => GetServiceLabel("SPPiPUKLetter");
    private ILocator NextDayNineAmOption => GetServiceLabel("SSD1D9AM");
    private ILocator TrackedOption => GetServiceLabel("STracked48SPcl");
    private ILocator NextDayByOnePmOption => GetServiceLabel("SSD1D1PM");
    private ILocator TwoDayOption => GetServiceLabel("SSD2D750");
    private ILocator TotalPrice => Page.Locator("[class='service-item\\.price']").First;
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Continue" });

    /// <summary>
    /// Gets the locator for a service by its value.
    /// </summary>
    /// <param name="serviceValue">The service input value.</param>
    /// <returns>The locator for the service label.</returns>
    private ILocator GetServiceLabel(string serviceValue)
    {
        return Page.Locator("label.service-item")
            .Filter(new LocatorFilterOptions
            {
                Has = Page.Locator($"input[value='{serviceValue}']")
            });
    }

    /// <summary>
    /// Gets the locator for a service by name.
    /// </summary>
    /// <param name="serviceName">The display name of the service.</param>
    /// <returns>The locator for the service.</returns>
    /// <exception cref="ArgumentException">Thrown when an unknown service name is provided.</exception>
    private ILocator GetServiceLocator(string serviceName)
    {
        return serviceName.ToLowerInvariant() switch
        {
            "united kingdom letter" => UkLetterOption,
            "special delivery two day by 5.30pm" => TwoDayOption,
            "special delivery next day before 1pm" => NextDayByOnePmOption,
            "tracked" => TrackedOption,
            "special delivery next day before 9am" => NextDayNineAmOption,
            _ => throw new ArgumentException($"Unknown service: {serviceName}")
        };
    }

    /// <summary>
    /// Waits for new service options to load after selection.
    /// </summary>
    public async Task WaitForNewOptions()
    {
        await WaitForPageLoadAsync(5000);
    }

    /// <summary>
    /// Selects the service based on the provided option.
    /// </summary>
    /// <param name="serviceName">The name of the service to select.</param>
    public async Task SelectService(string serviceName)
    {
        var locator = GetServiceLocator(serviceName);
        await locator.ClickAsync(new LocatorClickOptions { Force = true });
    }

    /// <summary>
    /// Gets the final price displayed on the page.
    /// </summary>
    /// <param name="includeVat">Whether to include VAT text. Default: false.</param>
    /// <returns>The price string.</returns>
    public async Task<string> GetFinalPrice(bool includeVat = false)
    {
        var priceText = await TotalPrice.InnerTextAsync();

        if (includeVat)
            return priceText.Trim();

        var match = Regex.Match(priceText, @"£\d+\.\d{2}");
        return match.Success ? match.Value : priceText.Trim();
    }
}