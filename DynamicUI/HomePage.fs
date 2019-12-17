namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open FSharp.Data

module HomePage =

    type Msg =
        | GetMusicData
        | LogOutTapped

    type Model =
        { Title: string
          ArtistName: string }

    type MusicData = JsonProvider<"https://itunes.apple.com/search?term=jack+johnson&entity=musicVideo">

    type ExternalMsg =
        | NoOp
        | GoToLoginPage

    let getDataFromApple =
        MusicData.GetSample().Results |> Array.toList

    let init =
        let results = getDataFromApple.Item 0
        { Title = "Welcome"
          ArtistName = results.TrackName }

    let update msg model =
        match msg with
        | GetMusicData ->
            { model with Title = model.Title }, Cmd.none, ExternalMsg.NoOp
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
                                      View.StackLayout
                                          [ View.Label(text = value.ArtistName)
                                            View.Label(text = value.TrackName) ] ]) ]))
