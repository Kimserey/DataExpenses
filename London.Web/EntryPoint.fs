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
open London.Core

module EntryPoint =
    
    type OwinHost(rootDirectory: string, dataDirectory: string, baseUrl: string) =
        let mutable server: IDisposable = 
            Unchecked.defaultof<IDisposable>
        
        let options = 
            new WebSharperOptions<Sitelet.EndPoint>(
                Debug = true, 
                ServerRootDirectory = rootDirectory)

        member x.Start() = 
            
            // Instantiate global dataframe
            Dataframe.agent.Refresh (Some dataDirectory)

            server <-
                WebApp.Start(baseUrl, fun appB ->
                    appB.UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(rootDirectory)))
                        .UseWebSharper(options.WithSitelet(Sitelet.sitelet))
                        |> ignore)

            stdout.WriteLine("Root directory {0}", rootDirectory)
            stdout.WriteLine("Data directory {0}", dataDirectory)
            stdout.WriteLine("Serving {0}", baseUrl)

        member x.Stop() =
            server.Dispose()

    [<EntryPoint>]
    let Main args =
        let mutable root = ".."
        let mutable data = "..\\..\\..\\..\\Documents\\Expenses"
        let mutable url = "http://+:9600/"

        HostFactory.Run(Action<HostConfigurator>(fun hostCfg ->
        
            hostCfg.AddCommandLineDefinition("args", Action<string>(fun args -> 
                let r, d, u =
                    match args.Split(',') with
                    | [| r; d; u |] -> r, d, u
                    | _ -> failwith "Expecting -args=rootDirectory,dataDirectory,baseUrl"
                root <- r
                data <- d
                url <- u))

            hostCfg.ApplyCommandLine()

            hostCfg.Service<OwinHost>(Action<ServiceConfigurator<OwinHost>>(fun s ->
                s.ConstructUsing(Func<OwinHost>(fun () -> new OwinHost(root, data, url)))
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

