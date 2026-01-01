using Microsoft.Playwright;
using Playwright_ReqRoll.pages;

namespace Playwright_ReqRoll.pages.search;

/// <summary>
/// Page Object for the Search page in the application.
/// Provides methods to interact with search form elements and verify results.
/// </summary>
public class SearchPageLocators : BasePage
{
    /// <summary>
    /// Initializes a new instance of the SearchPageLocators class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public SearchPageLocators(IPage page) : base(page)
    {
    }

    private ILocator SearchInput => Page.GetByTestId("search-input");
    private ILocator SearchButton => Page.GetByTestId("search-button");
    private ILocator SearchResults => Page.Locator(".MuiCard-root");

    /// <summary>
    /// Enters the specified query into the search input.
    /// </summary>
    /// <param name="query">The search query to enter.</param>
    public async Task EnterSearchQuery(string query)
    {
        await SearchInput.FillAsync(query);
    }

    /// <summary>
    /// Clicks the search button to submit the search query.
    /// </summary>
    public async Task ClickSearchButton()
    {
        await SearchButton.ClickAsync();
    }

    /// <summary>
    /// Gets the count of search results displayed on the page.
    /// </summary>
    /// <returns>The number of search results.</returns>
    public async Task<int> GetResultsCount()
    {
        await SearchResults.First.WaitForAsync();
        return await SearchResults.CountAsync();
    }

    /// <summary>
    /// Verifies that each search result has the required structure (title, URL, snippet).
    /// </summary>
    /// <returns>True if all results have the required elements, false otherwise.</returns>
    public async Task<bool> VerifyResultsStructure()
    {
        var results = SearchResults.Locator("div.MuiPaper-root");
        var count = await results.CountAsync();

        for (var i = 0; i < count; i++)
        {
            var result = results.Nth(i);
            var title = result.Locator("a");
            var url = result.Locator("p.text-green-700");
            var snippet = result.Locator("p.text-slate-700");

            if (await title.CountAsync() == 0 || await url.CountAsync() == 0 || await snippet.CountAsync() == 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if no results are displayed on the page.
    /// </summary>
    /// <returns>True if no results are shown, false otherwise.</returns>
    public async Task<bool> NoResultsDisplayed()
    {
        return await SearchResults.CountAsync() == 0;
    }

    /// <summary>
    /// Waits for search results to load.
    /// </summary>
    public async Task WaitForResultsToLoad()
    {
        await SearchResults.First.WaitForAsync();
    }
}