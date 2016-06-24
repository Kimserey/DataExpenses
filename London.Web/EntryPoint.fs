namespace London.Web

open System
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server
open global.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.StaticFiles
open Microsoft.Owin.FileSystems
open WebSharper.Owin

module EntryPoint =

    [<EntryPoint>]
    let Main args =

        let root, url =
            match args with
            | [| r; u |] -> r, u
            | _ -> "..", "http://+:9600/"

        use server = 
            let options = 
                new WebSharperOptions<Sitelet.EndPoint>(
                    Debug = true, 
                    ServerRootDirectory = root)

            WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(root)))
                    .UseWebSharper(options.WithSitelet(Sitelet.sitelet))
                    |> ignore)
        
        stdout.WriteLine("Serving {0}", url)
        stdin.ReadLine() |> ignore
        0

