namespace DynamicUI

open DynamicUI.Models
open FSharp.Data
open FSharp.Data.Runtime.StructuralInference
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module HomePage =
    type Msg =
        | MusicLoading
        | MusicLoaded of Music list
        | MusicLoadedError of string
        | GoToDetailPage of Music
        | RefreshMusicData

    type Model =
        { MusicList: Remote<Result<Music list, string>>
          MusicDataIsRefreshing: bool }

    [<Literal>]
    let url = @"https://itunes.apple.com/search?term="""

    type MusicData = JsonProvider<url>

    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music

    let getMusicData (delay: int option) =
        async {
            match delay with
            | Some delay -> do! Async.Sleep delay
            | None -> do! Async.Sleep 0

            let! result = MusicData.AsyncGetSample()
            return if result.ResultCount > 0 then
                       let music =
                           result.Results
                           |> Array.toList
                           |> List.map (fun c ->
                               { ImageUrl = c.ArtworkUrl60
                                 ArtistName = c.ArtistName
                                 Genre = c.PrimaryGenreName
                                 TrackName = (string) c.TrackName
                                 Country = c.Country })
                       MusicLoaded music
                   else
                       MusicLoadedError "Error getting data from iTunes"
        }

    let init =
        { MusicList = Remote.Loading
          MusicDataIsRefreshing = false }, Cmd.ofMsg MusicLoading

    let update msg model =
        match msg with
        | MusicLoading ->
            { model with MusicList = Loading }, Cmd.ofAsyncMsg (getMusicData None), ExternalMsg.NoOp

        | MusicLoaded data ->
            { model with
                  MusicList = Content(Ok data)
                  MusicDataIsRefreshing = false }, Cmd.none, ExternalMsg.NoOp

        | MusicLoadedError error ->
            { model with MusicList = Content(Error error) }, Cmd.none, ExternalMsg.NoOp

        | GoToDetailPage music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music

        | RefreshMusicData ->
            { model with MusicDataIsRefreshing = true }, Cmd.ofAsyncMsg (getMusicData (Some 3000)), ExternalMsg.NoOp

    let view model dispatch =

        let rederItem item =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ Image.image
                        [ Image.Source(Image.Path(item.ImageUrl))
                          Image.MarginTop 16.0
                          Image.HorizontalLayout LayoutOptions.FillAndExpand
                          Image.VerticalLayout LayoutOptions.FillAndExpand ]

                      Label.label
                          [ Label.Text item.ArtistName
                            Label.HorizontalTextAlignment TextAlignment.Center
                            Label.Margin 16.0 ] ] ]

        let renderEntries items =
            View.RefreshView
                (View.CollectionView
                    (items =
                        [ for item in items ->
                            let itemlayout = rederItem item
                            StackLayout.stackLayout
                                [ StackLayout.GestureRecognizers
                                    [ TapGestureRecognizer.tapGestureRecognizer
                                        [ TapGestureRecognizer.OnTapped(fun () -> dispatch (GoToDetailPage item)) ] ]
                                  StackLayout.Children
                                      [ Frame.frame
                                          [ Frame.CornerRadius 5.0
                                            Frame.HeightRequest 250.0
                                            Frame.Margin 8.0
                                            Frame.Content itemlayout ] ] ] ], selectionMode = SelectionMode.Single),
                 isRefreshing = model.MusicDataIsRefreshing, refreshing = (fun () -> dispatch RefreshMusicData))

        let loadingView =
            View.ActivityIndicator(color = Color.LightBlue, isRunning = true)

        let errorView errorMsg =
            Label.label
                [ Label.Text errorMsg
                  Label.HorizontalTextAlignment TextAlignment.Center
                  Label.HorizontalLayout LayoutOptions.Center
                  Label.VerticalLayout LayoutOptions.Center ]

        let content =
            match model.MusicList with
            | Loading -> loadingView
            | Content(Error errorMsg) -> errorView errorMsg
            | Content(Ok items) -> renderEntries items

        ContentPage.contentPage
            [ ContentPage.Title "Home"
              ContentPage.Content(content) ]
