module AuctionHouseES.Program

open Giraffe
open Marten
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration;
open Microsoft.Extensions.DependencyInjection

let routes =
    choose [ 
        GET >=> route "/" >=> text "Auction House ES" 
        POST >=> routef "/create-sample-auction/%O" Handlers.createSampleAuction
        POST >=> routef "/cancel-auction/%O" Handlers.cancelAuction
        POST >=> route "/place-bid/" >=> bindJson Handlers.placeBid 
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
            .AddMarten(fun (opts: StoreOptions) -> opts.Connection cs)
            |> ignore
    )
    |> ignore

let app = builder.Build()
app.UseGiraffe routes
app.Run()
