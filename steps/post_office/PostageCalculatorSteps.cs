using Playwright_ReqRoll.hooks;
using Playwright_ReqRoll.pages.post_office;
using Reqnroll;

namespace Playwright_ReqRoll.steps.post_office;

/// <summary>
/// Provides step definitions for the Postage Calculator flow in Reqnroll scenarios.
/// </summary>
[Binding]
public class PostageCalculatorSteps
{
    private static HomePage HomePage => new(PlaywrightHooks.Page);
    private static ProvideItemsPage ProvideItemsPage => new(PlaywrightHooks.Page);
    private static ParcelSizePage ParcelSizePage => new(PlaywrightHooks.Page);
    private static ServiceSelectionPage ServiceSelectionPage => new(PlaywrightHooks.Page);
    private static RecipientDetailsPage RecipientDetailsPage => new(PlaywrightHooks.Page);

    #region Home Page Steps

    /// <summary>
    /// Navigates to the online postage page.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    [Given(@"I navigate to the online postage page ""(.*)""")]
    public async Task GivenINavigateToTheOnlinePostagePage(string url)
    {
        await HomePage.NavigateToAsync(url);
    }

    /// <summary>
    /// Selects the destination country.
    /// </summary>
    /// <param name="country">The country to select.</param>
    [When(@"I select the ""(.*)""")]
    public async Task WhenISelectThe(string country)
    {
        await HomePage.SelectDestination(country);
    }

    /// <summary>
    /// Enters the weight value.
    /// </summary>
    /// <param name="weight">The weight to enter.</param>
    [When(@"I enter weight ""(.*)""")]
    public async Task WhenIEnterWeight(string weight)
    {
        await HomePage.EnterWeight(weight);
    }

    /// <summary>
    /// Selects the weight format (Grams or Kg).
    /// </summary>
    /// <param name="format">The format to select.</param>
    [When(@"I select format ""(.*)""")]
    public async Task WhenISelectFormat(string format)
    {
        await HomePage.SelectFormat(format);
    }

    /// <summary>
    /// Clicks the Get Started button.
    /// </summary>
    [When(@"I click the Get Started button")]
    public async Task WhenIClickTheGetStartedButton()
    {
        await HomePage.ClickGetStarted();
    }

    /// <summary>
    /// Verifies that the specified step text is visible.
    /// </summary>
    /// <param name="stepName">The step name to verify.</param>
    [Then(@"I should see the text step ""(.*)""")]
    public async Task ThenIShouldSeeTheTextStep(string stepName)
    {
        await HomePage.WaitForNextStep(stepName);
    }

    #endregion

    #region Provide Items Page Steps

    /// <summary>
    /// Selects the posting method.
    /// </summary>
    /// <param name="method">The posting method to select.</param>
    [When(@"I select posting method ""(.*)""")]
    public async Task WhenISelectPostingMethod(string method)
    {
        await ProvideItemsPage.SelectPostingMethod(method);
    }

    /// <summary>
    /// Clicks Continue on the posting method page.
    /// </summary>
    [When(@"I click Continue on posting method")]
    public async Task WhenIClickContinueOnPostingMethod()
    {
        await ProvideItemsPage.ClickContinueAsync();
    }

    #endregion

    #region Parcel Size Page Steps

    /// <summary>
    /// Selects the item type.
    /// </summary>
    /// <param name="itemType">The item type to select.</param>
    [When(@"I select item type ""(.*)""")]
    public async Task WhenISelectItemType(string itemType)
    {
        await ParcelSizePage.SelectItemType(itemType);
    }

    /// <summary>
    /// Clicks Continue on the parcel size page.
    /// </summary>
    [When(@"I click Continue on parcel size")]
    public async Task WhenIClickContinueOnParcelSize()
    {
        await ParcelSizePage.ClickContinueAsync();
    }

    #endregion

    #region Service Selection Steps

    /// <summary>
    /// Waits for the new service options to load.
    /// </summary>
    [Then("The page should load the new options")]
    public async Task ThenThePageShouldLoadTheNewOptions()
    {
        await ServiceSelectionPage.WaitForNewOptions();
    }

    /// <summary>
    /// Selects the specified service.
    /// </summary>
    /// <param name="serviceName">The service name to select.</param>
    [When(@"I select the service ""(.*)""")]
    public async Task WhenISelectTheService(string serviceName)
    {
        await ServiceSelectionPage.SelectService(serviceName);
    }

    /// <summary>
    /// Verifies that the total price is displayed correctly.
    /// </summary>
    [Then(@"the total price should change to the selected service")]
    public async Task ThenTheTotalPriceShouldChangeToTheSelectedService()
    {
        var price = await ServiceSelectionPage.GetFinalPrice();

        Assert.Multiple(() =>
        {
            Assert.That(price, Is.Not.Null.And.Not.Empty, "Price should not be empty");
            Assert.That(price, Does.StartWith("£"), "Price should start with £");
            Assert.That(price, Does.Match(@"^£\d+\.\d{2}$"), "Price should be in format £X.XX");
        });
    }

    /// <summary>
    /// Clicks Continue on the service selection page.
    /// </summary>
    [When(@"I click Continue on service selection")]
    [Then(@"I click Continue on service selection")]
    public async Task WhenIClickContinueOnServiceSelection()
    {
        await ServiceSelectionPage.ClickContinueAsync();
    }

    #endregion

    #region Recipient Details Page Steps

    [When("I try to find the {string}")]
    public async Task WhenITryToFindThe(string postcode)
    {
        await RecipientDetailsPage.SearchAddress(postcode);
    }

    /// <summary>
    /// Fills recipient details with auto-generated fake data.
    /// </summary>
    [When(@"I fill recipient details with fake data")]
    public async Task WhenIFillRecipientDetailsWithFakeData()
    {
    }

    /// <summary>
    /// Enters the recipient company name.
    /// </summary>
    /// <param name="company">The company name to enter.</param>
    [When(@"I enter recipient company ""(.*)""")]
    public async Task WhenIEnterRecipientCompany(string company)
    {
        await RecipientDetailsPage.EnterCompany(company);
    }

    /// <summary>
    /// Enters the recipient address line 1.
    /// </summary>
    /// <param name="address">The address to enter.</param>
    [When(@"I enter recipient address line 1 ""(.*)""")]
    public async Task WhenIEnterRecipientAddressLine1(string address)
    {
        await RecipientDetailsPage.EnterAddressLine1(address);
    }

    /// <summary>
    /// Enters the recipient town.
    /// </summary>
    /// <param name="town">The town to enter.</param>
    [When(@"I enter recipient town ""(.*)""")]
    public async Task WhenIEnterRecipientTown(string town)
    {
        await RecipientDetailsPage.EnterTownCity(town);
    }

    /// <summary>
    /// Enters the recipient postcode.
    /// </summary>
    /// <param name="postcode">The postcode to enter.</param>
    [When(@"I enter recipient postcode ""(.*)""")]
    public async Task WhenIEnterRecipientPostcode(string postcode)
    {
        await RecipientDetailsPage.EnterPostcode(postcode);
    }

    /// <summary>
    /// Searches for an address using the postcode lookup.
    /// </summary>
    /// <param name="postcode">The postcode to search for.</param>
    [When(@"I search for address ""(.*)""")]
    public async Task WhenISearchForAddress(string postcode)
    {
        await RecipientDetailsPage.SearchAddress(postcode);
    }

    /// <summary>
    /// Selects the first address from the lookup results.
    /// </summary>
    [When(@"I select the first address from results")]
    public async Task WhenISelectTheFirstAddressFromResults()
    {
        await RecipientDetailsPage.SelectAddressFromResults(0);
    }


    [When("I set sending to business {string} and {string}")]
    public async Task WhenISetSendingToBusinessAnd(string sendingToBusiness, string businessName)
    {
        await RecipientDetailsPage.SetSendingToBusiness(bool.Parse(sendingToBusiness));

        if (bool.Parse(sendingToBusiness) && !string.IsNullOrEmpty(businessName))
            await RecipientDetailsPage.EnterBusinessName(businessName);
    }

    /// <summary>
    /// Clicks Continue on the recipient details page.
    /// </summary>
    [When(@"I click Continue on recipient details")]
    public async Task WhenIClickContinueOnRecipientDetails()
    {
        await RecipientDetailsPage.ClickContinueAsync();
    }

    #endregion


    [When("I enter recipient first name {string}")]
    public async Task WhenIEnterRecipientFirstName(string firstName)
    {
        await RecipientDetailsPage.EnterFirstName(firstName);
    }


    [When("I enter recipient last name {string}")]
    public async Task WhenIEnterRecipientLastName(string lastName)
    {
        await RecipientDetailsPage.EnterLastName(lastName);
    }
}