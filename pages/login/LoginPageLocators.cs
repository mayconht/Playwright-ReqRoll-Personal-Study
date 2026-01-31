using Microsoft.Playwright;

namespace Playwright_ReqRoll.pages.login;

/// <summary>
/// Page Object for the Login page in the application.
/// Provides methods to interact with login form elements and verify authentication state.
/// </summary>
public class LoginPageLocators : BasePage
{
    /// <summary>
    /// Initializes a new instance of the LoginPageLocators class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public LoginPageLocators(IPage page) : base(page)
    {
    }

    private ILocator UsernameTextbox => Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Username" });
    private ILocator PasswordTextbox => Page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Password" });
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Login" });
    private ILocator WelcomeHeading => Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Level = 5 });
    private ILocator LoggedInAlert => Page.Locator(".MuiAlert-message");
    private ILocator LogoutButton => Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Logout" });

    /// <summary>
    /// Enters the specified username into the username textbox.
    /// </summary>
    /// <param name="username">The username to enter.</param>
    public async Task EnterUsername(string username)
    {
        await UsernameTextbox.FillAsync(username);
    }

    /// <summary>
    /// Enters the specified password into the password textbox.
    /// </summary>
    /// <param name="password">The password to enter.</param>
    public async Task EnterPassword(string password)
    {
        await PasswordTextbox.FillAsync(password);
    }

    /// <summary>
    /// Clicks the login button to submit the login form.
    /// </summary>
    public async Task ClickLoginButton()
    {
        await LoginButton.ClickAsync();
    }
    

    /// <summary>
    /// Waits for the specified error message to appear on the page.
    /// </summary>
    /// <param name="message">The error message text to wait for.</param>
    public async Task WaitForErrorMessage(string message)
    {
        await Page.GetByText(message).WaitForAsync();
    }

    /// <summary>
    /// Asserts that the dashboard is displayed for the specified user.
    /// Checks for welcome message and logged-in alert containing the username.
    /// </summary>
    /// <param name="userName">The username to verify in the dashboard messages.</param>
    public async Task AssertDashboardForUser(string userName)
    {
        var welcomeMessages = await WelcomeHeading.AllInnerTextsAsync();
        Assert.That(
            welcomeMessages.Any(msg => msg.Contains(userName, StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"Welcome message does not contain the username '{userName}'. Actual messages: {string.Join(", ", welcomeMessages)}");

        var alertMessages = await LoggedInAlert.AllInnerTextsAsync();
        Assert.That(
            alertMessages.Any(msg =>
                msg.Contains($"You are logged in as {userName.ToUpper()}", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            $"Banner message does not contain the expected text 'You are logged in as {userName.ToUpper()}'. Actual messages: {string.Join(", ", alertMessages)}");
    }

    /// <summary>
    /// Clicks the logout button to log out the user.
    /// </summary>
    public async Task ClickLogoutButton()
    {
        await LogoutButton.ClickAsync();
    }

    /// <summary>
    /// Gets all cookies from the current browser context.
    /// </summary>
    /// <returns>A list of all cookies in the current context.</returns>
    public async Task<IReadOnlyList<BrowserContextCookiesResult>> GetCookies()
    {
        return await Page.Context.CookiesAsync();
    }
    
}