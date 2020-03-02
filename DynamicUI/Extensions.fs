namespace DynamicUI

module Extensions =

    module Async =
        let await<'b> (arg : Async<'b>) = arg |> Async.RunSynchronously


