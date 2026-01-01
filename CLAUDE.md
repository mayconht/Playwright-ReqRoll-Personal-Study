# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Test Commands

```bash
dotnet restore                                    # Restore dependencies
dotnet build                                      # Build the project
dotnet test                                       # Run all tests
dotnet test --filter "Category=Login"             # Run tests by category tag
dotnet test --filter "Category=Search"            # Run search tests
dotnet test --filter "FullyQualifiedName~LoginFlow"  # Run specific feature
pwsh bin/Debug/net8.0/playwright.ps1 install      # Install Playwright browsers
```

## Architecture

This is a BDD UI automation framework using **Reqnroll** (successor to SpecFlow) with **Playwright** for browser
automation.

### Project Structure

- `features/` - Gherkin `.feature` files organized by domain (login/, search/, post_office/)
- `steps/` - Step definitions that implement Gherkin steps
- `pages/` - Page Object Models containing locators and action methods
- `hooks/` - Test lifecycle management (browser setup, tracing, screenshots, video capture)

### Key Patterns

**Shared Browser Context**: A single browser context is shared across all scenarios (`PlaywrightHooks.cs`). Tests run
sequentially (non-parallel) to maintain state.

**Page Object Model**: Each page has its own class with:

- Locators using ARIA roles, test IDs, or CSS selectors
- Action methods (e.g., `EnterUsername()`, `ClickLoginButton()`)
- Assertion methods (e.g., `AssertDashboardForUser()`)

**Dynamic DOM Handling**: Pages like `SearchPageLocators.cs` store element handles to detect DOM re-renders and
re-locate stale elements.

**Test Tags**: Features use tags for categorization:

- `@Login`, `@Search`, `@PostageCalculator` - Domain
- `@Positive`, `@Negative` - Test type
- `@High`, `@Medium` - Priority
- `@Smoke`, `@FullFlow` - Test scope

## Configuration

**`appsettings.json`** controls:

- `Browser.Type`: "chromium", "firefox", or "webkit"
- `Browser.Headless`: true/false
- `Browser.SlowMo`: delay in ms between actions
- `Tracing.SaveOnPass`: save traces for passing tests
- `Video.Record`: enable video recording
- `Screenshots.OnSuccess/OnFailure`: capture screenshots

**`Config.cs`** provides static accessors for these settings.

## Debugging Failed Tests

Traces are saved to `Playwright-Traces/` for failed tests. View them at:

- Online: https://trace.playwright.dev/
- Local: `playwright show-trace path/to/trace.zip`

Videos saved to `Playwright-Videos/`, screenshots to `Playwright-Screenshots/`.
