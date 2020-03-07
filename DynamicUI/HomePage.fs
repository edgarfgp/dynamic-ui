namespace DynamicUI

open DynamicUI
open DynamicUI.Models
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module Extensions =
    let rec filterMusic predicate musicList =
        match musicList with
        | x :: xs when predicate x -> x :: (filterMusic predicate xs)
        | _ :: xs -> filterMusic predicate xs
        | [] -> []

module HomePage =
    open Extensions

    type Msg =
        | Loading
        | Refresh
        | LoadingError of string
        | Loaded of Music list
        | GoToDetailPage of Music
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
            LoadingError "An error has occurred"
        | Ok musicEntries ->
            mutableMusicList <- musicEntries
            Loaded musicEntries

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
                    return Loaded result
            | _ ->
                match mutableMusicList with
                | [] ->
                    let! musicEntries = NetworkService.getMusicDataSearch None
                    return (searchResult musicEntries)
                | _ ->
                    return Loaded mutableMusicList
        }

    let init =
        { MusicList = Remote.LoadingState
          MusicDataIsRefreshing = false
          SearchText = "" }, Cmd.ofMsg Loading

    let update msg model =
        match msg with
        | Loading ->
            { model with MusicList = LoadingState }, Cmd.ofAsyncMsg (filterOrFetchMusicData None), ExternalMsg.NoOp

        | Loaded data ->
            { model with
                  MusicList = Content(Ok data)
                  MusicDataIsRefreshing = false }, Cmd.none, ExternalMsg.NoOp

        | LoadingError error ->
            { model with MusicList = Content(Error error) }, Cmd.none, ExternalMsg.NoOp

        | GoToDetailPage music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music

        | Refresh ->
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
                       View.Button(text = "Try again", command = (fun _ -> dispatch Refresh)) ])

        let emptyView =
            View.Label
                (text = "No results available for the current search", horizontalTextAlignment = TextAlignment.Center,
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
                        (placeholder = "Enter a valid artist",
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
                           refreshing = (fun () -> dispatch Refresh)) ])

        let content =
            match model.MusicList with
            | LoadingState -> loadingView
            | Content(Error errorMsg) -> errorView errorMsg
            | Content(Ok items) -> renderEntries items

        View.ContentPage(title = "Home", content = content)
