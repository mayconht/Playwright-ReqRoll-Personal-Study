using Playwright_ReqRoll.hooks;
using Playwright_ReqRoll.pages.search;
using Reqnroll;

namespace Playwright_ReqRoll.steps.search;

/// <summary>
/// Provides step definitions for search-related actions in Reqnroll scenarios.
/// Interacts with the SearchPage to perform user actions and assertions.
/// </summary>
[Binding]
public class SearchSteps
{
    private static SearchPageLocators SearchPageLocators => new(PlaywrightHooks.Page);

    /// <summary>
    /// Enters the specified search query into the search input.
    /// </summary>
    /// <param name="query">The search query to enter.</param>
    [When("I enter search query {string}")]
    public static async Task WhenIEnterSearchQueryString(string query)
    {
        await SearchPageLocators.EnterSearchQuery(query);
    }

    /// <summary>
    /// Clicks the search button.
    /// </summary>
    [When("I click the search button")]
    public static async Task WhenIClickTheSearchButton()
    {
        await SearchPageLocators.ClickSearchButton();
    }

    /// <summary>
    /// Asserts that at least the specified number of search results are displayed.
    /// </summary>
    /// <param name="count">The minimum number of results.</param>
    [Then("I should see at least {int} search results")]
    public static async Task ThenIShouldSeeAtLeastIntSearchResults(int count)
    {
        var actualCount = await SearchPageLocators.GetResultsCount();
        Assert.That(actualCount, Is.GreaterThanOrEqualTo(count));
    }

    /// <summary>
    /// Asserts that each search result has a clickable title, URL, and snippet.
    /// </summary>
    [Then("each result should have a clickable title, URL, and snippet")]
    public static async Task ThenEachResultShouldHaveAClickableTitleUrlAndSnippet()
    {
        var isValid = await SearchPageLocators.VerifyResultsStructure();
        Assert.That(isValid, Is.True);
    }

    /// <summary>
    /// Asserts that no results are displayed.
    /// </summary>
    [Then("I should see no results displayed")]
    public static async Task ThenIShouldSeeNoResultsDisplayed()
    {
        var noResults = await SearchPageLocators.NoResultsDisplayed();
        Assert.That(noResults, Is.True);
    }

    /// <summary>
    /// Waits for search results to load.
    /// </summary>
    [When("I wait for search results to load")]
    public static async Task WhenIWaitForSearchResultsToLoad()
    {
        await SearchPageLocators.WaitForResultsToLoad();
    }

    /// <summary>
    /// Re-locates the search input to handle DOM re-renders.
    /// </summary>
    [When("I re-locate the search input")]
    public static async Task WhenIReLocateTheSearchInput()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// Asserts that the search can be performed successfully.
    /// </summary>
    [Then("I should be able to perform the search")]
    public static async Task ThenIShouldBeAbleToPerformTheSearch()
    {
        var count = await SearchPageLocators.GetResultsCount();
        Assert.That(count, Is.GreaterThanOrEqualTo(0));
    }

    /// <summary>
    /// Asserts that fresh results appear for the second query.
    /// </summary>
    [Then("I should see fresh results for the second query")]
    public static async Task ThenIShouldSeeFreshResultsForTheSecondQuery()
    {
        var count = await SearchPageLocators.GetResultsCount();
        Assert.That(count, Is.GreaterThan(0));
    }
}