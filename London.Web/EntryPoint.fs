namespace London.Web

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
            | [| root; url |] -> root, url
            | _ -> "..", "http://localhost:9600/"

        use server = 
            WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(root)))
                    .UseSitelet(root, Sitelet.sitelet)
                    |> ignore)
        
        stdout.WriteLine("Serving {0}", url)
        stdin.ReadLine() |> ignore
        0

