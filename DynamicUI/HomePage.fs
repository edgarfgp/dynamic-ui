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

    let init =
        { MusicList = getArtistData }

    let update msg model =
        match msg with
        | MusicSelected music ->
            model, Cmd.none, ExternalMsg.NavigateToDetail music

    let view model dispatch =
        let tapGestureRecognizer msg =
            TapGestureRecognizer.tapGestureRecognizer [ TapGestureRecognizer.OnTapped(fun () -> dispatch msg) ]

        let rederItemlayout item =
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
                let itemlayout = rederItemlayout item
                StackLayout.stackLayout
                    [ StackLayout.GestureRecognizers [ tapGestureRecognizer (MusicSelected item) ]
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
                    [ View.CollectionView(items = renderEntries model.MusicList, selectionMode = SelectionMode.Single) ] ]

        ContentPage.contentPage
            [ ContentPage.Title "Home"
              ContentPage.Content content ]
