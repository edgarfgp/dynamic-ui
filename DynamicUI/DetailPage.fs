namespace DynamicUI

open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module DetailPage =

    type Msg = ShowInfo

    type Model =
        { Music: Music }

    let init music =
        { Music = music }

    let update msg model =
        match msg with
        | ShowInfo -> model

    let view model _ =

        let detailEntries =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ Image.image
                        [ Image.Source(Image.Path(model.ImageUrl))
                          Image.Height 200.0
                          Image.Margin 16.0 ]

                      Label.label
                          [ Label.Text model.ArtistName
                            Label.Margin 16.0
                            Label.HorizontalTextAlignment TextAlignment.Center ]

                      Label.label
                          [ Label.Text model.Genre
                            Label.Margin 16.0
                            Label.HorizontalTextAlignment TextAlignment.Center ]

                      Label.label
                          [ Label.Text model.TrackName
                            Label.Margin 16.0
                            Label.HorizontalTextAlignment TextAlignment.Center ]

                      Label.label
                          [ Label.Text model.Country
                            Label.Margin 16.0
                            Label.HorizontalTextAlignment TextAlignment.Center ] ] ]

        let content =
            ScrollView.scrollView [ ScrollView.Content detailEntries ]

        ContentPage.contentPage
            [ ContentPage.Title "Song Detail"
              ContentPage.Content content ]
