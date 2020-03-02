namespace DynamicUI

open System
open System.Net.Http
open DynamicUI.Models
open FSharp.Json

module NetworkService =
    let baseUrl = "https://api.github.com/users/"
    let mapFollower followerList =
        followerList
        |> List.map (fun c ->
            { login = c.login
              avatar_url = c.avatar_url })

    let mapUser (user: User) =
        { login = user.login
          avatar_url = user.avatar_url
          name = user.name
          location = user.location
          bio = user.bio
          public_repos = user.public_repos
          public_gists = user.public_gists
          html_url = user.html_url
          following = user.following
          followers = user.followers
          created_at = user.created_at }

    let getFollowers searchTerm =

        let urlString = sprintf "%s%s/followers?per_page=100&page=1" baseUrl searchTerm
        async {
            let httpClient = new HttpClient()
            try
                use! response = httpClient.GetAsync(Uri(urlString), HttpCompletionOption.ResponseHeadersRead)
                                |> Async.AwaitTask
                response.EnsureSuccessStatusCode |> ignore

                let! followers = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                let deserialized = Json.deserialize<Follower list> followers

                return Ok(mapFollower deserialized)

            with
            | :? HttpRequestException as ex -> return ex.Message |> Error
            | :? JsonDeserializationError as ex -> return ex.Message |> Error
        }
        |> Async.RunSynchronously

    let getUserInfo userName =

        let urlString = sprintf "%s%s" baseUrl userName
        async {
            let httpClient = new HttpClient()
            try
                use! response = httpClient.GetAsync(Uri(urlString), HttpCompletionOption.ResponseHeadersRead)
                                |> Async.AwaitTask
                response.EnsureSuccessStatusCode |> ignore

                let! user = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                let deserialized = Json.deserialize<User> user

                return Ok(mapUser deserialized)

            with
            | :? HttpRequestException as ex -> return ex.Message |> Error
            | :? JsonDeserializationError as ex -> return ex.Message |> Error
        }
        |> Async.RunSynchronously
