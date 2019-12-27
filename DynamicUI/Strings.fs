namespace DynamicUI

[<RequireQualifiedAccess>]
module Strings =

    [<Literal>]
    let BaseUrl = @"https://itunes.apple.com/search?term="""

    let Common_ErrorMessage = "An error has occured. Please try again later."

    let BaseUrlWithParam = sprintf @"https://itunes.apple.com/search?term=""%s"

    let SearchPlaceHolderMessage = "Enter a valid artist"

    let HomePageTitle = "Home"
    let DetailpageTitle = "Music Detail"
