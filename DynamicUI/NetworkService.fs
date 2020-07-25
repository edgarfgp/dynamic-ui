namespace DynamicUI

open FSharp.Data
open FSharp.Json

module NetworkService =
    let fetchMusic =
        async {
            let urlString = sprintf "https://itunes.apple.com/search?term=%A" System.String.Empty
            let! musicEntries = Async.Catch(Http.AsyncRequestString(urlString))
            let searchResult =
                match musicEntries with
                | Choice1Of2 musicList ->
                  let musicList = Json.deserialize<MusicList> musicList
                  Ok musicList.results
                | Choice2Of2 error -> Error error

            return searchResult
    }
