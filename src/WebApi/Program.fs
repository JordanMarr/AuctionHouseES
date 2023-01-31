module AuctionHouseES.Program

open Giraffe
open Giraffe.EndpointRouting
open Marten
open Marten.Events.Projections
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration;
open Microsoft.Extensions.DependencyInjection

let endpoints = [ 
        GET [ route "/" (text "Auction House ES") ]
        subRoute "/api/auction" [
            POST [ 
                route "/create/" (bindJson Handlers.createAuction)
                route "/cancel/" (bindJson Handlers.cancelAuction)
                route "/bid/" (bindJson Handlers.placeBid) 
            ]
            GET [ routef "/%O" Handlers.getAuction ]
        ]
    ]

let builder = WebApplication.CreateBuilder()
builder.WebHost
    .ConfigureAppConfiguration(fun ctx cfg ->
        cfg
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables() |> ignore
    )
    .ConfigureServices(fun ctx services -> 
        let cs = ctx.Configuration.GetConnectionString "postgres"
        services
            .AddGiraffe()
            .AddSingleton({ AppConfig.ConnectionString = cs })
            .AddMarten(fun (opts: StoreOptions) -> 
                opts.Connection cs

                // Tell Marten to update this aggregate inline
                opts.Projections.SelfAggregate<Events.Projections.Auction>(ProjectionLifecycle.Inline) |> ignore
            )
            |> ignore
    )
    |> ignore

let app = builder.Build()
app
    .UseRouting()
    .UseEndpoints(fun e -> e.MapGiraffeEndpoints endpoints)
    |> ignore 

app.Run()
