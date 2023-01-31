module AuctionHouseES.Program

open Giraffe
open Marten
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration;
open Microsoft.Extensions.DependencyInjection
open Marten.Events.Projections

let routes =
    choose [ 
        GET >=> route "/" >=> text "Auction House ES" 
        POST >=> route "/create-auction/" >=> bindJson Handlers.createAuction
        POST >=> routef "/cancel-auction/%O" Handlers.cancelAuction
        POST >=> route "/place-bid/" >=> bindJson Handlers.placeBid 
        GET >=> routef "/get-auction/%O" Handlers.getAuction
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
app.UseGiraffe routes
app.Run()
