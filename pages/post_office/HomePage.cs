using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Post Office home page.
/// URL: https://onlinepostage.iompost.com/
/// </summary>
public class HomePage : BasePage
{
    /// <summary>
    /// Initializes a new instance of the HomePage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public HomePage(IPage page) : base(page)
    {
    }

    private ILocator DestinationCountry => Page.Locator("#destinationCountry");
    private ILocator WeightInput => Page.Locator("#weight");
    private ILocator GramsRadio => Page.Locator("text=Grams").First;
    private ILocator KgRadio => Page.Locator("label").Filter(new LocatorFilterOptions { HasText = "Kg" });

    private ILocator GetStartedButton =>
        Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Get Started" });

    /// <summary>
    /// Selects the destination country from the dropdown.
    /// </summary>
    /// <param name="country">The country name to select.</param>
    public async Task SelectDestination(string country)
    {
        await DestinationCountry.SelectOptionAsync([new SelectOptionValue { Label = country }]);
        await DestinationCountry.PressAsync("Tab");
    }

    /// <summary>
    /// Enters the weight value into the weight input field.
    /// </summary>
    /// <param name="weight">The weight value to enter.</param>
    public async Task EnterWeight(string weight)
    {
        await FillAndTabAsync(WeightInput, weight);
    }

    /// <summary>
    /// Selects the weight format (Grams or Kg).
    /// </summary>
    /// <param name="format">The format to select: "Grams" or "Kg".</param>
    public async Task SelectFormat(string format)
    {
        var locator = format.ToLowerInvariant() switch
        {
            "grams" => GramsRadio,
            "kg" or "kilos" => KgRadio,
            _ => throw new ArgumentException($"Unknown format: {format}")
        };

        await locator.ClickAsync();
    }

    /// <summary>
    /// Clicks the Get Started button and waits for navigation.
    /// </summary>
    public async Task ClickGetStarted()
    {
        await GetStartedButton.ClickAsync(new LocatorClickOptions { Force = true });
        await WaitForUrlAsync("**/my-order/**");
    }

    /// <summary>
    /// Waits for a specific step to appear on the page.
    /// </summary>
    /// <param name="stepName">The name of the step to wait for.</param>
    public async Task WaitForNextStep(string stepName)
    {
        await WaitForTextAsync(stepName, false);
    }
}