namespace AuctionHouseES

open System

type AppConfig =
    {
        ConnectionString: string
    }

module Users = 
    let seller = Guid.Parse "D63503AF-6D4F-4A6B-B242-1893D99D28E5"
    let bidder1 = Guid.Parse "315DBA78-1744-42D3-B824-69399448AF9C"