#light

namespace NoRecruiters.Controllers

    open Bistro.FS.Controller
    open Bistro.FS.Definitions
    open Bistro.FS.Inference
    
    open Bistro.Controllers
    open Bistro.Controllers.Descriptor
    open Bistro.Controllers.Descriptor.Data
    
    module Base =
        [<Bind("?"); RenderWith("Views/index.django"); ReflectedDefinition>]
        let helloC (ctx: ictx) = 
            "hello world" |> named "Message"