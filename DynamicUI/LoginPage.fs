namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module LoginPage =
    type Msg =
        | LoginTapped
        | EmailTextChanged of string
        | PasswordTextChanged of string

    type ExternalMsg =
        | NoOp
        | GoToHomePage

    type Model =
        { Email: string
          Password: string
          IsEmailValid: bool
          isPasswordValid: bool }

    let init =
        { Email = "exmaple@email.com"
          Password = "enter a password"
          IsEmailValid = false
          isPasswordValid = false }

    let validateEmail (email: string) = email.Contains("@")
    //FIXME for testing purpose We will set this > 1. Once we fishing we will set this back to 6
    let validatePassword (password: string) = password.Length > 1

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

    let view model dispatch =
        let updateEmail = EmailTextChanged >> dispatch
        let updatePassword = PasswordTextChanged >> dispatch
        let goToHome = fun () -> dispatch LoginTapped

        View.ContentPage
            (View.ScrollView
                (View.StackLayout

                    [ View.Image
                        (source = Path "https://picsum.photos/id/0/5616/3744",
                         horizontalOptions = LayoutOptions.FillAndExpand, margin = Thickness(16.0, 50.0, 16.0, 16.0),
                         height = 200.0)

                      View.Entry
                          (placeholder = model.Email, horizontalTextAlignment = TextAlignment.Center,
                           textChanged = debounce 250 (fun args -> args.NewTextValue |> updateEmail),
                           margin = Thickness(16.0, 16.0, 16.0, 16.0), height = 50.0, keyboard = Keyboard.Email)

                      View.Entry
                          (placeholder = model.Password, horizontalTextAlignment = TextAlignment.Center,
                           textChanged = debounce 250 (fun args -> args.NewTextValue |> updatePassword),
                           margin = Thickness(16.0, 0.0, 16.0, 16.0), height = 50.0, isPassword = true)

                      View.Button
                          (text = "Login", margin = Thickness(16.0, 0.0, 16.0, 0.0), command = goToHome,
                           commandCanExecute = (model.IsEmailValid && model.isPasswordValid),
                           backgroundColor =
                               (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                else Color.LightGray),
                           borderColor =
                               (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                else Color.Transparent), borderWidth = 1.0) ]))
