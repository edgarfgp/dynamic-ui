namespace DynamicUI

[<RequireQualifiedAccess>]
module Strings =

    let BaseUrl = @"https://itunes.apple.com/search?term="""

    let CommonErrorMessage = "An error has occurred."

    let BaseUrlWithParam = sprintf @"https://itunes.apple.com/search?term=""%s"

    let SearchPlaceHolderMessage = "Enter a valid artist"

    let HomePageTitle = "Home"
    let DetailpageTitle = "Music Detail"

    let EmptyResultMessage = "No results available for the current search"

    let TryAgainText = "Try again"
