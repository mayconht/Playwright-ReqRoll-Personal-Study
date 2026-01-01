using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Provide Items (Posting Method) page.
/// URL: https://onlinepostage.iompost.com/my-order/provide-items
/// </summary>
public class ProvideItemsPage : BasePage
{
    /// <summary>
    /// Initializes a new instance of the ProvideItemsPage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public ProvideItemsPage(IPage page) : base(page)
    {
    }

    private ILocator CollectionOption => Page.Locator("text=Collection").First;
    private ILocator DropOffAtPostbox => Page.Locator("text=Drop off at a postbox").First;
    private ILocator DropOffAtPostOffice => Page.Locator("text=Drop off at a Post Office counter").First;

    /// <summary>
    /// Selects the posting method based on the provided option.
    /// </summary>
    /// <param name="method">Options: "Collection", "Drop off at a postbox", "Drop off at a Post Office counter".</param>
    /// <exception cref="ArgumentException">Thrown when an unknown posting method is provided.</exception>
    public async Task SelectPostingMethod(string method)
    {
        var locator = method.ToLowerInvariant() switch
        {
            "collection" => CollectionOption,
            "drop off at a postbox" => DropOffAtPostbox,
            "drop off at a post office counter" or "drop off at a post office" => DropOffAtPostOffice,
            _ => throw new ArgumentException($"Unknown posting method: {method}")
        };

        // DispatchEventAsync bypasses all actionability checks and directly dispatches the click event
        await locator.DispatchEventAsync("click");
    }
}