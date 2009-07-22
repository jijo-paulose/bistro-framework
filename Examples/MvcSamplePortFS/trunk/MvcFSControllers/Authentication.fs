#light

open System
open System.Collections.Generic
open System.Web
open System.Web.Security
open System.Security.Principal
open System.Threading

type IFormsAuthentication =
    abstract SignIn: string * bool -> IPrincipal
    abstract SignOut: unit -> unit

type IMembershipService =
    abstract MinPasswordLength: int
    abstract ValidateUser: string * string -> bool
    abstract CreateUser: string * string * string -> MembershipCreateStatus
    abstract ChangePassword: string * string * string -> bool
    
type FormsAuthenticationService() =
    interface IFormsAuthentication with
        member x.SignIn (user, persist) =
            FormsAuthentication.SetAuthCookie(user, persist)
            new GenericPrincipal((new GenericIdentity (user) :> IIdentity), [||]) :> IPrincipal
            
        member x.SignOut() = FormsAuthentication.SignOut()
        
type AccountMembershipService() =
    let provider = Membership.Provider
    
    interface IMembershipService with
        member x.MinPasswordLength = provider.MinRequiredPasswordLength
        
        member x.ValidateUser (name, pass) = provider.ValidateUser(name, pass)
        
        member x.CreateUser (name, pass, email) =
            let _, status = provider.CreateUser(name, pass, email, null, null, true, null)
            status
            
        member x.ChangePassword (name, old_pass, new_pass) =
            let current = provider.GetUser (name, true)
            current.ChangePassword (old_pass, new_pass)