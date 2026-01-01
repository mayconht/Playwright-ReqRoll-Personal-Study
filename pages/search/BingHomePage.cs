using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.search;

/// <summary>
/// Page Object for the Bing home page.
/// Provides methods to interact with Bing search functionality.
/// </summary>
public class BingHomePage : BasePage
{
    /// <summary>
    /// Initializes a new instance of the BingHomePage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public BingHomePage(IPage page) : base(page)
    {
    }

    private ILocator SearchInput => Page.Locator("#sb_form_q");
    private ILocator SearchResults => Page.Locator("#b_results .b_algo");

    /// <summary>
    /// Enters the specified search term into the Bing search input.
    /// </summary>
    /// <param name="term">The search term to enter.</param>
    public async Task EnterSearchTerm(string term)
    {
        await SearchInput.FillAsync(term);
    }

    /// <summary>
    /// Submits the search by pressing Enter on the search input.
    /// </summary>
    public async Task ClickSearch()
    {
        await SearchInput.PressAsync("Enter");
    }

    /// <summary>
    /// Checks if search results are displayed.
    /// </summary>
    /// <returns>True if results are present, false otherwise.</returns>
    public async Task<bool> HasResults()
    {
        await SearchResults.First.WaitForAsync();
        return await SearchResults.CountAsync() > 0;
    }
}