namespace DynamicUI

open Fabulous.XamarinForms
open Xamarin.Forms

module SurveyPage =

    type Msg = LoadigSurvey

    type Model =
        { Section: string list }

    let init = { Section = [] }

    let update msg model =
        match msg with
        | LoadigSurvey -> model

    let view model _ =

        let detailEntries =
            View.StackLayout
                (children =
                    [
                        View.Label("dadasdasdasds")
                    ])

        let content = View.ScrollView(content = detailEntries)

        View.ContentPage(title = "Detail", content = content)