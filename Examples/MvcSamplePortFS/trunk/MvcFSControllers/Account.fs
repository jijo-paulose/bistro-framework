#light

open System.Web.Security

open System.Collections.Generic

open Bistro.Controllers
open Bistro.Controllers.Security
open Bistro.Controllers.Descriptor
open Bistro.Controllers.Descriptor.Data

open Bistro.FS.Definitions

open Authentication
open Home

let forms_auth = new FormsAuthenticationService() :> IFormsAuthentication
let membership_svc = new AccountMembershipService() :> IMembershipService

let error_code_to_string = function
| MembershipCreateStatus.DuplicateUserName ->
    "Username already exists. Please enter a different user name."
| MembershipCreateStatus.DuplicateEmail ->
    "A username for that e-mail address already exists. Please enter a different e-mail address."
| MembershipCreateStatus.InvalidPassword ->
    "The password provided is invalid. Please enter a valid password value."
| MembershipCreateStatus.InvalidEmail ->
    "The e-mail address provided is invalid. Please check the value and try again."
| MembershipCreateStatus.InvalidAnswer ->
    "The password retrieval answer provided is invalid. Please check the value and try again."
| MembershipCreateStatus.InvalidQuestion ->
    "The password retrieval question provided is invalid. Please check the value and try again."
| MembershipCreateStatus.InvalidUserName ->
    "The user name provided is invalid. Please check the value and try again."
| MembershipCreateStatus.ProviderError ->
    "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator."
| MembershipCreateStatus.UserRejected ->
    "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator."
| _ ->
    "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator."


[<Bind("/auth/changepassword")>]
[<Deny("?", OnFailure = FailAction.Redirect, Target = "/auth/logon")>]
type ChangePasswordSecurity() = inherit SecurityController()

[<ReflectedDefinition>]
[<Bind("/auth")>]
let def_auth_ct (ctx: IExecutionContext) =
    let errors, PasswordLength = new Errors(), membership_svc.MinPasswordLength
    
    errors, PasswordLength

[<ReflectedDefinition>]
[<Bind("get /auth/logon")>]
[<RenderWith("Views/Account/logon.django")>]
let logon_disp_ct (ctx: IExecutionContext) = ()

let validate_logon name pass report = 
    validate_general report
        [(System.String.IsNullOrEmpty(name), "username", "You must specify a username.");
         (System.String.IsNullOrEmpty(pass), "password", "You must specify a password.");
         (not <| membership_svc.ValidateUser (name, pass), null, "The username or password provided is incorrect.")]

[<ReflectedDefinition>]
[<Bind("post /auth/logon")>]
[<RenderWith("Views/Account/logon.django")>]
let do_logon_ct (ctx: IExecutionContext) (username: string form) (password: string form) 
    (rememberMe: bool form) (errors: Errors) = 
    let username, password, rememberMe = 
        username.Value, password.Value, rememberMe.Value
        
    if not <| validate_logon username password (report_error errors) then ()
    else
        ctx.Authenticate (forms_auth.SignIn (username, rememberMe))
        ctx.Transfer "/home/index"
        
[<ReflectedDefinition>]
[<Bind("get /auth/logoff")>]
let do_logoff_ct (ctx: IExecutionContext) =
    forms_auth.SignOut()
    ctx.Authenticate null
    ctx.Transfer "home/index"
    ()

[<ReflectedDefinition>]
[<Bind("get /auth/newuser")>]
[<RenderWith("Views/Account/register.django")>]
let register_display_ct (ctx: IExecutionContext) = ()

let validate_registration name email pass confirm_pass report = 
    validate_general report
        [(System.String.IsNullOrEmpty name, "username", "You must specify a username.");
         (System.String.IsNullOrEmpty(email), "email", "You must specify an email address.");
         (System.String.IsNullOrEmpty(pass) || pass.Length < membership_svc.MinPasswordLength, "password", (sprintf "You must specify a password of %d or more characters." membership_svc.MinPasswordLength));
         (not <| (System.String.CompareOrdinal(pass, confirm_pass) = 0), null, "The new password and confirmation password do not match.")]

[<ReflectedDefinition>]
[<Bind("post /auth/newuser")>]
[<RenderWith("Views/Account/register.django")>]
let do_register_ct (ctx: IExecutionContext) (username: string form) (email: string form) 
    (password: string form) (confirmPassword: string form) (errors: Errors) = 
    let username, email, password, confirmPassword =
        username.Value, email.Value, password.Value, confirmPassword.Value
    
    let new_user = 
        if validate_registration username email password confirmPassword (report_error errors) then
            match membership_svc.CreateUser(username, password, email) with
            | MembershipCreateStatus.Success ->
                ctx.Transfer "home/index"
                let user = forms_auth.SignIn (username, false)
                ctx.Authenticate user
                Some user
            | _ as fail_status -> 
                report_error errors null (error_code_to_string fail_status)
                None
        else None
    
    username, email, new_user, errors
        
[<ReflectedDefinition>]
[<Bind("get /auth/changepassword")>]
[<RenderWith("Views/Account/register.django")>]
let change_pass_display_ct (ctx: IExecutionContext) = ()

let validate_change_pass current_pass new_pass confirm_pass report = 
    validate_general report
        [(System.String.IsNullOrEmpty current_pass, "currentPassword", "You must specify a current password.");
         (System.String.IsNullOrEmpty new_pass || new_pass.Length < membership_svc.MinPasswordLength, "newPassword", (sprintf "You must specify a password of %d or more characters." membership_svc.MinPasswordLength));
         (not <| (System.String.CompareOrdinal(new_pass, confirm_pass) = 0), null, "The new password and confirmation password do not match.")]

[<ReflectedDefinition>]
[<Bind("get /auth/changepassword")>]
[<RenderWith("Views/Account/register.django")>]
let do_change_pass_ct (ctx: IExecutionContext) (currentPassword: string form) (newPassword: string form) 
    (confirmPassword: string form) (errors: Errors) = 
    let currentPassword, newPassword, confirmPassword =
        currentPassword.Value, newPassword.Value, confirmPassword.Value
    
    let new_user = 
        if validate_change_pass currentPassword newPassword confirmPassword (report_error errors) then
            try
                if membership_svc.ChangePassword(ctx.CurrentUser.Identity.Name, currentPassword, newPassword) then
                    ctx.Response.RenderWith "Views/Account/changePasswordSuccess.django"
                else
                    report_error errors null "The current password is incorrect or the new password is invalid."
            with | _ -> 
                report_error errors null "The current password is incorrect or the new password is invalid."
    
    errors

