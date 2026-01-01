using Bogus;
using Microsoft.Playwright;

namespace Playwright_ReqRoll.pages.post_office;

/// <summary>
/// Page Object for the Recipient Details page.
/// URL: https://onlinepostage.iompost.com/my-order/recipient-details
/// Uses Bogus library for generating fake test data.
/// </summary>
public class RecipientDetailsPage(IPage page) : BasePage(page)
{
    private readonly Faker _faker = new("en_GB");

    // ===== ADDRESS LOOKUP =====
    private ILocator AddressSearchInput => Page.Locator(".simple-typeahead-input");
    private ILocator AddressResults => Page.Locator(".simple-typeahead-list-item");

    // ===== DESTINATION ADDRESS FIELDS =====
    private ILocator CompanyInput => Page.Locator("#RECIPIENTCOMPANY");
    private ILocator AddressLine1Input => Page.Locator("#RECIPIENTADDRESSLINE1");
    private ILocator AddressLine2Input => Page.Locator("#RECIPIENTADDRESSLINE2");
    private ILocator AddressLine3Input => Page.Locator("#RECIPIENTADDRESSLINE3");
    private ILocator TownCityInput => Page.Locator("#RECIPIENTCITY");
    private ILocator CountyInput => Page.Locator("#RECIPIENTCOUNTY");
    private ILocator PostcodeInput => Page.Locator("#RECIPIENTPOSTCODE");
    private ILocator CountryInput => Page.Locator("#recipientCountry");

    // ===== RECIPIENT DETAILS FIELDS =====
    private ILocator BusinessToggle => Page.Locator("[class='toggle-switch.switch.checkbox']");
    private ILocator BusinessToggleLabel => Page.Locator("label[class='toggle-switch.switch']");
    private ILocator BusinessName => Page.Locator("#recipientBusiness");
    private ILocator FirstNameInput => Page.Locator("#recipientFirstName");
    private ILocator LastNameInput => Page.Locator("#recipientLastName");

    // ===== BUTTONS =====
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Continue" });

    /// <summary>
    /// Generates fake recipient data using Bogus.
    /// </summary>
    private RecipientData GenerateFakeRecipient()
    {
        return new RecipientData
        {
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Company = _faker.Company.CompanyName(),
            AddressLine1 = _faker.Address.StreetAddress(),
            AddressLine2 = _faker.Address.SecondaryAddress(),
            AddressLine3 = null,
            TownCity = _faker.Address.City(),
            County = _faker.Address.County(),
            Postcode = GenerateUkPostcode(),
            IsBusiness = false
        };
    }

    /// <summary>
    /// Generates a realistic UK postcode format.
    /// </summary>
    private string GenerateUkPostcode()
    {
        var outward = _faker.Random.String2(1, "ABCDEFGHJKLMNPRSTUWXYZ") +
                      _faker.Random.String2(1, "ABCDEFGHJKLMNPRSTUWXYZ") +
                      _faker.Random.Number(1, 99);
        var inward = _faker.Random.Number(1, 9) +
                     _faker.Random.String2(2, "ABDEFGHJLNPQRSTUWXYZ");
        return $"{outward} {inward}";
    }

    // ===== ADDRESS LOOKUP METHODS =====

    public async Task SearchAddress(string postcode)
    {
        await AddressSearchInput.FillAsync(postcode);
        await AddressResults.First.WaitForAsync();
    }

    public async Task SelectAddressFromResults(int index = 0)
    {
        var count = await AddressResults.CountAsync();
        if (count > index) await AddressResults.Nth(index).ClickAsync();
    }

    // ===== DESTINATION ADDRESS METHODS =====

    public async Task EnterCompany(string company)
    {
        await FillIfVisibleAsync(CompanyInput, company);
    }

    public async Task EnterAddressLine1(string address)
    {
        await AddressLine1Input.FillAsync(address);
    }

    public async Task EnterAddressLine2(string address)
    {
        await FillIfVisibleAsync(AddressLine2Input, address);
    }

    public async Task EnterAddressLine3(string address)
    {
        await FillIfVisibleAsync(AddressLine3Input, address);
    }

    public async Task EnterTownCity(string town)
    {
        await TownCityInput.FillAsync(town);
    }

    public async Task EnterPostcode(string postcode)
    {
        await PostcodeInput.FillAsync(postcode);
    }

    public async Task<string> GetCountry()
    {
        return await CountryInput.InputValueAsync();
    }

    // ===== RECIPIENT DETAILS METHODS =====


    /// <summary>
    /// Sets the "Sending to a business" toggle.
    /// Uses DispatchEventAsync to bypass visibility checks on hidden checkbox.
    /// </summary>
    /// <param name="isBusiness">True to enable business recipient, false to disable.</param>
    public async Task SetSendingToBusiness(bool isBusiness)
    {
        await BusinessToggleLabel.ClickAsync();
        if(isBusiness)
            await BusinessToggleLabel.SetCheckedAsync(isBusiness);
    }

    public async Task EnterBusinessName(string businessName)
    {
        await BusinessName.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await BusinessName.FillAsync(businessName);
    }

    public async Task EnterFirstName(string firstName)
    {
        await FirstNameInput.FillAsync(firstName);
    }

    public async Task EnterLastName(string lastName)
    {
        await LastNameInput.FillAsync(lastName);
    }
    
}

/// <summary>
/// Data class for recipient information.
/// </summary>
public class RecipientData
{
    // Recipient details
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsBusiness { get; set; }

    // Destination address
    public string? Company { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public required string TownCity { get; set; }
    public string? County { get; set; }
    public required string Postcode { get; set; }
}