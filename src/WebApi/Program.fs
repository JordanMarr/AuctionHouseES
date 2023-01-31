module AuctionHouseES.Program

open Microsoft.AspNetCore.Builder
open Giraffe
open Marten
open Microsoft.AspNetCore.Http
open System

module Users = 
    let seller = Guid.Parse "D63503AF-6D4F-4A6B-B242-1893D99D28E5"
    let bidder1 = Guid.Parse "315DBA78-1744-42D3-B824-69399448AF9C"

let connectionString = "Server=localhost;Port=54320;Database=postgres;User Id=postgres;Password=example;Timeout=3"

let createSampleAuction (next: HttpFunc) (ctx: HttpContext) =
    task {
        let auctionId = Guid.NewGuid()

        let auctionStarted : Events.AuctionCreated = 
            let startedOn = DateTimeOffset.Now
            {
                Id = auctionId
                Title = "Sample Auction"
                Description = "A sample auction"
                StartedBy = Users.seller
                StartedOn = startedOn
                EndsOn = startedOn.AddHours 3
                MinimumBid = None
            }

        use store = DocumentStore.For(connectionString)
        use session = store.LightweightSession()
        
        session.Events.StartStream(auctionId, [ box auctionStarted ]) |> ignore
        
        do! session.SaveChangesAsync()
        return! next ctx
    }

let webApp =
    choose [ 
        route "/" >=> text "Hello world!" 
        route "/create-sample-auction" >=> createSampleAuction
    ]

let builder = WebApplication.CreateBuilder()

builder.Services.AddMarten(fun (opts: StoreOptions) -> 
    opts.Connection connectionString
    |> ignore
)
|> ignore

let app = builder.Build()
app.UseGiraffe webApp

app.Run()