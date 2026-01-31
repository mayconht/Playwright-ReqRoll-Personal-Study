using Microsoft.Playwright;

namespace Playwright_ReqRoll.pages;

/// <summary>
/// Base class for Page Object Models providing common functionality.
/// All page objects should inherit from this class.
/// </summary>
public abstract class BasePage
{
    /// <summary>
    /// The Playwright page instance.
    /// </summary>
    protected readonly IPage Page;

    /// <summary>
    /// Default timeout for page load operations in milliseconds.
    /// </summary>
    private const int DefaultPageLoadTimeout = 10000;

    /// <summary>
    /// Default timeout for element operations in milliseconds.
    /// </summary>
    private const int DefaultElementTimeout = 30000;

    /// <summary>
    /// Initializes a new instance of the BasePage class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    protected BasePage(IPage page)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    #region Navigation

    /// <summary>
    /// Navigates to the specified URL and waits for the page to load.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    /// <param name="waitForNetworkIdle">Whether to wait for network idle state. Default: true.</param>
    public virtual async Task NavigateToAsync(string url, bool waitForNetworkIdle = true)
    {
        await Page.GotoAsync(url);

        if (waitForNetworkIdle) await WaitForPageLoadAsync();
    }

    /// <summary>
    /// Waits for the page to reach network idle state.
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds. Default: 10000.</param>
    public async Task WaitForPageLoadAsync(int timeout = DefaultPageLoadTimeout)
    {
        try
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
            {
                Timeout = timeout
            });
        }
        catch (TimeoutException)
        {
            //Dont throw on timeout - sometimes network idle is not reached but page is usable
        }
    }

    /// <summary>
    /// Waits for the URL to match the specified pattern.
    /// </summary>
    /// <param name="urlPattern">URL pattern to match (supports wildcards with **).</param>
    /// <param name="timeout">Timeout in milliseconds. Default: 10000.</param>
    public async Task WaitForUrlAsync(string urlPattern, int timeout = DefaultPageLoadTimeout)
    {
        await Page.WaitForURLAsync(urlPattern, new PageWaitForURLOptions
        {
            Timeout = timeout
        });
    }

    #endregion

    #region Common Actions

    /// <summary>
    /// Clicks the Continue button on the page.
    /// </summary>
    /// <param name="force">Whether to force click bypassing actionability checks. Default: false.</param>
    public async Task ClickContinueAsync(bool force = false)
    {
        var continueButton = Page.Locator("text=Continue").First;
        await continueButton.ClickAsync(new LocatorClickOptions { Force = force });
    }

    /// <summary>
    /// Fills an input field and presses Tab to trigger validation.
    /// </summary>
    /// <param name="locator">The locator for the input field.</param>
    /// <param name="value">The value to fill.</param>
    public async Task FillAndTabAsync(ILocator locator, string value)
    {
        await locator.FillAsync(value);
        await locator.PressAsync("Tab");
    }

    /// <summary>
    /// Fills an input field if it is visible, then presses Tab.
    /// </summary>
    /// <param name="locator">The locator for the input field.</param>
    /// <param name="value">The value to fill.</param>
    /// <returns>True if the field was filled, false if not visible.</returns>
    public async Task<bool> FillIfVisibleAsync(ILocator locator, string value)
    {
        if (await locator.IsVisibleAsync())
        {
            await FillAndTabAsync(locator, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits for an element to become visible.
    /// </summary>
    /// <param name="locator">The locator for the element.</param>
    /// <param name="timeout">Timeout in milliseconds. Default: 30000.</param>
    public async Task WaitForElementAsync(ILocator locator, int timeout = DefaultElementTimeout)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    /// <summary>
    /// Waits for text to appear on the page.
    /// </summary>
    /// <param name="text">The text to wait for.</param>
    /// <param name="exact">Whether to match exactly. Default: false.</param>
    /// <param name="timeout">Timeout in milliseconds. Default: 30000.</param>
    public async Task WaitForTextAsync(string text, bool exact = false, int timeout = DefaultElementTimeout)
    {
        var textLocator = Page.GetByText(text, new PageGetByTextOptions { Exact = exact }).First;
        await textLocator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    #endregion

    #region Modal Handling

    /// <summary>
    /// Attempts to close any modal dialog that may be obstructing the view.
    /// </summary>
    /// <param name="modalSelector">CSS selector for the modal. Default: ".modal, [role='dialog']".</param>
    /// <param name="closeButtonSelector">CSS selector for close buttons.</param>
    /// <param name="timeout">Timeout in milliseconds. Default: 3000.</param>
    /// <returns>True if a modal was found and closed, false otherwise.</returns>
    public async Task<bool> TryCloseModalAsync(
        string modalSelector = ".modal, [role='dialog'], .login-modal",
        string closeButtonSelector =
            "button.close, .close-button, [aria-label='Close'], button:has-text('Close'), button:has-text('No thanks')",
        int timeout = 3000)
    {
        try
        {
            var modal = Page.Locator(modalSelector);
            await modal.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = timeout
            });

            if (await modal.IsVisibleAsync())
            {
                var closeBtn = modal.Locator(closeButtonSelector).First;

                if (await closeBtn.IsVisibleAsync())
                {
                    await closeBtn.ClickAsync();
                    await modal.WaitForAsync(new LocatorWaitForOptions
                    {
                        State = WaitForSelectorState.Hidden,
                        Timeout = 5000
                    });
                    return true;
                }

                // Fallback: Press Escape
                await Page.Keyboard.PressAsync("Escape");
                await Task.Delay(500);
                return !await modal.IsVisibleAsync();
            }
        }
        catch
        {
            // Modal not found or timeout - this is expected behavior
        }

        return false;
    }

    #endregion
}