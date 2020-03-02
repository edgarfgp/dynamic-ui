namespace DynamicUI

open Fabulous.XamarinForms.LiveUpdate
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module App =

    type Msg =
        | SearchPageMsg of SearchPage.Msg
        | FavoriteListPageMsg of FavoriteListPage.Msg

    type Model =
        { SearchPageModel : SearchPage.Model
          FavoritePageModel: FavoriteListPage.Model }

    let init () =
        let sModel, scmd = SearchPage.init
        let fModel = FavoriteListPage.init
        { SearchPageModel = sModel
          FavoritePageModel = fModel }, Cmd.map SearchPageMsg scmd

    let update msg model =
        match msg with
        | SearchPageMsg msg ->
            let sModel, cmd, _ = SearchPage.update msg model.SearchPageModel
            { model with SearchPageModel = sModel  }, Cmd.batch[ Cmd.map SearchPageMsg cmd ]
        | FavoriteListPageMsg msg ->
            let fModel, cmd, _ = FavoriteListPage.update msg model.FavoritePageModel
            { model with FavoritePageModel = fModel  }, Cmd.batch[ Cmd.map FavoriteListPageMsg cmd ]

    let view model dispatch =
        //Get the pages
        let searchPage = SearchPage.view model.SearchPageModel (SearchPageMsg >> dispatch)
        let favoritePage = FavoriteListPage.view model.FavoritePageModel (FavoriteListPageMsg >> dispatch)
        
        //Create tabs
        let searchTab = View.Tab(title = "Search", items = [ View.ShellContent(content = searchPage)])
        let favoriteTab = View.Tab(title = "Favorites", items = [ View.ShellContent(content = favoritePage)])
        let tabs = View.TabBar(items = [searchTab ; favoriteTab])
        
        View.Shell(items = [ tabs ])

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