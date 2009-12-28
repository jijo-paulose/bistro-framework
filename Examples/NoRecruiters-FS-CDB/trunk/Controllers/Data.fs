#light

namespace NoRecruiters

module Data =
    open Divan
    open FunctionalDivan.Dsl
    open NoRecruiters.Enums

    let database = 
        server "172.16.10.78" 5984 |>
        db "nr"

    module Entities =
        type baseDocument() =
            inherit Divan.CouchDocument()

        type tag() =
            inherit baseDocument()
            member x.text with get() = "hi"

        type posting() =
            inherit baseDocument()
            //override x.WriteJson writer =
                
            
            

    module Tags =
        let rankedTags num = 
            selectDocs<Entities.tag> (
                query "items" "alltags" database |> limitTo num
                )
            
    module Postings =
        open FunctionalDivan.Dsl.Fti

        let search text tags contentType =
            let t = 
                match tags with 
                | Some t when (List.length t) > 0 -> 
                    sprintf "text:\"%s\" or title:\"%s\" and (tag:%s)" text text (System.String.Join(" or tag:", Array.ofList (List.map (fun (e: Entities.tag) -> e.text)  t)))
                | _ -> sprintf "text:\"%s\" or title:\"%s\"" text text 
                
            selectDocs<Entities.posting> (
                query "items" "all" database |>
                q t
                )