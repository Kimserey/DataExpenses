#I __SOURCE_DIRECTORY__
#load "../packages/Deedle/Deedle.fsx"

open System
open Deedle

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let expenses = 
    Frame.ReadCsv("data/201511.csv")
