using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Parcel Size / Item Type selection page.
/// URL: https://onlinepostage.iompost.com/my-order/parcel-size
/// </summary>
public class ParcelSizePage : BasePage
{
    /// <summary>
    /// Initializes a new instance of the ParcelSizePage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public ParcelSizePage(IPage page) : base(page)
    {
    }

    private ILocator LetterOption => Page
        .Locator("div")
        .Filter(new LocatorFilterOptions
        {
            HasText = "Up to: 100g"
        }).Last;

    private ILocator LargeLetterOption => Page
        .Locator("div")
        .Filter(new LocatorFilterOptions
        {
            HasText = "Large Letter"
        })
        .Filter(new LocatorFilterOptions
        {
            HasText = "Up to: 750g"
        }).Last;

    private ILocator ParcelOption => Page
        .Locator("div")
        .Filter(new LocatorFilterOptions
        {
            HasText = "Parcel"
        })
        .Filter(new LocatorFilterOptions
        {
            HasText = "Up to: 20kg"
        }).Last;

    private ILocator ParcelforceOption => Page
        .Locator("div")
        .Filter(new LocatorFilterOptions
        {
            HasText = "Parcelforce"
        }).Last;

    /// <summary>
    /// Selects the item type based on the provided option.
    /// </summary>
    /// <param name="itemType">Options: "Letter", "Large Letter", "Parcel", "Parcelforce".</param>
    /// <exception cref="ArgumentException">Thrown when an unknown item type is provided.</exception>
    public async Task SelectItemType(string itemType)
    {
        var locator = itemType.ToLowerInvariant() switch
        {
            "letter" => LetterOption,
            "large letter" => LargeLetterOption,
            "parcel" => ParcelOption,
            "parcelforce" => ParcelforceOption,
            _ => throw new ArgumentException($"Unknown item type: {itemType}")
        };

        await locator.ClickAsync(new LocatorClickOptions { Force = true });
    }

    /// <summary>
    /// Waits for this page to load.
    /// </summary>
    public async Task WaitForPage()
    {
        await WaitForUrlAsync("**/my-order/parcel-size**");
    }
}