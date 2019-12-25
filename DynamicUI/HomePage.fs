namespace DynamicUI

open DynamicUI.Models
open FSharp.Data
open FSharp.Data.Runtime.StructuralInference
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module HomePage =
    type Msg = MusicSelected of Music

    type Model =
        { MusicList: Music list }

    [<Literal>]
    let url = @"https://itunes.apple.com/search?term="""

    type MusicData = JsonProvider<url>

    //Update function that takes a message and a model and give us back a new Model
    type ExternalMsg =
        | NoOp
        | NavigateToDetail of Music

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

                    return result

                | Choice2Of2 _ -> return []
             })

    let init =
        { MusicList = getArtistData }

    let update msg model =
        match msg with
        | MusicSelected music ->
            model, ExternalMsg.NavigateToDetail music

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
                    [ View.CollectionView(items = renderEntries model.MusicList, selectionMode = SelectionMode.Single) ] ]

        ContentPage.contentPage
            [ ContentPage.Title "Home"
              ContentPage.Content(content) ]
