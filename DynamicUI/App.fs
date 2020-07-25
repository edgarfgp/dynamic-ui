namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
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
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { HomePage: ViewElement
          DetailPage: ViewElement option }

    let init () =
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
            model.DetailPageModel
            |>Option.map (fun model -> DetailPage.view model.Music (DetailPageMsg >> dispatch))

        let allPages =
            { HomePage = homePage
              DetailPage = detailPage }

        View.NavigationPage(
            useSafeArea = true,
            barTextColor = Color.Azure,
            barBackgroundColor = Color.LightBlue,
            popped =(fun _ -> dispatch NavigationPopped),
            pages = (getPages allPages))

type App () as app =
    inherit Application ()
    do Device.SetFlags([
            "Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"; 
            "IndicatorView_Experimental"; "SwipeView_Experimental"; "MediaElement_Experimental"
            "AppTheme_Experimental"; "RadioButton_Experimental"; "Expander_Experimental"
        ])
    
    let _ = 
        Program.mkProgram App.init App.update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
        