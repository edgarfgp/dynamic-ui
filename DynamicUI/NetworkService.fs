namespace DynamicUI

open DynamicUI.Models
open FSharp.Data
open FSharp.Json

module NetworkService =
    let getMusicDataSearch (searchText: string option) =

        async {
            let term =
                match searchText with
                | Some value -> value
                | _ -> ""

            let urlString = sprintf "https://itunes.apple.com/search?term=%A" term

            let! musicEntries = Async.Catch(Http.AsyncRequestString(urlString))
            let searchResult =
                match musicEntries with
                | Choice1Of2 musicList ->
                    let musicList = Json.deserialize<MusicList> musicList
                    Ok musicList.results
                | Choice2Of2 error -> Error error

            return searchResult
        }
