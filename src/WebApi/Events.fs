module AuctionHouseES.Events

open System

type AuctionId = Guid
type UserId = Guid

type AuctionCreated = 
    {
        Id: AuctionId
        StartedBy: UserId
        StartedOn: DateTimeOffset
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
