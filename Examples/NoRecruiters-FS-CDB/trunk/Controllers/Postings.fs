#light

namespace NoRecruiters.Controllers

    open Bistro.FS.Controller
    open Bistro.FS.Definitions
    open Bistro.FS.Inference
    
    open Bistro.Controllers
    open Bistro.Controllers.Descriptor
    open Bistro.Controllers.Descriptor.Data
    open Bistro.Http
    
    open System.Text.RegularExpressions
    open System.Web
    
    open NoRecruiters.Data.Enums
    open NoRecruiters.Data.Enums.Content
    open NoRecruiters.Data.Enums.User
    open NoRecruiters.Data.Enums.Common

    module Search =
        [<Bind("get /postings/{contentType}?{firstTime}"); ReflectedDefinition>]
        let firstTimeSearchC contentType firstTime defaultContentType = 
            let newContentType = if firstTime then contentType else defaultContentType
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("nrDefaultContentType", defaultContentType))
            newContentType |> named "defaultContentType"
        
        [<Bind("get /postings/{contentType}?{firstTime}")>]
        [<RenderWith("Views/Posting/search.django"); ReflectedDefinition>]
        let searchC (txtQuery: string form) currentTags =
            let getCurrentTagsAsCDL = function
            | Some v ->
                match v with
                | [] -> ""
                | _ -> v |> List.fold (fun s e -> match s with | "" -> e | _ -> s + "," + e) ""
            | None -> ""
            
            let getPopularTags tags =
                Tags.RankedTags 15 |>
                List.filter (fun e -> match tags with | Some v -> not <| List.exists ((=) e) v | None -> true) 
            
            Posting.Search txtQuery.Value (getCurrentTagsAsCDL currentTags) (Content.fromString contentType)