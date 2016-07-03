namespace London.Web

open System
open global.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.StaticFiles
open Microsoft.Owin.FileSystems
open WebSharper.Owin
open Topshelf
open Topshelf.ServiceConfigurators
open Topshelf.HostConfigurators

module EntryPoint =
    
    type OwinHost(rootDirectory: string, baseUrl: string) =
        let mutable server: IDisposable = 
            Unchecked.defaultof<IDisposable>
        
        let options = 
            new WebSharperOptions<Sitelet.EndPoint>(
                Debug = true, 
                ServerRootDirectory = rootDirectory)

        member x.Start() = 
            server <-
                WebApp.Start(baseUrl, fun appB ->
                    appB.UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(rootDirectory)))
                        .UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem("..\\..\\..\\Documents\\Expenses")))
                        .UseWebSharper(options.WithSitelet(Sitelet.sitelet))
                        |> ignore)
        
            stdout.WriteLine("Serving {0}", baseUrl)

        member x.Stop() =
            server.Dispose()

    [<EntryPoint>]
    let Main args =
        let mutable root = ".."
        let mutable url = "http://+:9600/"

        HostFactory.Run(Action<HostConfigurator>(fun hostCfg ->
        
            hostCfg.AddCommandLineDefinition("args", Action<string>(fun args -> 
                let r, u =
                    match args.Split(',') with
                    | [| r; u |] -> r, u
                    | _ -> failwith "Expecting -args=rootDirectory,baseUrl"
                root <- r
                url <- u))

            hostCfg.ApplyCommandLine()

            hostCfg.Service<OwinHost>(Action<ServiceConfigurator<OwinHost>>(fun s ->
                s.ConstructUsing(Func<OwinHost>(fun () -> new OwinHost(root, url)))
                    .WhenStarted(Action<OwinHost>(fun s -> s.Start()))
                    .WhenStopped(Action<OwinHost>(fun s -> s.Stop())) |> ignore)
                ) |> ignore
    
            hostCfg
                .RunAsLocalSystem() |> ignore

            hostCfg.SetServiceName("london-expenses")
            hostCfg.SetDisplayName("London expenses")
            hostCfg.SetDescription("London expenses manager. Author: Kimserey Lam.")))
        |> ignore

        0

