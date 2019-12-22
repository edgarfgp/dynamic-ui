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
    let validatePassword (password: string) = password.Length > 6

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

        let loginEntries =
            StackLayout.stackLayout
                [ StackLayout.Children
                    [ Image.image
                        [ Image.Source(Image.Path("https://picsum.photos/id/0/5616/3744"))
                          Image.HorizontalLayout LayoutOptions.FillAndExpand
                          Image.MarginLeft 16.0
                          Image.MarginRight 16.0
                          Image.MarginTop 80.0
                          Image.Height 200.0 ]

                      TextEntry.textEntry
                          [ TextEntry.Placeholder model.Email
                            TextEntry.HorizontalTextAlignment TextAlignment.Center
                            TextEntry.OnTextChanged(debounce 250 (fun args -> args.NewTextValue |> updateEmail))
                            TextEntry.MarginLeft 16.0
                            TextEntry.MarginRight 16.0
                            TextEntry.MarginTop 16.0
                            TextEntry.Height 50.0
                            TextEntry.Keyboard Keyboard.Email ]

                      TextEntry.textEntry
                          [ TextEntry.Placeholder model.Password
                            TextEntry.HorizontalTextAlignment TextAlignment.Center
                            TextEntry.OnTextChanged(debounce 250 (fun args -> args.NewTextValue |> updatePassword))
                            TextEntry.MarginLeft 16.0
                            TextEntry.MarginRight 16.0
                            TextEntry.MarginTop 16.0
                            TextEntry.Height 50.0
                            TextEntry.IsPassword true ]

                      Button.button
                          [ Button.Text "Login"
                            Button.Margin 16.0
                            Button.OnClick goToHome
                            Button.CanExecute(model.IsEmailValid && model.isPasswordValid)
                            Button.BackgroundColor
                                (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                 else Color.LightGray)
                            Button.BorderColor
                                (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                 else Color.Transparent)
                            Button.BorderWidth 1.0 ] ] ]

        let mainLayout =
            StackLayout.stackLayout
                [ StackLayout.Children [ ScrollView.scrollView [ ScrollView.Content loginEntries ] ] ]

        ContentPage.contentPage
            [ ContentPage.HasNavigationBar false
              ContentPage.Content mainLayout ]
