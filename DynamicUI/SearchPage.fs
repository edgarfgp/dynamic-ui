namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module SearchPage =

    type Msg =
        | GoToFollowerLisPage of string

    type ExternalMsg =
        | NoOp
        | NavigateToFollowersList of string

    type Model =
        { UserName : string }

    let init =
        { UserName = "" }, Cmd.none

    let update msg model =
        match msg with
        | GoToFollowerLisPage userName ->
            { model with UserName = userName }, Cmd.none, ExternalMsg.NoOp

    let view _ _ =

        let content =
            View.FlexLayout(
                margin = Thickness(32., 0., 32., 0.),
                direction = FlexDirection.Column,
                justifyContent = FlexJustify.SpaceAround,
                children = [

                    View.Image(
                        source = Path "https://www.freeiconspng.com/img/38981",
                        height = 180.,
                        backgroundColor = Color.Blue
                    )

                    View.Entry(
                        placeholder = "Enter a username",
                        height = 40.,
                        clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                        returnType = ReturnType.Go)

                    View.Button(
                        text = "Search",
                        textColor = Color.White,
                        backgroundColor = Color.ForestGreen,
                        cornerRadius = 8)
                ]
            )

        View.ContentPage(
            hasNavigationBar = false,
            content = content)

