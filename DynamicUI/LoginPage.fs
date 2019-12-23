namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module LoginPage =
    type Msg =
        | LoginTapped
        | EmailTextChanged of string
        | PasswordTextChanged of string

    //ExternalMsg exposes the Action that will be handleExternalMsg int DynamicUI.fs
    type ExternalMsg =
        | NoOp
        | NavigateToHomePage

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
    let validatePassword (password: string) = password.Length > 0

    //Update function that takes a message and a model and give us back a new Model
    let update msg model =
        match msg with
        | EmailTextChanged email ->
            { model with
                  Email = email
                  IsEmailValid = validateEmail email }, ExternalMsg.NoOp
        | PasswordTextChanged password ->
            { model with
                  Password = password
                  isPasswordValid = validatePassword password }, ExternalMsg.NoOp
        | LoginTapped ->
            model, ExternalMsg.NavigateToHomePage

    //View that takes a model and update the view if needed
    let view model dispatch =

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
                            TextEntry.OnTextChanged
                                (debounce 250 (fun args -> args.NewTextValue |> (EmailTextChanged >> dispatch)))
                            TextEntry.MarginLeft 16.0
                            TextEntry.MarginRight 16.0
                            TextEntry.MarginTop 16.0
                            TextEntry.Height 50.0
                            TextEntry.Keyboard Keyboard.Email ]

                      TextEntry.textEntry
                          [ TextEntry.Placeholder model.Password
                            TextEntry.HorizontalTextAlignment TextAlignment.Center
                            TextEntry.OnTextChanged
                                (debounce 250 (fun args -> args.NewTextValue |> (PasswordTextChanged >> dispatch)))
                            TextEntry.MarginLeft 16.0
                            TextEntry.MarginRight 16.0
                            TextEntry.MarginTop 16.0
                            TextEntry.Height 50.0
                            TextEntry.IsPassword true ]

                      Button.button
                          [ Button.Text "Login"
                            Button.Margin 16.0
                            Button.OnClick(fun () -> dispatch LoginTapped)
                            Button.CanExecute(model.IsEmailValid && model.isPasswordValid)
                            Button.BackgroundColor
                                (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                 else Color.LightGray)
                            Button.BorderColor
                                (if (model.IsEmailValid && model.isPasswordValid) then Color.LightBlue
                                 else Color.Transparent)
                            Button.BorderWidth 1.0 ] ] ]

        let content =
            ScrollView.scrollView [ ScrollView.Content loginEntries ]

        ContentPage.contentPage
            [ ContentPage.HasNavigationBar false
              ContentPage.Content content ]
