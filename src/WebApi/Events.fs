namespace AuctionHouseES

open System

type AuctionId = Guid
type UserId = Guid

module Events = 

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
    open Events

    [<CLIMutable>]
    type Auction = 
        {
            Id: Guid
            StartedBy: UserId
            StartsOn: DateTimeOffset
            EndsOn: DateTimeOffset
            Title: string
            Description: string
            MinimumBid: decimal option
            Status: AuctionStatus
            Bids: Bid list
        }
        member this.Apply(ev: AuctionCreated) = 
            { 
                Id = ev.Id
                StartedBy = ev.StartedBy
                StartsOn = ev.StartsOn
                EndsOn = ev.EndsOn
                Title = ev.Title
                Description = ev.Description
                MinimumBid = ev.MinimumBid
                Status = if DateTimeOffset.Now < ev.StartsOn then Created else Started
                Bids = []
            }

        member this.Apply(ev: AuctionCanceled) =
            { this with Status = Canceled }

        member this.Apply(ev: BidPlaced) =
            let bid = { Bidder = ev.Bidder; Amount = ev.Amount; ReceivedOn = ev.ReceivedOn }
            { this with Bids = this.Bids @ [ bid ] }
                
    and AuctionStatus = 
        | Created
        | Started
        | Ended
        | Canceled

    and [<CLIMutable>] Bid = 
        { 
            Bidder: UserId
            Amount: decimal
            ReceivedOn: DateTimeOffset
        }
        