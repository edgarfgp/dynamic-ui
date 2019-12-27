// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace DynamicUI.iOS

open System
open UIKit
open Foundation
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit FormsApplicationDelegate()

    override this.FinishedLaunching(app, options) =
        Xamarin.Forms.Forms.SetFlags([| "Shell_Experimental"; "CollectionView_Experimental"; "Visual_Experimental"; "CarouselView_Experimental" |])
        FFImageLoading.Forms.Platform.CachedImageRenderer.Init()
        Forms.Init()
        FFImageLoading.Forms.Platform.CachedImageRenderer.Init()
        let appcore = new DynamicUI.App()
        this.LoadApplication(appcore)
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0
