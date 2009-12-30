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
    
    open NoRecruiters.Enums
    open NoRecruiters.Enums.Content
    open NoRecruiters.Enums.User
    open NoRecruiters.Enums.Common
    
    open NoRecruiters.Data

    module Search =
        [<Bind("get /postings/{contentType}?{firstTime}"); ReflectedDefinition>]
        let firstTimeSearchC contentType firstTime defaultContentType = 
            let newContentType = if firstTime then contentType else defaultContentType
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("nrDefaultContentType", defaultContentType))
            newContentType |> named "defaultContentType"
        
        [<Bind("get /postings/{contentType}?{firstTime}")>]
        [<RenderWith("Views/Posting/search.django"); ReflectedDefinition>]
        let searchC (txtQuery: string form) currentTags contentType =
            let popularTags = 
                match currentTags with
                | Some l ->
                    Tags.rankedTags 15 |>
                    List.filter (fun e -> not <| List.exists ((=) e) l) 
                | None -> Tags.rankedTags 15
            
            popularTags, 
            (Postings.search (txtQuery.Value) currentTags (Content.fromString contentType)) |> named "searchResults"