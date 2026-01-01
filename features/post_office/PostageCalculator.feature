@PostageCalculator
Feature: Postage Calculator - Full Flow
As a customer
I want to calculate and prepare postage for my mail
So that I can send items through the Isle of Man Post Office

    Background:
        Given I navigate to the online postage page "https://onlinepostage.iompost.com/"

# ============================================
# SCENARIO OUTLINE: Complete Postage Flow
# ============================================

    @Positive
    @FullFlow
    Scenario Outline: Complete postage flow for <ItemType> to <Destination>
        # Step 1: Home Page - Enter package details
        When I select the "<Destination>"
        And I enter weight "<Weight>"
        And I select format "<Format>"
        And I click the Get Started button
        Then I should see the text step "How will you get your items to us?"

        # Step 2: Postage Calculator
        When I select posting method "<PostingMethod>"
        And I click Continue on posting method

        # Step 3: Item Type / Parcel Size Selection
        When I select item type "<ItemType>"

        # Step 4: Service Selection
        Then The page should load the new options
        When I select the service "<Service>"
        Then the total price should change to the selected service
        Then I click Continue on service selection

        # Step 5: Recipient Details
        When I search for address "<RecipientPostcode>"
        And I select the first address from results
        And I enter recipient company "<RecipientName>"
        And I enter recipient address line 1 "<RecipientAddress>"
        And I enter recipient town "<RecipientTown>"
        And I enter recipient postcode "<RecipientPostcode>"
        And I set sending to business "<SendingToBusiness>" and "<RecipientName>"
        And I enter recipient first name "<FirstName>"
        And I enter recipient last name "<LastName>"
        # UI - Phone Number is shown in a different page depending on the package selected.
        And I click Continue on recipient details

        Examples: UK Letter Services
          | Destination    | Weight | Format | PostingMethod                     | ItemType | Service               | RecipientName | RecipientAddress | RecipientTown | RecipientPostcode | SendingToBusiness | FirstName | LastName |
          | United Kingdom | 50     | Grams  | Drop off at a Post Office counter | Letter   | United Kingdom Letter | Test Corp     | 123 Main St      | PATHHEAD      | EH37 5PT          | true              | User1     | Name1    |

       # Examples: UK Special Delivery
       #   | Destination    | Weight | Format | PostingMethod                     | ItemType | Service                            | RecipientName | RecipientAddress | RecipientTown | RecipientPostcode | SendingToBusiness | FirstName | LastName |
       #   | United Kingdom | 50     | Grams  | Drop off at a Post Office counter | Letter   | Special Delivery Two Day by 5.30pm | Fast Co       | UNIT M3A         | BICESTER      | OX26 4LD          | false             | User1     | Name1    |

        Examples: UK Parcel Services
          | Destination    | Weight | Format | PostingMethod                     | ItemType | Service               | RecipientName | RecipientAddress | RecipientTown | RecipientPostcode | SendingToBusiness | FirstName | LastName |
          | United Kingdom | 1      | Kg     | Drop off at a Post Office counter | Letter   | United Kingdom Letter | Test Corp     | 123 Main St      | PATHHEAD      | EH37 5PT          | false           | User1     | Name1    |