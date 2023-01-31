module AuctionHouseES.Handlers

open System
open Giraffe
open Marten
open Microsoft.AspNetCore.Http

let createSampleAuction (auctionId: Guid) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let auctionCreated : Events.AuctionCreated = 
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

        let cfg = ctx.GetService<AppConfig>()
        use store = DocumentStore.For(cfg.ConnectionString)
        use session = store.LightweightSession()        
        session.Events.StartStream(auctionId, [ box auctionCreated ]) |> ignore
        do! session.SaveChangesAsync()
        return! Successful.OK() next ctx        
    }

let cancelAuction (auctionId: Guid) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let auctionCanceled : Events.AuctionCanceled = 
            {
                Id = auctionId
                CanceledBy = Users.seller
                CanceledOn = DateTimeOffset.Now
                Reason = "Test cancel"
            }

        let cfg = ctx.GetService<AppConfig>()
        use store = DocumentStore.For(cfg.ConnectionString)
        use session = store.LightweightSession()
        session.Events.Append(auctionId, [ box auctionCanceled ]) |> ignore
        do! session.SaveChangesAsync()
        return! Successful.OK() next ctx
    }