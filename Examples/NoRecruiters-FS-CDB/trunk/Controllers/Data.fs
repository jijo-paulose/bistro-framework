#light

namespace NoRecruiters

module Data =
    open Divan
    open FunctionalDivan.Dsl
    open NoRecruiters.Enums

    let database = 
        server "localhost" 5984 |>
        db "nr"

    module Entities =
        type tag = {
            id: string; rev: string;
            tagText: string
            safeText: string
            }

        type posting = {
            id: string; rev: string;
            tags: tag list
            userId: string
            createdOn: System.DateTime
            updatedOn: System.DateTime
            heading: string
            shortname: string
            shorttext: string
            views: int
            deleted: bool
            flagged: bool
            published: bool
            active: bool
            contents: string
            contentType: int
            }

    module Tags =
        let rankedTags num = 
            selectRecords<Entities.tag> (
                query "items" "alltags" database |> limitTo num
                )
            
    module Postings =
        open FunctionalDivan.Dsl.Fti

        let search text tags contentType =
            let t = 
                match tags with 
                | Some t when (List.length t) > 0 -> 
                    sprintf "text:\"%s\" or title:\"%s\" and (tag:%s)" text text 
                        (System.String.Join(" or tag:", Array.ofList (List.map (fun (e: Entities.tag) -> e.safeText)  t)))
                | _ -> sprintf "text:\"%s\" or title:\"%s\"" text text 
                
            selectRecords<Entities.posting> (
                query "items" "all" database |>
                q t
                )
                
        let byName name = 
            name |> from<Entities.posting> database