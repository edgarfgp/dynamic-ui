namespace DynamicUI

open DynamicUI.Controls
open DynamicUI.Models
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module Extensions =
    let rec filterMusic predicate musicList: Music list =
        match musicList with
        | x :: xs when predicate x -> x :: (filterMusic predicate xs)
        | _ :: xs -> filterMusic predicate xs
        | [] -> []

module HomePage =
    open Extensions

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

    let mutable mutableMusicList: Music list = []

    let searchResult musicEntries =
        match musicEntries with
        | Error _ ->
            MusicLoadedError Strings.CommonErrorMessage
        | Ok musicEntries ->
            mutableMusicList <- musicEntries
            printfn "------Using API----"
            MusicLoaded musicEntries

    let filterOrFetchMusicData searchText =
        async {
            match searchText with
            | Some text when text <> "" ->
                let filterCondition = (fun c -> c.artistName.ToLower().Contains(text.ToLower()))
                let result = filterMusic filterCondition mutableMusicList
                match result with
                | [] ->
                    let! musicEntries = NetworkService.getMusicDataSearch (Some text)
                    return (searchResult musicEntries)
                | _ ->
                    return MusicLoaded result
            | _ ->
                match mutableMusicList with
                | [] ->
                    let! musicEntries = NetworkService.getMusicDataSearch None
                    return (searchResult musicEntries)
                | _ ->
                    return MusicLoaded mutableMusicList
        }

    let init =
        { MusicList = Remote.Loading
          MusicDataIsRefreshing = false
          SearchText = "" }, Cmd.ofMsg MusicLoading

    let update msg model =
        match msg with
        | MusicLoading ->
            { model with MusicList = Loading }, Cmd.ofAsyncMsg (filterOrFetchMusicData None), ExternalMsg.NoOp

        | MusicLoaded data ->
            { model with
                  MusicList = Content(Ok data)
                  MusicDataIsRefreshing = false }, Cmd.none, ExternalMsg.NoOp

        | MusicLoadedError error ->
            { model with MusicList = Content(Error error) }, Cmd.none, ExternalMsg.NoOp

        | GoToDetailPage music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music

        | RefreshMusicData ->
            { model with MusicDataIsRefreshing = true }, Cmd.ofAsyncMsg (filterOrFetchMusicData None), ExternalMsg.NoOp

        | MusicTextSearchChanged searchText ->
            { model with SearchText = searchText }, Cmd.ofAsyncMsg (filterOrFetchMusicData (Some searchText)),
            ExternalMsg.NoOp

    let view model dispatch =
        let searchMusic = MusicTextSearchChanged >> dispatch

        let loadingView =
            View.ActivityIndicator(color = Color.LightBlue, isRunning = true)

        let errorView errorMsg =
            View.StackLayout
                (verticalOptions = LayoutOptions.Center,
                 children =
                     [ View.Label(text = errorMsg, horizontalTextAlignment = TextAlignment.Center)
                       View.Button(text = Strings.TryAgainText, command = (fun _ -> dispatch RefreshMusicData)) ])

        let emptyView =
            View.Label
                (text = Strings.EmptyResultMessage, horizontalTextAlignment = TextAlignment.Center,
                 horizontalOptions = LayoutOptions.Center, verticalOptions = LayoutOptions.Center)

        let rederItem item =
            View.StackLayout
                (children =
                    [ View.Image
                        (source = Path item.artworkUrl60, margin = Thickness(16.),
                         horizontalOptions = LayoutOptions.FillAndExpand, verticalOptions = LayoutOptions.FillAndExpand)

                      View.Label
                          (text = item.artistName, horizontalTextAlignment = TextAlignment.Center,
                           margin = Thickness(16.)) ])

        let renderEntries items =
            View.StackLayout
                (children =
                    [ View.UnlinedSearchBar
                        (placeholder = Strings.SearchPlaceHolderMessage,
                         textChanged = debounce 200 (fun args -> args.NewTextValue |> searchMusic),
                         margin = Thickness(8.0, 0.0), keyboard = Keyboard.Text, isSpellCheckEnabled = false)

                      View.RefreshView
                          (content =
                              View.CollectionView
                                  (selectionMode = SelectionMode.Single, margin = Thickness(8., 0., 8., 0.),
                                   emptyView = emptyView,
                                   items =
                                       [ for item in items ->
                                           let itemlayout = rederItem item

                                           View.StackLayout
                                               (gestureRecognizers =
                                                   [ View.TapGestureRecognizer
                                                       (command = fun () -> dispatch (GoToDetailPage item)) ],
                                                children =
                                                    [ View.Frame
                                                        (cornerRadius = 4., height = 250., margin = Thickness(8.),
                                                         content = itemlayout) ]) ]),
                           isRefreshing = model.MusicDataIsRefreshing,
                           refreshing = (fun () -> dispatch RefreshMusicData)) ])

        let content =
            match model.MusicList with
            | Loading -> loadingView
            | Content(Error errorMsg) -> errorView errorMsg
            | Content(Ok items) -> renderEntries items

        View.ContentPage(title = Strings.HomePageTitle, content = content)
