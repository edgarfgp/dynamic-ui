namespace DynamicUI.Extensions

open System
open System.Text.Json
open System.Text.Json.Serialization

[<AutoOpen>]
module Option =
    let OfString s =
        if String.IsNullOrWhiteSpace s then None else Some s
       
module JSON =

    let createJsonOption: JsonSerializerOptions =
        let options = JsonSerializerOptions()
        options.Converters.Add(JsonFSharpConverter())
        options

    let decode<'T> (json: string) =
        try
            JsonSerializer.Deserialize<'T>(json, createJsonOption)
            |> Ok
        with ex -> ex.Message |> Error