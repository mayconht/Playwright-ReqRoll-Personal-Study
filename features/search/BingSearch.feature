Feature: Bing Search

    @BingSearch
    Scenario: Search for Playwright on Bing
        Given I navigate to Bing home page "https://www.bing.com.br"
        When I enter "Playwright" in the search box
        And I click the Bing search button
        Then I should see results related to "Playwright"