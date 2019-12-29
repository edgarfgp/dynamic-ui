namespace DynamicUI

open Thoth.Json.Net


module Models =

    type Music =
        { TrackId: int
          ArtistId: int
          ImageUrl: string
          ArtistName: string
          Genre: string }
        static member Decoder: Decoder<Music> =
            Decode.object (fun get ->
                { ImageUrl = get.Required.Field "artworkUrl60" Decode.string
                  ArtistName = get.Required.Field "artistName" Decode.string
                  Genre = get.Required.Field "primaryGenreName" Decode.string
                  TrackId = get.Optional.Field "trackId" Decode.int |> Option.defaultValue 0
                  ArtistId = get.Optional.Field "artistId" Decode.int |> Option.defaultValue 0 })

    type MusicList =
        { MusicCount: int
          Music: Music list }

        static member Decoder: Decoder<MusicList> =
            Decode.object (fun get ->
                { MusicCount = get.Required.Field "resultCount" Decode.int
                  Music = get.Required.Field "results" (Decode.list Music.Decoder) })

    type Remote<'t> =
        | Loading
        | Content of 't
