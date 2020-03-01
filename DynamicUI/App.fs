namespace DynamicUI

open Fabulous.XamarinForms.LiveUpdate
open Fabulous
open Fabulous.XamarinForms
open Models
open Xamarin.Forms

module App =

    type Msg =
        | SearchPageMsg of SearchPage.Msg
        | HomePageMsg of HomePage.Msg
        | DetailPageMsg of DetailPage.Msg
        | GoToDetailPage of Music
        | GoToFollowerListPage of string
        | NavigationPopped

    type Model =
        { SearchPageModel : SearchPage.Model
          HomePageModel: HomePage.Model
          DetailPageModel: DetailPage.Model option
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { SearchPage : ViewElement
          HomePage: ViewElement
          DetailPage: ViewElement option }

    let init () =
            let sModel, scmd = SearchPage.init
            let hModel, _ = HomePage.init
            { SearchPageModel = sModel
              HomePageModel = hModel
              DetailPageModel = None
              WorkaroundNavPageBug = false
              WorkaroundNavPageBugPendingCmd = Cmd.none }, Cmd.map SearchPageMsg scmd

    let handleHomeExternalMsg externalMsg =
        match externalMsg with
        | HomePage.ExternalMsg.NoOp ->
            Cmd.none
        | HomePage.ExternalMsg.NavigateToDetail music ->
            Cmd.ofMsg (GoToDetailPage music)

    let handleSearchExternalMsg externalMsg =
        match externalMsg with
        | SearchPage.ExternalMsg.NoOp ->
            Cmd.none
        | SearchPage.ExternalMsg.NavigateToFollowersList userName ->
            Cmd.ofMsg (GoToFollowerListPage userName)

    let navigationMapper model =
        let detailModel = model.DetailPageModel
        match  detailModel with
        | None ->
            model
        | Some _  ->
            { model with DetailPageModel = None }

    let update msg model =
        match msg with
        | SearchPageMsg msg ->
            let sModel, cmd, externalMsg = SearchPage.update msg model.SearchPageModel
            let externalSearchMsg = handleSearchExternalMsg externalMsg
            { model with SearchPageModel = sModel  }, Cmd.batch[ Cmd.map SearchPageMsg cmd ; externalSearchMsg ]
        | HomePageMsg msg ->
            let hModel, cmd, externalMsg = HomePage.update msg model.HomePageModel
            let externalHomeMsg = handleHomeExternalMsg externalMsg
            { model with HomePageModel = hModel }, Cmd.batch[ Cmd.map HomePageMsg cmd ; externalHomeMsg]

        | DetailPageMsg _ ->
            { model with  DetailPageModel = None }, Cmd.none

        | GoToDetailPage music ->
            let dModel = DetailPage.init music
            { model with DetailPageModel = Some dModel }, Cmd.none

        | GoToFollowerListPage _ ->
            let dModel, _ = HomePage.init
            { model with HomePageModel = dModel }, Cmd.none

        | NavigationPopped ->
            match model.WorkaroundNavPageBug with
            | true ->
                let newModel =
                    { model with
                        WorkaroundNavPageBug = false
                        WorkaroundNavPageBugPendingCmd = Cmd.none }
                newModel, model.WorkaroundNavPageBugPendingCmd
            | false ->
                navigationMapper model, Cmd.none

    let getPages allPages =
        let searchPage = allPages.SearchPage
        let homePage = allPages.HomePage
        let detailPage = allPages.DetailPage

        match detailPage with
        | None -> [ searchPage ]
        | Some detailPage -> [ searchPage; homePage ; detailPage ]

    let view model dispatch =
        let searchPage = SearchPage.view model.SearchPageModel (SearchPageMsg >> dispatch)
        let homePage = HomePage.view model.HomePageModel (HomePageMsg >> dispatch)
        let detailPage =
            model.DetailPageModel |> Option.map (fun dmodel -> DetailPage.view dmodel.Music (DetailPageMsg >> dispatch))

        let allPages =
            { SearchPage = searchPage
              HomePage = homePage
              DetailPage = detailPage }

        View.Shell(
            items = [
                View.TabBar(
                    items = [
                        View.Tab(
                            title = "Search",
                            items = [
                                View.ShellContent(
                                    content = searchPage
                                )
                            ]
                        )

                        View.Tab(
                            title = "Favorites",
                            items = [
                                View.ShellContent(
                                    content = View.ContentPage(backgroundColor = Color.Green)
                                )
                            ]
                        )
                    ]
                )
            ])

//        View.NavigationPage(
//            useSafeArea = true,
//            barTextColor = Color.Azure,
//            barBackgroundColor = Color.LightBlue,
//            popped =(fun _ -> dispatch NavigationPopped),
//            pages = (getPages allPages))

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