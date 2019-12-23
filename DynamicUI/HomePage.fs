namespace DynamicUI

open DynamicUI.Models
open FSharp.Data
open FSharp.Data.Runtime.StructuralInference
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module HomePage =
    type Msg =
        | MusicSelected of Music
        | AboutTapped
        | LoginTapped
        | RefreshViewRefreshing
        | RefreshViewRefreshDone of Music list

    type Model =
        { MusicList: Music list
          IsRefreshingData: bool }

    [<Literal>]
    let url = @"https://itunes.apple.com/search?term="""

    type MusicData = JsonProvider<url>

    //Update function that takes a message and a model and give us back a new Model
    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music
        | NavigateToAbout
        | NavigateToLogin

    let getArtistData =
        MusicData.GetSample().Results
        |> Array.toList
        |> List.map (fun c ->
            { ImageUrl = c.ArtworkUrl60
              ArtistName = c.ArtistName
              Genre = c.PrimaryGenreName
              TrackName = (string) c.TrackName
              Country = c.Country })

    let getMusicData =
        Cmd.ofAsyncMsg
            (async {
                do! Async.Sleep 2000
                let! blogEntries = Async.Catch(Http.AsyncRequestString(url))
                match blogEntries with
                | Choice1Of2 musicList ->
                    let result =
                        (JsonProvider<url>.Parse musicList).Results
                        |> Array.toList
                        |> List.map (fun c ->
                            { ImageUrl = c.ArtworkUrl60
                              ArtistName = c.ArtistName
                              Genre = c.PrimaryGenreName
                              TrackName = (string) c.TrackName
                              Country = c.Country })

                    return RefreshViewRefreshDone result

                | Choice2Of2 _ -> return RefreshViewRefreshing
             })

    let init =
        { MusicList = getArtistData
          IsRefreshingData = false }

    let update msg model =
        match msg with
        | MusicSelected music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music
        | AboutTapped ->
            model, Cmd.none, ExternalMsg.NavigateToAbout
        | LoginTapped ->
            model, Cmd.none, ExternalMsg.NavigateToLogin
        | RefreshViewRefreshing ->
            { model with IsRefreshingData = true }, getMusicData, ExternalMsg.NoOp
        | RefreshViewRefreshDone music ->
            { model with
                  IsRefreshingData = false
                  MusicList = music }, Cmd.none, ExternalMsg.NoOp

    //View that takes a model and update the view if needed
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
            [ for item in items ->
                let itemlayout = rederItem item
                StackLayout.stackLayout
                    [ StackLayout.GestureRecognizers
                        [ TapGestureRecognizer.tapGestureRecognizer
                            [ TapGestureRecognizer.OnTapped(fun () -> dispatch (MusicSelected item)) ] ]
                      StackLayout.Children
                          [ Frame.frame
                              [ Frame.CornerRadius 5.0
                                Frame.HeightRequest 250.0
                                Frame.Margin 8.0
                                Frame.Content itemlayout ] ] ] ]

        let content =
            StackLayout.stackLayout
                [ StackLayout.Children
                    //Can not use the Simplified version of CollectionView due to a exception
                    [ View.RefreshView
                        (content =
                            View.CollectionView
                                (items = renderEntries model.MusicList, selectionMode = SelectionMode.Single),
                         isRefreshing = model.IsRefreshingData, refreshing = (fun () -> dispatch RefreshViewRefreshing)) ] ]

        ContentPage.contentPage
            [ ContentPage.Title "Home"
              ContentPage.ToolbarItems
                  [ View.ToolbarItem(text = "About", command = (fun () -> dispatch AboutTapped))
                    View.ToolbarItem(text = "Log in", command = (fun () -> dispatch LoginTapped)) ]
              ContentPage.Content(content) ]
