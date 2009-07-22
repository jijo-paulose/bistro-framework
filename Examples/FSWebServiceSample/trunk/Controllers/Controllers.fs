#light

open System.Xml

open Bistro
open Bistro.Controllers
open Bistro.Controllers.Descriptor
open Bistro.Controllers.Descriptor.Data

open Bistro.FS.Definitions

let rec fibonacci a b rem = 
    if (rem = 0) then [a]
    else [a] @ fibonacci b (a+b) (rem-1)

[<ReflectedDefinition>]
[<Bind("get /fib/{numbers}")>]
[<RenderWith(@"Templates\fibResult.django")>]
let fibCt (ctx: IExecutionContext) numbers = 
    let fibNbrs = fibonacci 0 1 numbers
    fibNbrs
