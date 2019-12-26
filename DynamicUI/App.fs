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
        | GoToDetailPage of Music
        | NavigationPopped

    type Model =
        { HomePageModel: HomePage.Model
          DetailPageModel: DetailPage.Model option
          // Workaround Cmd limitation -- Can not pop a page in page stack and send Cmd at the same time
          // Otherwise it would pop pages 2 times in NavigationPage
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { HomePage: ViewElement
          DetailPage: ViewElement option }

    let init() =
            let hModel, cmd = HomePage.init
            { HomePageModel = hModel
              DetailPageModel = None
              WorkaroundNavPageBug = false
              WorkaroundNavPageBugPendingCmd = Cmd.none }, Cmd.map HomePageMsg cmd

    let handleHomeExternalMsg externalMsg =
        match externalMsg with
        | HomePage.ExternalMsg.NoOp ->
            Cmd.none
        | HomePage.ExternalMsg.NavigateToDetail music ->
            Cmd.ofMsg (GoToDetailPage music)

    let navigationMapper model =
        let detailModel = model.DetailPageModel
        match  detailModel with
        | None ->
            model
        | Some _  ->
            { model with DetailPageModel = None }

    let update msg model =
        match msg with
        | HomePageMsg msg ->
            let hModel, cmd, externalMsg = HomePage.update msg model.HomePageModel
            let externalHomeMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = hModel }, Cmd.batch[ Cmd.map HomePageMsg cmd ; externalHomeMsg]

        | DetailPageMsg _ ->
            { model with  DetailPageModel = None }, Cmd.none

        | GoToDetailPage music ->
            let dModel = DetailPage.init music
            { model with DetailPageModel = Some dModel }, Cmd.none

        | NavigationPopped ->
            match model.WorkaroundNavPageBug with
            | true ->
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

        match detailPage with
        | None -> [ homePage ]
        | Some detailPage -> [ homePage ; detailPage ]

    let view model dispatch =

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
            NavigationPage.OnPopped  (fun _ -> dispatch NavigationPopped)
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