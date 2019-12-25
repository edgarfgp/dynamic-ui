namespace DynamicUI

open Fabulous.XamarinForms.LiveUpdate
open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module App =

    type Remote<'t> =
        | Empty
        | Loading
        | LoadError of string
        | Content of 't

    type Msg =
        | HomePageMsg of HomePage.Msg
        | DetailPageMsg of DetailPage.Msg
        | GoToDetailPage of Music
        | GoToHomePage

    type Model =
        { HomePageModel: HomePage.Model
          DetailPageModel: DetailPage.Model option }

    type Pages =
        { HomePage: ViewElement
          DetailPage: ViewElement option }

    let initialModel =
            { HomePageModel = HomePage.init
              DetailPageModel = None }

    let init() = initialModel, Cmd.none

    let handleHomeExternalMsg externalMsg =
        match externalMsg with
        | HomePage.ExternalMsg.NoOp ->
            Cmd.none
        | HomePage.ExternalMsg.NavigateToDetail music ->
            Cmd.ofMsg (GoToDetailPage music)

    let update msg model =
        match msg with
        | HomePageMsg msg ->
            let hModel, externalMsg = HomePage.update msg model.HomePageModel
            let externalHomeMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = hModel }, externalHomeMsg
        | DetailPageMsg _ ->
            { model with  DetailPageModel = None }, Cmd.none
        | GoToHomePage ->
            let homeModel = HomePage.init
            { model with HomePageModel = homeModel }, Cmd.none
        | GoToDetailPage music ->
            let m = DetailPage.init music
            { model with DetailPageModel = Some m }, Cmd.none

    let getPages allPages =
        let homePage = allPages.HomePage
        let detailPage = allPages.DetailPage

        match detailPage with
        | None -> [ homePage ]
        | Some detailPage -> [ homePage ; detailPage ]

    let view (model: Model) dispatch =

        let homePage = HomePage.view model.HomePageModel (HomePageMsg >> dispatch)

        let detailPage =
            model.DetailPageModel |> Option.map (fun dmodel -> DetailPage.view dmodel.Music (DetailPageMsg >> dispatch))

        let allPages =
            { HomePage = homePage
              DetailPage = detailPage }

        NavigationPage.navigationPage[
            NavigationPage.UseSafeArea true
            NavigationPage.BarTextColor Color.Azure
            NavigationPage.BarBackgroundColor Color.LightBlue
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