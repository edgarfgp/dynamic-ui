namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Helpers

module LoginPage =
    /// The messages dispatched by the view
    type Msg =
        | LoginTapped
        | EmailTextChanged of string
        | PasswordTextChanged of string

    type ExternalMsg =
        | NoOp
        | GoToHomePage

    /// The model from which the view is generated
    type Model =
        { Email: string
          Password: string
          IsEmailValid: bool
          isPasswordValid: bool }

    /// Returns the initial state
    let init =
        { Email = "exmaple@email.com"
          Password = "enter a password"
          IsEmailValid = false
          isPasswordValid = false }

    let validateEmail (email: string) = email.Contains("@")
    let validatePassword (password: string) = password.Length > 6

    /// The function to update the view
    let update msg model =
        match msg with
        | EmailTextChanged email ->
            { model with
                  Email = email
                  IsEmailValid = (validateEmail email) }, Cmd.none, ExternalMsg.NoOp
        | PasswordTextChanged password ->
            { model with
                  Password = password
                  isPasswordValid = (validatePassword password) }, Cmd.none, ExternalMsg.NoOp
        | LoginTapped ->
            model, Cmd.none, ExternalMsg.GoToHomePage

    /// The view function giving updated content for the page
    let view model dispatch =
        let updateEmail = EmailTextChanged >> dispatch
        let updatePassword = PasswordTextChanged >> dispatch
        let goToHome = fun () -> dispatch LoginTapped

        View.ContentPage
            (View.ScrollView
                (View.StackLayout
                    [ View.Entry
                        (placeholder = model.Email, horizontalTextAlignment = TextAlignment.Center,
                         textChanged = debounce 250 (fun args -> args.NewTextValue |> updateEmail),
                         margin = Thickness(16.0, 50.0, 16.0, 16.0), height = 50.0, keyboard = Keyboard.Email)

                      View.Entry
                          (placeholder = model.Password, horizontalTextAlignment = TextAlignment.Center,
                           textChanged = debounce 250 (fun args -> args.NewTextValue |> updatePassword),
                           margin = Thickness(16.0, 0.0, 16.0, 16.0), height = 50.0, keyboard = Keyboard.Text)

                      View.Button
                          (text = "Login", margin = Thickness(16.0, 0.0, 16.0, 0.0), command = goToHome,
                           commandCanExecute = (model.IsEmailValid && model.isPasswordValid)) ]))
