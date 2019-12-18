namespace DynamicUI

open FSharp.Data
open FSharp.Data.Runtime.StructuralInference
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module HomePage =
    type Music =
        { ImageUrl: string
          ArtistName: string }

    type Msg =
        | LogOutTapped

    type Model =
        { MusicList: Music list }

    [<Literal>]
    let musicApiUrl = @"https://itunes.apple.com/search?term="""

    type MusicData = JsonProvider<musicApiUrl>

    type ExternalMsg =
        | NoOp
        | GoToLoginPage

    let getDataFromApple =
        MusicData.GetSample().Results
        |> Array.toList
        |> List.map (fun c ->
            { ImageUrl = c.ArtworkUrl60
              ArtistName = c.ArtistName })

    let init =
        { MusicList = getDataFromApple }

    let update msg model =
        match msg with
        | LogOutTapped ->
            model, Cmd.none, ExternalMsg.GoToLoginPage

    let view model dispatch =
        let goToLoginPage = fun () -> dispatch LogOutTapped
        View.ContentPage
            (title = "Home", toolbarItems = [ View.ToolbarItem(text = "Log out", command = goToLoginPage) ],
             content =
                 View.StackLayout
                     (children =
                         [ View.CarouselView
                             (items =
                                 [ for index in 0 .. model.MusicList.Length - 1 ->
                                     View.StackLayout
                                         (children =
                                             [ View.Frame
                                                 (content =
                                                     View.StackLayout
                                                         [ View.Image
                                                             (source = Path (model.MusicList.Item index).ImageUrl,
                                                              horizontalOptions = LayoutOptions.FillAndExpand,
                                                              verticalOptions = LayoutOptions.FillAndExpand)
                                                           View.Label
                                                               (text = (model.MusicList.Item index).ArtistName,
                                                                horizontalTextAlignment = TextAlignment.Center,
                                                                margin = Thickness(16.0)) ], margin = Thickness(8.0),
                                                  cornerRadius = 5.0, hasShadow = true,
                                                  verticalOptions = LayoutOptions.CenterAndExpand, height = 250.0) ]) ],
                              peekAreaInsets = Thickness(10.0),
                              emptyView = View.Label(text = "There is not information for now...")) ]))
