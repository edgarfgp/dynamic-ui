namespace DynamicUI

open System.Threading.Tasks
open DynamicUI.Extensions
open FSharp.Control.Tasks
open DynamicUI.Network

type ServiceError =
    | NetworkError
    | ParseError of string

type INetworkService =
    abstract GetMusic: unit -> ValueTask<Result<MusicList, ServiceError>>

type NetworkService() =
    let httpClientFactory = Http.createHttpClientFactory ()
    
    let fetch urlString =
        let request =
            Http.createRequest urlString Get
            |> withHeader ("Accept", "application/json")
            |> withQueryParam ("print", "Url")

        request |> Http.execute httpClientFactory
    
    interface INetworkService with
        member __.GetMusic() =
            let urlString = sprintf "https://itunes.apple.com/search?term=%A" System.String.Empty

            vtask {
                let! response = fetch urlString
                match response.StatusCode with
                | 200 ->
                    return response.Body
                        |> JSON.decode
                        |> Result.mapError ParseError
                | _ ->
                    return Error NetworkError
            }
