module DynamicUI.Network

open System
open System.Net.Http
open Microsoft.Extensions.DependencyInjection

type HttpMethod = | Get

[<RequireQualifiedAccess>]
type RequestBody = Json of string

type Request =
    { Url: string
      Method: HttpMethod
      Timeout: TimeSpan option
      Headers: (string * string) list
      Body: RequestBody option
      Query: (string * string) list }

module HttpMethod =
    let value method =
        match method with
        | Get -> System.Net.Http.HttpMethod.Get

module RequestBody =
    let value body =
        match body with
        | RequestBody.Json json -> new StringContent(json, System.Text.Encoding.UTF8, "application/json")

[<AutoOpen>]
module Request =
    let withTimeout timeout request : Request = { request with Timeout = Some timeout }

    let withBody body request : Request = { request with Body = Some body }

    let withHeader header request : Request =
        { request with
              Headers = header :: request.Headers }

    let withQueryParam param request : Request =
        { request with
              Query = param :: request.Query }

type Response =
    { StatusCode: int
      Body: string
      Headers: (string * string) list }

module Http =
    
    let createHttpClientFactory () =
        let services = ServiceCollection()
        services.AddHttpClient() |> ignore

        let serviceProvider = services.BuildServiceProvider()

        serviceProvider.GetRequiredService<IHttpClientFactory>()

    let createRequest url method =
        { Url = url
          Method = method
          Timeout = None
          Headers = []
          Body = None
          Query = [] }

    module Url =
        let private encodeUrlParam param = Uri.EscapeDataString param

        let appendQueryToUrl (url: string) query =
            match query with
            | [] -> url
            | query ->
                url
                + if url.Contains "?" then "&" else "?"
                + String.concat "&" [ for k, v in List.rev query -> encodeUrlParam k + "=" + encodeUrlParam v ]

    let execute (httpClientFactory: IHttpClientFactory) (request: Request): Async<Response> =
        async {
            use httpClient = httpClientFactory.CreateClient()

            request.Timeout
            |> Option.iter (fun t -> httpClient.Timeout <- t)

            let fullUrl =
                Url.appendQueryToUrl request.Url request.Query

            use requestMessage =
                new HttpRequestMessage(request.Method |> HttpMethod.value, fullUrl)

            request.Headers
            |> List.iter requestMessage.Headers.Add

            request.Body
            |> Option.iter (fun b ->
                let body = RequestBody.value b
                requestMessage.Content <- body)

            use! response =
                httpClient.SendAsync(requestMessage)
                |> Async.AwaitTask

            let! body =
                response.Content.ReadAsStringAsync()
                |> Async.AwaitTask

            return
                { StatusCode = int response.StatusCode
                  Body = body
                  Headers = [ for (KeyValue (k, v)) in response.Headers -> (k, String.concat "," v) ] }
        }
        
