namespace London.Core

open System

module GradientDescent =

    type Options = {
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
        with override x.ToString() = match x with Cost v -> sprintf "Cost: %.4f%%" v
                  
    let costFunc thethas (data: List<float * float>): float =
        match thethas with
        | thetha0::thetha1::_ ->
            let sum = 
                [0..data.Length - 1] 
                |> List.map (fun i -> data.[i])
                |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                |> List.sum

            (1./float data.Length) * (Math.Pow(sum, 2.))
        | _ -> failwith "Could not compute cost function, thethas are not in correct format."

    let next thethas (options: Options) =
        match thethas with
        | thetha0::thetha1::_ ->
            let thetha0' = 
                let sum = 
                    [0..options.Dataset.Length - 1] 
                    |> List.map (fun i -> options.Dataset.[i])
                    |> List.map (fun (x, y) -> thetha0 + thetha1 * x - y)
                    |> List.sum

                thetha0 - (options.LearningRate * (1./float options.Dataset.Length) * sum)

            let thetha1' =
                let sum =
                    [0..options.Dataset.Length - 1]
                    |> List.map (fun i -> options.Dataset.[i])
                    |> List.map (fun (x, y) -> (thetha0 + thetha1 * x - y) * x)
                    |> List.sum
        
                thetha1 - (options.LearningRate * (1./float options.Dataset.Length) * sum)

            [thetha0'; thetha1']
        | _ -> failwith "Could not compute next thethas, thethas are not in correct format."

    let train options =
        [0..options.Iterations]
        |> List.scan (fun thethas _ -> next thethas options) [0.; 0.]

    let createModel options =
        let interationSteps = train options

        match List.last interationSteps with
        | thetha0::thetha1::_ as thethas->
            { Estimate = fun  x -> thetha0 + thetha1 * x
              Cost = costFunc thethas options.Dataset |> Cost
              Thethas = thethas
              ThethaCalculationSteps = ThethaCalculationSteps interationSteps }
        | _ -> failwith "Failed to create model. Could not compute thethas."