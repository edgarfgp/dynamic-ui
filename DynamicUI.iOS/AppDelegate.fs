// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace DynamicUI.iOS

open Foundation
open UIKit
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

[<Register("AppDelegate")>]
type AppDelegate() =
    inherit FormsApplicationDelegate()

    override this.FinishedLaunching(app, options) =
        Forms.Init()
        let appcore = DynamicUI.App()
        this.LoadApplication(appcore)
        base.FinishedLaunching(app, options)

module Main =
    [<EntryPoint>]
    let main args =
        UIApplication.Main(args, null, "AppDelegate")
        0
