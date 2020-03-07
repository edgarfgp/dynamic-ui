namespace DynamicUI.iOS

open UIKit
open Xamarin.Forms.Platform.iOS

type UnlinedSearchBarRenderer() as self =
    inherit SearchBarRenderer()

    override __.OnElementChanged(e) =
        base.OnElementChanged(e)

        if (e.NewElement <> null) then
            self.Control.SearchBarStyle <- UISearchBarStyle.Minimal
            self.Control.BarTintColor <- UIColor.Clear
            self.Control.BackgroundColor <- UIColor.Clear

        else
            ()

module UnlinedSearchBarRenderer =
    [<assembly:Xamarin.Forms.ExportRenderer(typeof<DynamicUI.UnlinedSearchBar>,
                                            typeof<UnlinedSearchBarRenderer>)>]
    do ()
