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
        let tapGestureRecognizer msg = View.TapGestureRecognizer(command = fun () -> dispatch msg)
        View.ContentPage
            (title = "Home",
             content =
                 View.StackLayout
                     (children =
                         [ View.CollectionView
                             (items =
                                 [ for index in 0 .. model.MusicList.Length - 1 ->
                                     let item = model.MusicList.Item index
                                     View.StackLayout
                                         (children =
                                             [ View.Frame
                                                 (content =
                                                     View.StackLayout
                                                         [ View.Image
                                                             (source = Path item.ImageUrl,
                                                              horizontalOptions = LayoutOptions.FillAndExpand,
                                                              verticalOptions = LayoutOptions.FillAndExpand)

                                                           View.Label
                                                               (text = item.ArtistName,
                                                                horizontalTextAlignment = TextAlignment.Center,
                                                                margin = Thickness(16.0)) ], margin = Thickness(8.0),
                                                  cornerRadius = 5.0, height = 250.0,
                                                  gestureRecognizers = [ tapGestureRecognizer (MusicSelected item) ]) ]) ],
                              emptyView = View.Label(text = "There is not information for now..."),
                              selectionMode = SelectionMode.Single) ]))
