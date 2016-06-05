namespace London.Web.Rpcs

open WebSharper
open WebSharper.Remoting

module Hello =

    [<Rpc>]
    let sayHello() =
        async { return "Hello"; }