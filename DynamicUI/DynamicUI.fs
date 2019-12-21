namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module App =

    type Msg =
        | LoginPageMsg of LoginPage.Msg
        | HomePageMsg of HomePage.Msg
        | DetailPageMsg of DetailPage.Msg
        | GoToHomePage
        | GoToDetailPage of Music

    type Model =
        { LoginPageModel: LoginPage.Model
          HomePageModel: HomePage.Model option
          DetailPageModel: DetailPage.Model option }

    type Pages =
        { LoginPage: ViewElement
          HomePage: ViewElement option
          DetailPage: ViewElement option }

    let initialModel =
            { LoginPageModel = LoginPage.init
              HomePageModel = None
              DetailPageModel = None }

    let init() = initialModel, Cmd.none

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
        | HomePage.ExternalMsg.NavigateToDetail music ->
            Cmd.ofMsg (GoToDetailPage music)

    let handleDetailExternalMsg externalMsg =
        match externalMsg with
        | DetailPage.ExternalMsg.NoOp ->
            Cmd.none

    let update msg model =
        match msg with
        | LoginPageMsg msg ->
            let loginModel, cmd, externalMsg = LoginPage.update msg model.LoginPageModel
            let externalLoginMsg = handleLoginExternalMsg externalMsg
            let batchCmd = Cmd.batch [ (Cmd.map LoginPageMsg cmd); externalLoginMsg ]
            { model with LoginPageModel = loginModel }, batchCmd
        | HomePageMsg msg ->
            let hModel, _, externalMsg = HomePage.update msg model.HomePageModel
            let externalLoginMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = hModel }, externalLoginMsg
        | DetailPageMsg msg ->
            let _, cmd, externaMsg = DetailPage.update msg model.DetailPageModel
            let externalDetailMsg = handleDetailExternalMsg externaMsg
            let batchCmd = Cmd.batch [ (Cmd.map DetailPageMsg cmd); externalDetailMsg ]
            { model with  DetailPageModel = None }, batchCmd
        | GoToHomePage ->
            let homeModel = HomePage.init
            { model with HomePageModel = Some homeModel }, Cmd.none
        | GoToDetailPage music ->
            let m, cmd = DetailPage.init music
            {model with DetailPageModel = Some m}, (Cmd.map DetailPageMsg cmd)

    let getPages allPages =
        let loginPage = allPages.LoginPage
        let homePage = allPages.HomePage
        let detailPage = allPages.DetailPage

        match homePage, detailPage with
        | None, None -> [ loginPage ]
        | Some homePage, None -> [ homePage ]
        | Some homePage, Some detailPage -> [ homePage; detailPage ]
        | None , Some detailPage -> [ detailPage ]

    let view (model: Model) dispatch =
        let loginPage = LoginPage.view model.LoginPageModel (LoginPageMsg >> dispatch)
        let homePage =
            model.HomePageModel |> Option.map (fun hmodel -> HomePage.view hmodel (HomePageMsg >> dispatch))

        let detailPage =
            model.DetailPageModel |> Option.map (fun dmodel -> DetailPage.view dmodel.Music (DetailPageMsg >> dispatch))

        let allPages =
            { LoginPage = loginPage
              HomePage = homePage
              DetailPage = detailPage }

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