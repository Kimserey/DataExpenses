namespace London.Core

open System

module GradientDescent =

    type Settings = {
        LearningRate: float
        Dataset: List<float * float>
        Iterations: int
    } with
        static member Default dataset = { 
            LearningRate = 0.006
            Dataset = dataset
            Iterations = 5000 
        }

    type ModelResult = {
        Estimate: float -> float
        Cost: Cost
        Thethas: float list
        ThethaCalculationSteps: ThethaCalculationSteps
    }

    and ThethaCalculationSteps = ThethaCalculationSteps of float list list
        with override x.ToString() = match x with ThethaCalculationSteps v -> sprintf "Thethas: %A" v

    and Cost = Cost of float
        with 
            override x.ToString() = match x with Cost v -> sprintf "Cost: %.4f%%" v
            
            static member Compute(data: List<float * float>, thethas: float list) =
                match thethas with
                | thetha0::thetha1::_ ->
                    let sum = 
                        [0..data.Length - 1] 
                        |> List.map (fun i -> data.[i])
                        |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                        |> List.sum

                    Cost <| (1./float data.Length) * (Math.Pow(sum, 2.))
                | _ -> failwith "Could not compute cost function, thethas are not in correct format."

                  

    let nextThetha innerDerivative (settings: Settings) thetha =
        let sigma =
            [0..settings.Dataset.Length - 1]
            |> List.map (fun i -> settings.Dataset.[i])
            |> List.map (fun (x, y) -> innerDerivative x y)
            |> List.sum

        thetha - settings.LearningRate * ((1./float settings.Dataset.Length) * sigma)

    let next thethas (settings: Settings) =
        match thethas with
        | thetha0::thetha1::_ ->
            let thetha0 = nextThetha (fun x y -> thetha0 + thetha1 * x - y) settings thetha0
            let thetha1 = nextThetha (fun x y -> (thetha0 + thetha1 * x - y) * x) settings thetha1
            [ thetha0; thetha1 ]
        | _ -> failwith "Could not compute next thethas, thethas are not in correct format."

    let train settings =
        [0..settings.Iterations]
        |> List.scan (fun thethas _ -> next thethas settings) [0.; 0.]

    let createModel settings =
        let interationSteps = train settings

        match List.last interationSteps with
        | thetha0::thetha1::_ as thethas->
            { Estimate = fun  x -> thetha0 + thetha1 * x
              Cost = Cost.Compute(settings.Dataset, thethas)
              Thethas = thethas
              ThethaCalculationSteps = ThethaCalculationSteps interationSteps }
        | _ -> failwith "Failed to create model. Could not compute thethas."