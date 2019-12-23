namespace DynamicUI

open Fabulous.XamarinForms.LiveUpdate
open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module App =

    type Msg =
        | HomePageMsg of HomePage.Msg
        | DetailPageMsg of DetailPage.Msg
        | AboutPageMsg of AboutPage.Msg
        | LoginPageMsg of LoginPage.Msg
        | NavigationPopped
        | GoToHomePage
        | GoToDetailPage of Music
        | GoToAboutPage
        | GoToLoginPage

    type Model =
        {
          HomePageModel: HomePage.Model
          DetailPageModel: DetailPage.Model option
          AboutPageModel : AboutPage.Model option
          LoginPageModel: LoginPage.Model option
          // Workaround Cmd limitation -- Can not pop a page in page stack and send Cmd at the same time
          // Otherwise it would pop pages 2 times in NavigationPage
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { HomePage: ViewElement
          DetailPage: ViewElement option
          AboutPate : ViewElement option
          LoginPage: ViewElement option}

    let initialModel =
            { LoginPageModel = None
              HomePageModel = HomePage.init
              DetailPageModel = None
              AboutPageModel = None
              WorkaroundNavPageBug = false
              WorkaroundNavPageBugPendingCmd = Cmd.none }

    let init() = initialModel, Cmd.none

    let handleLoginExternalMsg externalMsg =
        match externalMsg with
        | LoginPage.ExternalMsg.NoOp ->
            Cmd.none
        | LoginPage.ExternalMsg.NavigateToHomePage ->
            Cmd.ofMsg GoToHomePage

    let handleHomeExternalMsg externalMsg =
        match externalMsg with
        | HomePage.ExternalMsg.NoOp ->
            Cmd.none
        | HomePage.ExternalMsg.NavigateToAbout ->
            Cmd.ofMsg (GoToAboutPage)
        | HomePage.ExternalMsg.NavigateToLogin ->
            Cmd.ofMsg (GoToLoginPage)
        | HomePage.ExternalMsg.NavigateToDetail music ->
            Cmd.ofMsg (GoToDetailPage music)

    let navigationMapper model=

        let detailModel = model.DetailPageModel
        let aboutModel = model.AboutPageModel
        let loginModel = model.LoginPageModel

        match aboutModel, detailModel, loginModel with
        | None, None, None -> model
        | Some _, None, None ->
            { model with AboutPageModel = None }
        | _, Some _, None ->
            { model with DetailPageModel = None }
        |  _, _, Some _ ->
            { model with LoginPageModel = None }

    let update msg model =
        match msg with
        | HomePageMsg msg ->
            let hModel, _, externalMsg = HomePage.update msg model.HomePageModel
            let externalLoginMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = hModel }, externalLoginMsg
        | LoginPageMsg msg ->
            let lModel, externalMsg = LoginPage.update msg model.LoginPageModel.Value
            let externalLoginMsg = handleLoginExternalMsg externalMsg
            { model with LoginPageModel = Some lModel }, externalLoginMsg
        | DetailPageMsg _ ->
            { model with  DetailPageModel = None }, Cmd.none
        | AboutPageMsg _ ->
            { model with AboutPageModel = None  }, Cmd.none
        | GoToHomePage ->
            let homeModel = HomePage.init
            { model with HomePageModel = homeModel ; LoginPageModel = None }, Cmd.none
        | GoToDetailPage music ->
            let m = DetailPage.init music
            { model with DetailPageModel = Some m }, Cmd.none
        | GoToLoginPage ->
            let m = LoginPage.init
            {model with LoginPageModel = Some m}, Cmd.none
        | GoToAboutPage ->
            let aboutModel = AboutPage.init
            { model with AboutPageModel = Some aboutModel }, Cmd.none
        | NavigationPopped ->
            match model.WorkaroundNavPageBug with
            | true ->
                //Workaround based on https://github.com/TimLariviere/FabulousContacts/blob/47a921ce28e06114b37f49d00ea5d5343fede89d/FabulousContacts/App.fs#L28
                // Do not pop pages if already done manually
                let newModel =
                    { model with
                        WorkaroundNavPageBug = false
                        WorkaroundNavPageBugPendingCmd = Cmd.none }
                newModel, model.WorkaroundNavPageBugPendingCmd
            | false ->
                navigationMapper model, Cmd.none

    let getPages allPages =
        let homePage = allPages.HomePage

        let detailPage = allPages.DetailPage
        let aboutPage = allPages.AboutPate
        let loginPage = allPages.LoginPage

        match detailPage, aboutPage, loginPage with
        | None, None, None -> [ homePage ]
        | None, None, Some loginPage -> [ homePage; loginPage ]
        | Some detailPage, None, None -> [ homePage; detailPage  ]
        | _, Some aboutPage, _ -> [ homePage; aboutPage ]
        | _, _, Some _ -> [ homePage  ]


    let view (model: Model) dispatch =

        let homePage = HomePage.view model.HomePageModel (HomePageMsg >> dispatch)

        let loginPage = model.LoginPageModel |> Option.map (fun hmodel -> LoginPage.view hmodel (LoginPageMsg >> dispatch))

        let detailPage =
            model.DetailPageModel |> Option.map (fun dmodel -> DetailPage.view dmodel.Music (DetailPageMsg >> dispatch))

        let aboutPage =
            model.AboutPageModel |> Option.map(fun abModel -> AboutPage.view abModel (AboutPageMsg >> dispatch))

        let allPages =
            { LoginPage = loginPage
              HomePage = homePage
              DetailPage = detailPage
              AboutPate = aboutPage}

        NavigationPage.navigationPage[
            NavigationPage.UseSafeArea true
            NavigationPage.BarTextColor Color.Azure
            NavigationPage.BarBackgroundColor Color.LightBlue
            NavigationPage.OnPopped (fun _ -> dispatch NavigationPopped)
            NavigationPage.Pages (getPages allPages) ]

    let program = Program.mkProgram init update view

type App () as app =
    inherit Application ()

    let runner =
        App.program
#if DEBUG
        |> Program.withConsoleTrace

#endif
        |> XamarinFormsProgram.run app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif