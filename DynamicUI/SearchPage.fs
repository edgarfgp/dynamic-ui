namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module SearchPage =

    type Msg =
        | UserEntered of string

    type ExternalMsg =
        | NoOp

    type Model =
        { UserName : string }

    let init =
        {UserName = "" }, Cmd.none

    let getUserInfo userName =
        match (NetworkService.getUserInfo userName) with
        | Ok _ ->
           ()
        | Error _ ->
           ()

    let update msg model =
        match msg with
        | UserEntered userName ->
            { model with UserName = userName  }, Cmd.none, ExternalMsg.NoOp

    let view _ dispatch =

        let userEntered = UserEntered >> dispatch

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
                        completed = (fun args -> args |> userEntered),
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

