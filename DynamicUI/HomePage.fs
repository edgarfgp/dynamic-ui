namespace DynamicUI

open DynamicUI
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.InputTypes.Image
open Xamarin.Forms

module HomePage =
    type Msg =
        | Loading
        | Loaded of Music list
        | LoadingError of string
        | MusicTextSearchChanged of string
        | GoToDetailPage of Music

    type Model =
        { Music: Remote<Result<Music list, string>>
          IsRefreshing: bool
          Text: string option }

    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music

    let mutable musicList: Music list = []

    let fetchMusic =
        async {
            let! music = NetworkService.fetchMusic

            match music with
            | Ok music ->
                musicList <- music
                return Loaded music
            | Error _ -> return LoadingError "An error has occurred"
        }

    let filterMusic (text: string) =
        let music =
            musicList
            |> List.filter (fun c -> c.artistName.ToLower().Contains(text.ToLower()))

        match music with
        | [] -> LoadingError "No result available"
        | ms -> Loaded ms

    let init =
        { Music = Remote.LoadingState
          IsRefreshing = false
          Text = None },
        Cmd.ofMsg Loading

    let update msg model =
        match msg with
        | Loading -> { model with Music = LoadingState }, Cmd.ofAsyncMsg fetchMusic, ExternalMsg.NoOp
        | Loaded data ->
            { model with
                  Music = Content(Ok data)
                  IsRefreshing = false },
            Cmd.none,
            ExternalMsg.NoOp
        | LoadingError error ->
            { model with
                  Music = Content(Error error) },
            Cmd.none,
            ExternalMsg.NoOp
        | GoToDetailPage music -> model, Cmd.none, ExternalMsg.NavigateToDetail music
        | MusicTextSearchChanged text -> { model with Text = Some text }, Cmd.ofMsg (filterMusic text), ExternalMsg.NoOp

    let view model dispatch =
        let loadingView =
            View.ActivityIndicator
                (color = Color.LightBlue, isRunning = true, verticalOptions = LayoutOptions.CenterAndExpand)

        let emptyView error =
            View.Label
                (text = error,
                 horizontalTextAlignment = TextAlignment.Center,
                 horizontalOptions = LayoutOptions.Center,
                 verticalOptions = LayoutOptions.Center)

        let searchBarView =
            View.SearchBar
                (placeholder = "Enter a valid artist",
                 textChanged =
                     debounce 200 (fun args ->
                         args.NewTextValue
                         |> MusicTextSearchChanged
                         |> dispatch),
                 margin = Thickness(8.0, 0.0),
                 keyboard = Keyboard.Text,
                 isSpellCheckEnabled = false)

        let renderItem item =
            dependsOn (item.artworkUrl60, item.artistName) (fun _ (artworkUrl60, artistName) ->
                View.StackLayout
                    (children =
                        [ View.Image
                            (source = ImagePath artworkUrl60,
                             margin = Thickness(16.),
                             horizontalOptions = LayoutOptions.FillAndExpand,
                             verticalOptions = LayoutOptions.FillAndExpand)

                          View.Label
                              (text = artistName,
                               horizontalTextAlignment = TextAlignment.Center,
                               margin = Thickness(16.)) ]))

        let renderEntries model =
            View.CollectionView
                (selectionMode = SelectionMode.Single,
                 margin = Thickness(8., 0., 8., 0.),
                 items =
                     [ for item in model ->
                         let itemLayout = renderItem item
                         View.StackLayout
                             (gestureRecognizers =
                                 [ View.TapGestureRecognizer(command = fun () -> dispatch (GoToDetailPage item)) ],
                              children =
                                  [ View.Frame
                                      (cornerRadius = 4., height = 250., margin = Thickness(8.), content = itemLayout) ]) ])

        let pageContent =
            match model.Music with
            | LoadingState -> loadingView
            | Content (Error error) -> emptyView error
            | Content (Ok music) -> renderEntries music

        View.ContentPage(title = "Home", content = View.StackLayout(children = [ searchBarView; pageContent ]))
