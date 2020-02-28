namespace DynamicUI

module Models =

    type Music =
        { trackId: int option
          artistId: int option
          artworkUrl60: string
          artistName: string
          primaryGenreName: string }

        static member CreateMusic(?imageUrl, ?artistName, ?genre, ?trackId, ?artistId) =
            let initMember x = Option.fold (fun state param -> param) <| x
            { artworkUrl60 = initMember "" imageUrl
              artistName = initMember "" artistName
              primaryGenreName = initMember "" genre
              trackId = initMember (Some 0) trackId
              artistId = initMember (Some 0) artistId }

    type MusicList =
        { resultCount: int
          results: Music list }

        static member CreateMusicList(?musicCount, ?music) =
            let initMember x = Option.fold (fun state param -> param) <| x
            { resultCount = initMember 0 musicCount
              results = initMember [ { artworkUrl60 = "" ; artistName = "" ; primaryGenreName = "" ; trackId =  Some 0 ; artistId= Some 0 } ] music }

    type Remote<'t> =
        | Loading
        | Content of 't
