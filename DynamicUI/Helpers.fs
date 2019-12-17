namespace DynamicUI

open Xamarin.Forms

module Helpers =
    let displayAlert (title, message, cancel) =
        Application.Current.MainPage.DisplayAlert(title, message, cancel) |> Async.AwaitTask
