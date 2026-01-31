using Bogus;
using Microsoft.Playwright;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Sender's Details page.
/// URL: https://onlinepostage.iompost.com/my-order/senders-details
/// </summary>
public class SendersDetailsPage(IPage page) : BasePage(page)
{
    private readonly Faker _faker = new("en_GB");

    // ===== SENDER DETAILS FIELDS (visible in the form) =====
    private ILocator FirstNameInput => Page.GetByLabel("First name", new PageGetByLabelOptions { Exact = false });
    private ILocator LastNameInput => Page.GetByLabel("Last name", new PageGetByLabelOptions { Exact = false });
    private ILocator PhoneInput => Page.GetByLabel("Contact telephone", new PageGetByLabelOptions { Exact = false });
    private ILocator EmailInput => Page.GetByLabel("Email address", new PageGetByLabelOptions { Exact = false });
    private ILocator ConfirmEmailInput => Page.GetByLabel("Confirm email", new PageGetByLabelOptions { Exact = false });

    // ===== ADDRESS LOOKUP =====
    private ILocator FindAddressInput => Page.Locator(".simple-typeahead-input");
    private ILocator AddressResults => Page.Locator(".simple-typeahead-list-item");
    private ILocator AddToBasketButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to basket" });

    // ===== ADDRESS LOOKUP METHODS =====

    /// <summary>
    /// Searches for a sender's address using postcode lookup.
    /// </summary>
    /// <param name="postcode">The postcode to search for.</param>
    public async Task SearchAddress(string postcode)
    {
        await FindAddressInput.FillAsync(postcode);
        await AddressResults.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });
    }

    /// <summary>
    /// Selects an address from the lookup results.
    /// </summary>
    /// <param name="index">The index of the address to select. Default: 0 (first result).</param>
    public async Task SelectAddressFromResults(int index = 0)
    {
        var count = await AddressResults.CountAsync();
        if (count > index)
        {
            await AddressResults.Nth(index).ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
        }
    }

    // ===== SENDER DETAILS METHODS =====

    /// <summary>
    /// Enters the sender's first name.
    /// </summary>
    /// <param name="firstName">The first name to enter.</param>
    public async Task EnterFirstName(string firstName)
    {
        await FirstNameInput.FillAsync(firstName);
    }

    /// <summary>
    /// Enters the sender's last name.
    /// </summary>
    /// <param name="lastName">The last name to enter.</param>
    public async Task EnterLastName(string lastName)
    {
        await LastNameInput.FillAsync(lastName);
    }

    /// <summary>
    /// Enters the sender's phone number.
    /// </summary>
    /// <param name="phone">The phone number to enter.</param>
    public async Task EnterPhone(string phone)
    {
        await PhoneInput.FillAsync(phone);
    }

    /// <summary>
    /// Enters the sender's email address.
    /// </summary>
    /// <param name="email">The email address to enter.</param>
    public async Task EnterEmail(string email)
    {
        await EmailInput.FillAsync(email);
    }

    /// <summary>
    /// Confirms the sender's email address.
    /// </summary>
    /// <param name="email">The email address to confirm.</param>
    public async Task ConfirmEmail(string email)
    {
        await ConfirmEmailInput.FillAsync(email);
    }

    /// <summary>
    /// Clicks the "Add to basket" button to continue to the next step.
    /// </summary>
    public async Task ClickContinueAsync()
    {
        var enabledButton = Page.Locator("button.main-button.--form-nav:not(.--disabled)");
        await enabledButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });

        await enabledButton.ScrollIntoViewIfNeededAsync();

        await enabledButton.ClickAsync();
    }

}
