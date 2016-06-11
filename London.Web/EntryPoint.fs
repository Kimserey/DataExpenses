namespace London.Web

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open global.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.StaticFiles
open Microsoft.Owin.FileSystems
open WebSharper.Owin

module EntryPoint =
    
    type Index = {
        Title: string
        Body: Doc list
    }

    let template = 
        Content.Template<Index>("~/index.html")
            .With("Title", fun x -> x.Title)
            .With("Body", fun x -> x.Body)

    let app = 
        Application.SinglePage(fun _ -> 
            Content.WithTemplate template 
                { Title = "London expenses"
                  Body = [ divAttr [ attr.id "main" ] []
                           divAttr [ attr.id "nav"  ] [] ] })

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
