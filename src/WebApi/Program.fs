module AuctionHouseES.Program

open Microsoft.AspNetCore.Builder
open Giraffe
open Marten

let webApp =
    choose [ route "/" >=> text "Hello world!" ]

let builder = WebApplication.CreateBuilder()

builder.Services.AddMarten(fun (opts: StoreOptions) -> 
    let connectionString = "Server=localhost;Port=54320;Database=postgres;User Id=postgres;Password=example;Timeout=3"
    opts.Connection(connectionString) 
    |> ignore
)
|> ignore

let app = builder.Build()
app.UseGiraffe webApp

app.Run()