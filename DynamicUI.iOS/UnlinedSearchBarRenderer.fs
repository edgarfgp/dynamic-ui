namespace DynamicUI.iOS

open UIKit
open Xamarin.Forms
open Xamarin.Forms.Platform.iOS

type UnlinedSearchBarRenderer() =
    inherit SearchBarRenderer()

    override this.OnElementChanged(e) =
        base.OnElementChanged(e)

        if (e.NewElement <> null) then
            this.Control.SearchBarStyle <- UISearchBarStyle.Minimal
            this.Control.BarTintColor <- UIColor.Clear
            this.Control.BackgroundColor <- UIColor.Clear
        else
            ()

module UnlinedSearchBarRenderer =
    [<assembly:Xamarin.Forms.ExportRenderer(typeof<DynamicUI.Controls.UnlinedSearchBar>,
                                            typeof<UnlinedSearchBarRenderer>)>]
    do ()
