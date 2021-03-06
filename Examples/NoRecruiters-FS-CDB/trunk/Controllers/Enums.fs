﻿#light

namespace NoRecruiters.Enums

open System

module Content = 
    
    type ContentType =
    | Ad = 0
    | Resume = 1
    
    let asString (contentType: ContentType) = Enum.GetName(typeof<ContentType>, contentType).ToLower()
    
    let fromString contentType = Enum.Parse(typeof<ContentType>, contentType, true) :?> ContentType
    
module User =

    type UserType =
    | Company = 0
    | Person = 1
    | Recruiter = 2
    
    let asString (contentType: UserType) = Enum.GetName(typeof<UserType>, contentType).ToLower()
    
module Common =
    open Content
    open User
    
    let asUserType = function 
    | ContentType.Resume -> UserType.Company
    | ContentType.Ad -> UserType.Person
    | _ -> UserType.Recruiter
    
    let asContentType = function
    | UserType.Company -> ContentType.Resume
    | _ -> ContentType.Ad