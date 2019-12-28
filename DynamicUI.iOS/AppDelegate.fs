// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace DynamicUI.iOS

open System
open System.IO
open Foundation
open UIKit
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit FormsApplicationDelegate()

    let getDbPath() =
        let docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
        let libFolder = Path.Combine(docFolder, "..", "Library", "Databases")

        if not (Directory.Exists libFolder) then
            Directory.CreateDirectory(libFolder) |> ignore
        else
            ()

        Path.Combine(libFolder, "MusicData.db3")

    override this.FinishedLaunching(app, options) =
        Xamarin.Forms.Forms.SetFlags([| "Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"; "CarouselView_Experimental" |])
        FFImageLoading.Forms.Platform.CachedImageRenderer.Init()
        Forms.Init()
        FFImageLoading.Forms.Platform.CachedImageRenderer.Init()
        let dbPath = getDbPath()
        let appcore = DynamicUI.App(dbPath)
        this.LoadApplication(appcore)
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0
