namespace DynamicUI.Extensions

open System

module Option =
    let OfString s =
        if String.IsNullOrWhiteSpace s then None
        else Some s