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
        | MusicTextSearchChanged of string

    type Model =
        { MusicList: Remote<Result<Music list, string>>
          MusicDataIsRefreshing: bool }

    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music

    let parseMusic musicList =
        (JsonProvider<Strings.BaseUrl>.Parse musicList).Results
        |> Array.toList
        |> List.map (fun c ->
            { ImageUrl = c.ArtworkUrl60
              ArtistName = c.ArtistName
              Genre = c.PrimaryGenreName
              TrackName = (string) c.TrackName
              Country = c.Country })

    let result musicEntries =
        match musicEntries with
        | Choice1Of2 musicList ->
            MusicLoaded(parseMusic musicList)
        | Choice2Of2 _ ->
            MusicLoadedError Strings.Common_ErrorMessage

    let getMusicDataSearch searchText =
        async {
            match searchText with
            | Some searchText ->
                let! musicEntries = Async.Catch(Http.AsyncRequestString(Strings.BaseUrlWithParam searchText))
                return (result musicEntries)
            | None ->
                let! musicEntries = Async.Catch(Http.AsyncRequestString(Strings.BaseUrl))
                return (result musicEntries)
        }

    let init =
        { MusicList = Remote.Loading
          MusicDataIsRefreshing = false }, Cmd.ofMsg MusicLoading

    let update msg model =
        match msg with
        | MusicLoading ->
            { model with MusicList = Loading }, Cmd.ofAsyncMsg (getMusicDataSearch None), ExternalMsg.NoOp

        | MusicLoaded data ->
            { model with
                  MusicList = Content(Ok data)
                  MusicDataIsRefreshing = false }, Cmd.none, ExternalMsg.NoOp

        | MusicLoadedError error ->
            { model with MusicList = Content(Error error) }, Cmd.none, ExternalMsg.NoOp

        | GoToDetailPage music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music

        | RefreshMusicData ->
            { model with MusicDataIsRefreshing = true }, Cmd.ofAsyncMsg (getMusicDataSearch None), ExternalMsg.NoOp

        | MusicTextSearchChanged searchText ->
            model, Cmd.ofAsyncMsg (getMusicDataSearch (Some searchText)), ExternalMsg.NoOp

    let view model dispatch =
        let searchMusic = MusicTextSearchChanged >> dispatch

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
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ View.SearchBar
                        (placeholder = Strings.SearchPlaceHolderMessage,
                         textChanged = debounce 250 (fun args -> args.NewTextValue |> searchMusic))
                      View.RefreshView
                          (CollectionView.collectionView
                              [ CollectionView.SelectionMode SelectionMode.Single
                                CollectionView.Items
                                    [ for item in items ->
                                        let itemlayout = rederItem item
                                        StackLayout.stackLayout
                                            [ StackLayout.GestureRecognizers
                                                [ TapGestureRecognizer.tapGestureRecognizer
                                                    [ TapGestureRecognizer.OnTapped
                                                        (fun () -> dispatch (GoToDetailPage item)) ] ]
                                              StackLayout.Children
                                                  [ Frame.frame
                                                      [ Frame.CornerRadius 5.0
                                                        Frame.HeightRequest 250.0
                                                        Frame.Margin 8.0
                                                        Frame.Content itemlayout ] ] ] ] ],
                           isRefreshing = model.MusicDataIsRefreshing,
                           refreshing = (fun () -> dispatch RefreshMusicData)) ] ]

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
            [ ContentPage.Title Strings.HomePageTitle
              ContentPage.Content(content) ]