module AuctionHouseES.Events

open System

type AuctionCreated = 
    {
        Id: AuctionId
        StartedBy: UserId
        StartsOn: DateTimeOffset
        EndsOn: DateTimeOffset
        Title: string
        Description: string
        MinimumBid: decimal option
    }

type AuctionCanceled  =
    {
        Id: AuctionId
        CanceledBy: UserId
        CanceledOn: DateTimeOffset
        Reason: string
    }

type BidPlaced = 
    {
        Bidder: UserId
        Amount: decimal
        ReceivedOn: DateTimeOffset
    }

module Projections = 

    type Auction() =
        member val Id: AuctionId = Guid.Empty with get, set
        member val StartedBy: UserId = Guid.Empty with get, set
        member val StartsOn: DateTimeOffset = DateTimeOffset.MinValue with get, set
        member val EndsOn: DateTimeOffset = DateTimeOffset.MinValue with get, set
        member val Title: string = "" with get, set
        member val Description: string = "" with get, set
        member val MinimumBid: decimal option = None with get, set
        member val Status: AuctionStatus = Created with get, set
        member val Bids: Bid list = [] with get, set

        member this.Apply(ev: AuctionCreated) = 
            this.Id <- ev.Id
            this.StartedBy <- ev.StartedBy
            this.StartsOn <- ev.StartsOn
            this.EndsOn <- ev.EndsOn
            this.Title <- ev.Title
            this.Description <- ev.Description
            this.MinimumBid <- ev.MinimumBid
            this.Status <- if DateTimeOffset.Now < ev.StartsOn then Created else Started
            this.Bids <- []

        member this.Apply(ev: AuctionCanceled) =
            this.Status <- Canceled

        member this.Apply(ev: BidPlaced) =
            let bid = Bid(Bidder = ev.Bidder, Amount = ev.Amount, ReceivedOn = ev.ReceivedOn)
            this.Bids <- this.Bids @ [ bid ]

    and AuctionStatus = 
        | Created
        | Started
        | Ended
        | Canceled

    and Bid() =
        member val Bidder: UserId = Guid.Empty with get, set
        member val Amount: decimal = 0.0M with get, set
        member val ReceivedOn: DateTimeOffset = DateTimeOffset.MinValue with get, set
        