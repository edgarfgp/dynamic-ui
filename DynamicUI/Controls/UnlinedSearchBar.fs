namespace DynamicUI.Controls

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

type UnlinedSearchBar() =
    inherit SearchBar()

[<AutoOpen>]
module UnlinedSearchBar =
    type Fabulous.XamarinForms.View with
        static member inline UnlinedSearchBar(?placeholder, ?textChanged, ?isSpellCheckEnabled, ?keyboard, ?margin) =
            let attribs =
                ViewBuilders.BuildSearchBar
                    (0, ?placeholder = placeholder, ?textChanged = textChanged,
                     ?isSpellCheckEnabled = isSpellCheckEnabled, ?keyboard = keyboard, ?margin = margin)

            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: UnlinedSearchBar) =
                ViewBuilders.UpdateSearchBar(prevOpt, source, target)

            ViewElement.Create(UnlinedSearchBar, update, attribs)

//  (placeholder = Strings.SearchPlaceHolderMessage,
//                         textChanged = debounce 200 (fun args -> args.NewTextValue |> searchMusic),
//                         isSpellCheckEnabled = false, keyboard = Keyboard.Text, margin = Thickness(8.0, 0.0))
