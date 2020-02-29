namespace DynamicUI

open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module DetailPage =

    type Msg = MusicDetailInfo

    type Model =
        { Music: Music }

    let init music =
        { Music = music }

    let update msg model =
        match msg with
        | MusicDetailInfo -> model

    let view model _ =

        let detailEntries =
            View.StackLayout(
                children =
                    [
                        View.Image(
                            source = (Image.Path(model.artworkUrl60)),
                            height = 200.0,
                            margin = Thickness(16.))
                        
                        View.Label(
                            text = model.primaryGenreName,
                            margin = Thickness(16.),
                            horizontalTextAlignment = TextAlignment.Center)
                        
                        View.Label(
                            text = model.artistName,
                            margin = Thickness(16.),
                            horizontalTextAlignment = TextAlignment.Center)
                    ]
                )
            
        let content =
            View.ScrollView(content = detailEntries)

        View.ContentPage(
              title = Strings.DetailpageTitle,
              content = content)
