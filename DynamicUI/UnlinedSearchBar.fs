namespace DynamicUI

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

type UnlinedSearchBar() =
    inherit SearchBar()

[<AutoOpen>]
module UnlinedSearchBar =
    type Fabulous.XamarinForms.View with
        static member inline UnlinedSearchBar(?placeholder, ?textChanged, ?isSpellCheckEnabled, ?keyboard, ?margin,
                                              ?horizontalTextAlignment) =
            let attribs =
                ViewBuilders.BuildSearchBar
                    (0, ?placeholder = placeholder, ?textChanged = textChanged,
                     ?isSpellCheckEnabled = isSpellCheckEnabled, ?keyboard = keyboard, ?margin = margin,
                     ?horizontalTextAlignment = horizontalTextAlignment)

            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: UnlinedSearchBar) =
                ViewBuilders.UpdateSearchBar(prevOpt, source, target)

            ViewElement.Create(UnlinedSearchBar, update, attribs)
