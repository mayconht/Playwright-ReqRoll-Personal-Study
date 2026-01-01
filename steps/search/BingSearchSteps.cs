using Playwright_ReqRoll.hooks;
using Playwright_ReqRoll.pages.search;
using Reqnroll;

namespace Playwright_ReqRoll.steps.search;

/// <summary>
/// Provides step definitions for Bing search-related actions in Reqnroll scenarios.
/// </summary>
[Binding]
public class BingSearchSteps
{
    private static BingHomePage BingHomePage => new(PlaywrightHooks.Page);

    /// <summary>
    /// Navigates to the Bing home page.
    /// </summary>
    /// <param name="url">The Bing URL to navigate to.</param>
    [Given(@"I navigate to Bing home page ""(.*)""")]
    public async Task GivenINavigateToBingHomePage(string url)
    {
        await BingHomePage.NavigateToAsync(url);
    }

    /// <summary>
    /// Enters the specified term in the Bing search box.
    /// </summary>
    /// <param name="term">The search term to enter.</param>
    [When(@"I enter ""(.*)"" in the search box")]
    public async Task WhenIEnterInTheSearchBox(string term)
    {
        await BingHomePage.EnterSearchTerm(term);
    }

    /// <summary>
    /// Clicks the Bing search button.
    /// </summary>
    [When(@"I click the Bing search button")]
    public async Task WhenIClickTheSearchButton()
    {
        await BingHomePage.ClickSearch();
    }

    /// <summary>
    /// Asserts that search results are displayed.
    /// </summary>
    /// <param name="term">The search term used for verification message.</param>
    [Then(@"I should see results related to ""(.*)""")]
    public async Task ThenIShouldSeeResultsRelatedTo(string term)
    {
        var hasResults = await BingHomePage.HasResults();
        Assert.That(hasResults, Is.True, $"No results found for {term}");
    }
}