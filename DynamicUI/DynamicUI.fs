namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module App =

    type Msg =
        | LoginPageMsg of LoginPage.Msg
        | HomePageMsg of HomePage.Msg
        | GoToHomePage
        | GoToLoginPage

    type Model =
        { LoginPageModel: LoginPage.Model
          HomePageModel: HomePage.Model option }

    type Pages =
        { LoginPage: ViewElement
          HomePage: ViewElement option }

    let init() =
        let loginModel = LoginPage.init

        let initialModel =
            { LoginPageModel = loginModel
              HomePageModel = None }
        initialModel, Cmd.none

    let handleLoginExternalMsg externalMsg =
        match externalMsg with
        | LoginPage.ExternalMsg.NoOp ->
            Cmd.none
        | LoginPage.ExternalMsg.GoToHomePage ->
            Cmd.ofMsg GoToHomePage

    let handleHomeExternalMsg externalMsg =
        match externalMsg with
        | HomePage.ExternalMsg.NoOp ->
            Cmd.none
        | HomePage.ExternalMsg.GoToLoginPage ->
            Cmd.ofMsg GoToLoginPage

    let update msg model =
        match msg with
        | LoginPageMsg msg ->
            let loginModel, cmd, externalMsg = LoginPage.update msg model.LoginPageModel
            let externalLoginMsg = handleLoginExternalMsg externalMsg
            let batchCmd = Cmd.batch [ (Cmd.map LoginPageMsg cmd); externalLoginMsg ]
            { model with LoginPageModel = loginModel }, batchCmd
        | HomePageMsg msg ->
            let _, _, externalMsg = HomePage.update msg model.HomePageModel.Value
            let externalLoginMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = None }, externalLoginMsg
        | GoToHomePage ->
            let homeModel = HomePage.init
            { model with HomePageModel = Some homeModel }, Cmd.none
        | GoToLoginPage ->
            let loginModel = LoginPage.init
            { model with LoginPageModel = loginModel }, Cmd.none

    let getPages allPages =
        let loginPage = allPages.LoginPage
        let homePage = allPages.HomePage
        match homePage with
        | Some homePage -> [ homePage ]
        | None -> [ loginPage ]

    let view (model: Model) dispatch =
        let loginPage = LoginPage.view model.LoginPageModel (LoginPageMsg >> dispatch)
        let homePage =
            model.HomePageModel |> Option.map (fun hModel -> HomePage.view hModel (HomePageMsg >> dispatch))

        let allPages =
            { LoginPage = loginPage
              HomePage = homePage }

        View.NavigationPage
            (barTextColor = Color.Azure, useSafeArea = true, barBackgroundColor = Color.LightBlue,
             pages = getPages allPages)

    let program = Program.mkProgram init update view

type App () as app =
    inherit Application ()

    let _ =
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app