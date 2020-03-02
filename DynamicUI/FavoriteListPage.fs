namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module FavoriteListPage =

    type Msg = GoToDetailPage of Music

    type ExternalMsg =
        | NoOp
        | NavigateToDetailPage of Music

    type Model =
        { music: Music }

    let init =
        { music = Music.CreateMusic() }

    let update msg model =
        match msg with
        | GoToDetailPage music ->
            { model with music = music }, Cmd.none, ExternalMsg.NoOp

    let view _ _ =

        let content =
            View.FlexLayout
                (margin = Thickness(32., 0., 32., 0.), direction = FlexDirection.Column,
                 justifyContent = FlexJustify.SpaceAround, children = [], backgroundColor = Color.Red)

        View.ContentPage(hasNavigationBar = false, content = content)
