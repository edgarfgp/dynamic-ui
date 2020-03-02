namespace DynamicUI

open System

module Models =

    type Music =
        { trackId: int option
          artistId: int option
          artworkUrl60: string
          artistName: string
          primaryGenreName: string option }

        static member CreateMusic(?imageUrl, ?artistName, ?genre, ?trackId, ?artistId) =
            let initMember x = Option.fold (fun _ param -> param) <| x
            { artworkUrl60 = initMember "" imageUrl
              artistName = initMember "" artistName
              primaryGenreName = initMember (Some "") genre
              trackId = initMember (Some 0) trackId
              artistId = initMember (Some 0) artistId }

    type MusicList =
        { resultCount: int
          results: Music list }

        static member CreateMusicList(?musicCount, ?music) =
            let initMember x = Option.fold (fun _ param -> param) <| x
            { resultCount = initMember 0 musicCount
              results =
                  initMember
                      [ { artworkUrl60 = ""
                          artistName = ""
                          primaryGenreName = Some ""
                          trackId = Some 0
                          artistId = Some 0 } ] music }

    type User =
        { login: string
          avatar_url: string
          name: string option
          location: string option
          bio: string option
          public_repos: int
          public_gists: int
          html_url: string
          following: int
          followers: int
          created_at: DateTime }
        static member CreateUser(?login, ?avatarUrl, ?name, ?location, ?bio, ?publicRepos, ?publicGists, ?htmlUrl,
                                 ?following, ?followers, ?createdAt) =
            let initMember x = Option.fold (fun _ param -> param) <| x
            { login = initMember "" login
              avatar_url = initMember "" avatarUrl
              name = initMember (Some "") name
              location = initMember (Some "") location
              bio = initMember (Some "") bio
              public_repos = initMember 0 publicRepos
              public_gists = initMember 0 publicGists
              html_url = initMember "" htmlUrl
              following = initMember 0 following
              followers = initMember 0 followers
              created_at = initMember DateTime.Now createdAt }


    type Follower =
        { login: string
          avatar_url: string }
        static member CreateFollower(?login, ?avatarUrl) =
            let initMember x = Option.fold (fun _ param -> param) <| x
            { login = initMember "Edgar" login
              avatar_url = initMember "avatar-placeholder" avatarUrl }

    type Remote<'t> =
        | Loading
        | Content of 't
