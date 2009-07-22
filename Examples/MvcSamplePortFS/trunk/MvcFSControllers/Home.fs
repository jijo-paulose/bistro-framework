#light

open Bistro.Controllers
open Bistro.Controllers.Descriptor
open Bistro.Controllers.Descriptor.Data

open Bistro.FS.Definitions

open Microsoft.FSharp.Quotations

open System.Web
open System.Collections.Generic

type Errors = Dictionary<string, string>

let report_error (errors: Errors) errorKey error = 
    let error_key =
        match errorKey with 
        | null -> errors.Count.ToString()
        | _ -> errorKey
        
    if errors.ContainsKey error_key then
        errors.[error_key] <- errors.[error_key] + error
    else
        errors.Add(error_key, error)

let validate_general report rules =
    let count =
        rules |>
        List.fold_left (fun count ex -> 
                            let cond, target, msg = ex
                            if cond then 
                                report target msg
                                count+1
                            else count) 0
    
    count = 0

[<ReflectedDefinition>]
[<Bind("?")>]
let def_ct (ctx: IExecutionContext) = 
    let user = ctx.CurrentUser
    let root = 
        if HttpContext.Current.Request.ApplicationPath = "/" then System.String.Empty
        else "/" + HttpContext.Current.Request.ApplicationPath.Trim([|'/'|])
        
    user, root

[<ReflectedDefinition>]
[<Bind("get /home/index")>]
[<RenderWith("Views/Home/index.django")>]
let home_ct (ctx: IExecutionContext) = 
    let Message = "Welcome to Bistro.FS!"
    Message

[<ReflectedDefinition>]
[<Bind("get /home/about")>]
[<RenderWith("Views/Home/about.django")>]
let about_ct (ctx: IExecutionContext) = ()
