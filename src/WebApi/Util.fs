namespace AuctionHouseES

open System


type AuctionId = Guid
type UserId = Guid

type AppConfig =
    {
        ConnectionString: string
    }

module Users = 
    let seller = Guid "d63503af-6d4f-4a6b-b242-1893d99d28e5"
    let bidder1 = Guid "315dba78-1744-42d3-b824-69399448af9c"
