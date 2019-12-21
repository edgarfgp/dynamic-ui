namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms


module DetailPage =

    type Msg = ShowInfo

    type ExternalMsg = NoOp

    type Model =
        { Music: Music }

    let init music =
        { Music = music }, Cmd.none

    let update msg model =
        match msg with
        | ShowInfo -> model, Cmd.none, ExternalMsg.NoOp

    let view model _ =
        View.ContentPage
            (title = "Song Detail",
             content =
                 View.ScrollView
                     (View.StackLayout
                         (children =
                             [ View.Image(source = Path model.ImageUrl, height = 200.0, margin = Thickness(16.0))
                               View.Label
                                   (text = model.ArtistName, margin = Thickness(16.0),
                                    horizontalTextAlignment = TextAlignment.Center)
                               View.Label
                                   (text = model.Genre, margin = Thickness(16.0),
                                    horizontalTextAlignment = TextAlignment.Center)
                               View.Label
                                   (text = model.TrackName, margin = Thickness(16.0),
                                    horizontalTextAlignment = TextAlignment.Center)
                               View.Label
                                   (text = model.Country, margin = Thickness(16.0),
                                    horizontalTextAlignment = TextAlignment.Center) ])))
