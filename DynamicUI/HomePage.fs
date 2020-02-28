namespace DynamicUI

open DynamicUI.Controls
open DynamicUI.Models
open FSharp.Data
open FSharp.Json
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
          MusicDataIsRefreshing: bool
          SearchText: string }

    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music

    let getMusicDataSearchMapper musicEntries =
        match musicEntries with
        | Choice1Of2 musicList ->
            let musicList = Json.deserialize<MusicList> musicList
            MusicLoaded musicList.results
        | Choice2Of2 _ ->
            MusicLoadedError Strings.CommonErrorMessage

    let getMusicDataSearch searchText =
        async {
            //Adding some delay for testing purpose
            do! Async.Sleep 2000
            match searchText with
            | Some searchText ->
                let! musicEntries = Async.Catch(Http.AsyncRequestString(Strings.BaseUrlWithParam searchText))
                return getMusicDataSearchMapper musicEntries
            | None ->
                let! musicEntries = Async.Catch(Http.AsyncRequestString(Strings.BaseUrl))
                return getMusicDataSearchMapper musicEntries
        }

    let init =
        { MusicList = Remote.Loading
          MusicDataIsRefreshing = false
          SearchText = "" }, Cmd.ofMsg MusicLoading

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
            { model with MusicDataIsRefreshing = true }, Cmd.ofAsyncMsg (getMusicDataSearch (Some model.SearchText)),
            ExternalMsg.NoOp

        | MusicTextSearchChanged searchText ->
            { model with SearchText = searchText }, Cmd.ofAsyncMsg (getMusicDataSearch (Some searchText)),
            ExternalMsg.NoOp

    let view model dispatch =
        let searchMusic = MusicTextSearchChanged >> dispatch

        let loadingView =
            View.ActivityIndicator(color = Color.LightBlue, isRunning = true)

        let errorView errorMsg =
            StackLayout.stackLayout
                [ StackLayout.VerticalLayout LayoutOptions.Center
                  StackLayout.Children
                      [ Label.label
                          [ Label.Text errorMsg
                            Label.HorizontalTextAlignment TextAlignment.Center ]

                        Button.button
                            [ Button.Text Strings.TryAgainText
                              Button.OnClick(fun _ -> dispatch RefreshMusicData) ] ] ]

        let emptyView =
            Label.label
                [ Label.Text Strings.EmptyResultMessage
                  Label.HorizontalTextAlignment TextAlignment.Center
                  Label.HorizontalLayout LayoutOptions.Center
                  Label.VerticalLayout LayoutOptions.Center ]

        let rederItem item =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ Image.image
                        [ Image.Source(Image.Path(item.artworkUrl60))
                          Image.MarginTop 16.0
                          Image.HorizontalLayout LayoutOptions.FillAndExpand
                          Image.VerticalLayout LayoutOptions.FillAndExpand ]

                      Label.label
                          [ Label.Text item.artistName
                            Label.HorizontalTextAlignment TextAlignment.Center
                            Label.Margin 16.0 ] ] ]

        let renderEntries items =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ View.UnlinedSearchBar
                        (placeholder = Strings.SearchPlaceHolderMessage,
                         textChanged = debounce 200 (fun args -> args.NewTextValue |> searchMusic),
                         margin = Thickness(8.0, 0.0), keyboard = Keyboard.Text, isSpellCheckEnabled = false)
                      View.RefreshView
                          (CollectionView.collectionView
                              [ CollectionView.SelectionMode SelectionMode.Single
                                CollectionView.MarginLeft 8.0
                                CollectionView.MarginRight 8.0
                                CollectionView.EmptyView emptyView
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
                                                      [ Frame.CornerRadius 4.0
                                                        Frame.HeightRequest 250.0
                                                        Frame.Margin 8.0
                                                        Frame.Content itemlayout ] ] ] ] ],
                           isRefreshing = model.MusicDataIsRefreshing,
                           refreshing = (fun () -> dispatch RefreshMusicData)) ] ]

        let content =
            match model.MusicList with
            | Loading -> loadingView
            | Content(Error errorMsg) -> errorView errorMsg
            | Content(Ok items) -> renderEntries items

        ContentPage.contentPage
            [ ContentPage.Title Strings.HomePageTitle
              ContentPage.Content(content) ]
