module DynamicUI.Tests

open NUnit.Framework
open DynamicUI.LoginPage
open FsUnit

module ``LoginPage Tests`` =
    [<Test>]
    let ``Init should return a valid initial state``() =
        let initialState =
            { Email = "exmaple@email.com"
              Password = "enter a password"
              IsEmailValid = false
              isPasswordValid = false }

        LoginPage.init |> should equal initialState
