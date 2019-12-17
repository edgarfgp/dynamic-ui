namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FSharp.Data

module HomePage =

    type Msg =
        | WelcomeScreen
        | LogOutTapped

    type Model =
        { Title: string }

    type ExternalMsg =
        | NoOp
        | GoToLoginPage

    let init =
        { Title = "Welcome" }

    type MusicData = JsonProvider<"Music.json">

    let getDataFromApple =
        MusicData.GetSample().Results
        |> Seq.filter (fun issue -> issue.Kind = "Jack Johnson & G. Love")

    let update msg model =
        match msg with
        | WelcomeScreen -> { model with Title = model.Title }, Cmd.none, ExternalMsg.NoOp
        | LogOutTapped ->
            model, Cmd.none, ExternalMsg.GoToLoginPage

    let view model dispatch =
        let goToLoginPage = fun () -> dispatch LogOutTapped
        View.ContentPage
            (title = "Home", toolbarItems = [ View.ToolbarItem(text = "+", command = goToLoginPage) ],
             content =
                 View.StackLayout
                     (verticalOptions = LayoutOptions.StartAndExpand, horizontalOptions = LayoutOptions.Center,
                      children =
                          [ View.CollectionView
                              [ View.Label(text = model.Title, margin = Thickness(16.0, 50.0, 16.0, 0.0)) ] ]))
