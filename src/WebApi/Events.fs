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

type BidPlaced = 
    {
        Bidder: UserId
        Amount: decimal
        ReceivedOn: DateTimeOffset
    }

