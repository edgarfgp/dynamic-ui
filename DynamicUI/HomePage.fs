namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FSharp.Data
open Xamarin.Forms

module HomePage =

    type Msg = LogOutTapped

    type Model =
        { Title: string }

    type MusicData = JsonProvider<"https://itunes.apple.com/search?term=jack+johnson&entity=musicVideo">

    type ExternalMsg =
        | NoOp
        | GoToLoginPage

    let getDataFromApple =
        MusicData.GetSample().Results |> Array.toList

    let init =
        { Title = "Welcome" }

    let update msg model =
        match msg with
        | LogOutTapped ->
            model, Cmd.none, ExternalMsg.GoToLoginPage

    let view model dispatch =
        let goToLoginPage = fun () -> dispatch LogOutTapped
        View.ContentPage
            (title = "Home", toolbarItems = [ View.ToolbarItem(text = "X", command = goToLoginPage) ],
             content =
                 View.StackLayout
                     (verticalOptions = LayoutOptions.StartAndExpand, horizontalOptions = LayoutOptions.Center,
                      children =
                          [ View.CollectionView
                              (items =
                                  [ for index in 0 .. getDataFromApple.Length - 1 ->
                                      let value = getDataFromApple.Item index
                                      View.Label(text = value.TrackName, margin = Thickness(16.0, 8.0, 16.0, 8.0)) ]) ]))
