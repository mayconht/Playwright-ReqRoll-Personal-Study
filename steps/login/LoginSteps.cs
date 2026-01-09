using Microsoft.Playwright;
using Playwright_ReqRoll.hooks;
using Playwright_ReqRoll.pages.login;
using Reqnroll;

namespace Playwright_ReqRoll.steps.login;

/// <summary>
/// Provides step definitions for login-related actions in Reqnroll scenarios.
/// Interacts with the LoginPage to perform user actions and assertions.
/// </summary>
[Binding]
public class LoginSteps
{
    private static LoginPageLocators LoginPageLocators => new(PlaywrightHooks.Page);

    /// <summary>
    /// Enters the specified username into the login form.
    /// </summary>
    /// <param name="testUser">The username to enter.</param>
    [When("I enter username {string}")]
    public static async Task WhenIEnterUsernameString(string testUser)
    {
        await LoginPageLocators.EnterUsername(testUser);
    }

    /// <summary>
    /// Enters the specified password into the login form.
    /// </summary>
    /// <param name="password">The password to enter.</param>
    [When("I enter password {string}")]
    public static async Task WhenIEnterPasswordString(string? password)
    {
        await LoginPageLocators.EnterPassword(password ?? string.Empty);
    }

    /// <summary>
    /// Clicks the login button to submit the login form.
    /// </summary>
    [When("I click the login button")]
    public static async Task WhenIClickTheLoginButton()
    {
        await LoginPageLocators.ClickLoginButton();
    }

    /// <summary>
    /// Asserts that the specified error message is displayed on the page.
    /// </summary>
    /// <param name="message">The error message text to check for.</param>
    [Then("I should see the error message {string}")]
    public static async Task ThenIShouldSeeTheErrorMessageString(string message)
    {
        await LoginPageLocators.WaitForErrorMessage(message);
    }

    /// <summary>
    /// Asserts that the dashboard is displayed for the specified user.
    /// </summary>
    /// <param name="userName">The username to verify in the dashboard.</param>
    [Then("I should see the dashboard for {string}")]
    public static async Task ThenIShouldSeeTheDashboardForString(string userName)
    {
        await LoginPageLocators.AssertDashboardForUser(userName);
    }

    /// <summary>
    /// Clicks the logout button to log out the user.
    /// </summary>
    [When("I click the logout button")]
    public static async Task WhenIClickTheLogoutButton()
    {
        await LoginPageLocators.ClickLogoutButton();
    }

    /// <summary>
    /// Validates that the user is redirected to the login page after logout.
    /// </summary>
    [Then("I should be redirected to the login page")]
    public static async Task ThenIShouldBeRedirectedToTheLoginPage()
    {
        var page = PlaywrightHooks.Page;
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var usernameField = page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Username" });
        await Assertions.Expect(usernameField).ToBeVisibleAsync();
    }

    /// <summary>
    /// Validates that session cookies have been cleared after logout.
    /// </summary>
    [Then("the cookies should be cleared")]
    public static async Task ThenTheCookiesShouldBeCleared()
    {
        var cookies = await LoginPageLocators.GetCookies();

        var sessionCookies = cookies.Where(c =>
            c.Name.Contains("session", StringComparison.OrdinalIgnoreCase) ||
            c.Name.Contains("auth", StringComparison.OrdinalIgnoreCase) ||
            c.Name.Contains("token", StringComparison.OrdinalIgnoreCase) ||
            c.Name.Contains("user", StringComparison.OrdinalIgnoreCase)
        ).ToList();

        Assert.That(sessionCookies, Is.Empty, $"Expected session cookies to be cleared after logout, but found: {string.Join(", ", sessionCookies.Select(c => c.Name))}");
    }
}