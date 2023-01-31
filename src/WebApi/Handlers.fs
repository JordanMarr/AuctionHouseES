module AuctionHouseES.Handlers

open System
open Giraffe
open Marten
open Microsoft.AspNetCore.Http
open Events

type CreateAuctionRequest = 
    {
        AuctionId: AuctionId
        StartedBy: UserId
        StartsOn: DateTimeOffset
        EndsOn: DateTimeOffset
        Title: string
        Description: string
        MinimumBid: decimal option
    }

let createAuction (req: CreateAuctionRequest) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let auctionCreated : Events.AuctionCreated = 
            {
                Id = req.AuctionId
                Title = req.Title
                Description = req.Description
                StartedBy = req.StartedBy
                StartsOn = req.StartsOn
                EndsOn = req.EndsOn
                MinimumBid = req.MinimumBid
            }

        let cfg = ctx.GetService<AppConfig>()
        use store = DocumentStore.For(cfg.ConnectionString)
        use session = store.LightweightSession()        
        session.Events.StartStream(req.AuctionId, [ box auctionCreated ]) |> ignore
        do! session.SaveChangesAsync()
        return! Successful.OK() next ctx        
    }

let cancelAuction (auctionId: AuctionId) (next: HttpFunc) (ctx: HttpContext) =
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

    
type BidRequest = { AuctionId: AuctionId; Bidder: UserId; Amount: decimal }

let placeBid (req: BidRequest) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let bidPlaced : Events.BidPlaced = 
            {
                Events.BidPlaced.Bidder = req.Bidder
                Events.BidPlaced.Amount = req.Amount
                Events.BidPlaced.ReceivedOn = DateTimeOffset.Now
            }

        let cfg = ctx.GetService<AppConfig>()
        use store = DocumentStore.For(cfg.ConnectionString)
        use session = store.LightweightSession()
        session.Events.Append(req.AuctionId, [ box bidPlaced ]) |> ignore
        do! session.SaveChangesAsync()
        return! Successful.OK() next ctx
    }

let getAuction (auctionId: AuctionId) (next: HttpFunc) (ctx: HttpContext) = 
    task {
        let cfg = ctx.GetService<AppConfig>()
        use store = DocumentStore.For(cfg.ConnectionString)
        use session = store.LightweightSession()

        // If using "Live" aggregation
        let! aggregate = session.Events.AggregateStreamAsync<Projections.Auction>(auctionId)
        return! Successful.ok (json aggregate) next ctx
    }