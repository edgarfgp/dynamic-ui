namespace DynamicUI

open DynamicUI.Models
open FSharp.Data
open FSharp.Json

module NetworkService =

    let getMusicDataSearch (searchText: string option) =
        async {
            let term = match searchText with
                       | Some value -> value
                       | _ -> ""
            let! musicEntries = Async.Catch(Http.AsyncRequestString((Strings.BaseUrlWithParam term)))
            let searchResult =
                match musicEntries with
                | Choice1Of2 musicList ->
                    let musicList = Json.deserialize<MusicList> musicList
                    musicList.results
                | Choice2Of2 _ -> []

            return searchResult
        }



