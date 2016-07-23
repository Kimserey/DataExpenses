namespace London.Web

open System
open System.IO
open System.Text.RegularExpressions
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

    type Config = {
        ServiceName: string
        DataDir: string
        Url: string
        RootDir: string
        BinDir: string
    } with
        static member Default = {
            ServiceName = "london-expenses"
            DataDir     = "C:\\Documents\\Expenses"
            Url         = "http://+:9600/"
            RootDir     = "httproot"
            BinDir      = "."
        }

    let (|Arg|_|) pattern (input: string) =
        let m = Regex.Match(input, sprintf "^%s=(.*)$" pattern) 
        if (m.Success) then Some m.Groups.[1].Value else None  
    
    let mutable config = Config.Default

    type OwinHost(config: Config) =
        let mutable server: IDisposable = 
            Unchecked.defaultof<IDisposable>
        
        let options = 
            new WebSharperOptions<Sitelet.EndPoint>(
                Debug = true, 
                ServerRootDirectory = config.RootDir,
                BinDirectory = config.BinDir,
                Sitelet = Some Sitelet.sitelet)

        member x.Start() = 
            
            // Instantiate global dataframe
            Dataframe.agent.Refresh (Some config.DataDir)
            
            server <-
                WebApp.Start(config.Url, fun appB ->
                    appB.UseWebSharper(options)
                        .UseStaticFiles(StaticFileOptions(FileSystem = PhysicalFileSystem(config.RootDir)))
                        |> ignore)

            printfn "Root directory %s" config.RootDir
            printfn "Data directory %s" config.DataDir
            printfn "Serving %s" config.Url

        member x.Stop() =
            server.Dispose()

    [<EntryPoint>]
    let Main args =
        HostFactory.Run(Action<HostConfigurator>(fun hostCfg ->

            hostCfg.AddCommandLineDefinition("args", Action<string>(fun args -> 
                let rec processArgs args config =
                    match args with
                    | (Arg "data" dataDir)::l               -> processArgs l { config with DataDir = dataDir } 
                    | (Arg "url" url)::l                    -> processArgs l { config with Url = url } 
                    | (Arg "servicename" servicename)::l    -> processArgs l { config with ServiceName = servicename } 
                    | unrecognized::l                      -> 
                        printfn "Skipped unrecognized args \"%s\"" unrecognized
                        processArgs l config
                    | [] -> config
                
                config <- processArgs (args.Split(',') |> Array.toList) Config.Default
            ))
            
            hostCfg.ApplyCommandLine()

            hostCfg.Service<OwinHost>(Action<ServiceConfigurator<OwinHost>>(fun s ->
                s.ConstructUsing(Func<OwinHost>(fun () -> new OwinHost(config)))
                    .WhenStarted(Action<OwinHost>(fun s -> s.Start()))
                    .WhenStopped(Action<OwinHost>(fun s -> s.Stop())) |> ignore)
                ) |> ignore
    
            hostCfg
                .RunAsLocalSystem() |> ignore

            hostCfg.SetServiceName(config.ServiceName)
            hostCfg.SetDisplayName("London expenses")
            hostCfg.SetDescription("London expenses manager. Author: Kimserey Lam.")))
        |> ignore

        0

