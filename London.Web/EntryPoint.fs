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
    
    let page = 
        Templates.Index.Doc(
            Title = "London expenses",
            Nav = [ client <@ App.nav @> ],
            Main = [ client <@ App.main @> ])
        |> Content.Page

    let app = 
        Application.SinglePage(fun _ -> page)

    [<EntryPoint>]
    let Main args =
        let rootDirectory = ".."
        let url = "http://localhost:9100/"

        use server = 
            WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(rootDirectory)))
                    .UseSitelet(rootDirectory, app)
                    |> ignore)
        
        
        stdout.WriteLine("Serving {0}", url)
        stdin.ReadLine() |> ignore
        0

