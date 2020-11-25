namespace DynamicUI

open System.Text.Json.Serialization

[<AutoOpen>]
module Models =

    [<JsonFSharpConverter>]
    type Music =
        { trackId: int option
          artistId: int option
          artworkUrl60: string
          artistName: string
          primaryGenreName: string }

    [<JsonFSharpConverter>]
    type MusicList =
        { resultCount: int
          results: Music list }

    type Remote<'t> =
        | LoadingState
        | Content of 't
