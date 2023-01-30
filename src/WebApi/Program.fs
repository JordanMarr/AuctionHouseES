open Microsoft.AspNetCore.Builder
open Giraffe

let webApp =
    choose [ route "/" >=> text "Hello world!" ]

let app = WebApplication.CreateBuilder().Build()
app.UseGiraffe webApp
app.Run()