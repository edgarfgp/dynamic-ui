namespace DynamicUI

[<AutoOpen>]
module Models =

    type Music =
        { trackId: int option
          artistId: int option
          artworkUrl60: string
          artistName: string
          primaryGenreName: string }

    type MusicList =
        { resultCount: int
          results: Music list }

    type Remote<'t> =
        | LoadingState
        | Content of 't
