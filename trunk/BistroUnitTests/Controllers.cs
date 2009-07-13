using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;
using Bistro.Controllers.Descriptor;
using Bistro.Controllers.Descriptor.Data;
using Bistro.Validation;
using Bistro.Extensions.Validation.Common;
using Bistro.Extensions.Validation;

namespace Bistro.UnitTests
{
    //public class Member: IValidatable
    //{
    //    public string firstName;

    //    public IValidator Validator
    //    {
    //        get { 
    //            //return new Validator<Member>()
    //            //                .Value(c => c.firstName)
    //            //                    .IsLongerThan(3, "minimum of 3 characters required");

    //            return new Validator<Entity.member>();
    //        }
    //    }

    //    public List<string> Messages
    //    {
    //        get;
    //        set;
    //    }

    //    public bool IsValid
    //    {
    //        get;
    //        set;
    //    }
    //}

    [Bind("/validationTest")]
    public class ValidatingController : AbstractController, IValidatable
    {
        public string someField;

        public string firstName;

        [Request]
        public List<string> Messages { get; set; }
        public bool IsValid { get; set; }

        public IValidator Validator
        {
            get
            {
                var v = new Validator<ValidatingController>();

                v
                    .As("validationTest")
                    .Value(c => c.someField)
                        .IsRequired("someField is required");
                    //.WithRulesFrom(new Member());

                return v;
            }
        }

        public override void DoProcessRequest(IExecutionContext context)
        {
            //var member = new Member();
            //member.firstName = firstName;

            //member.Validator.IsValid(member, ...);
        }
    }


    [Bind("/")]
    public class HomeUrlController1 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/")]
    public class HomeUrlController2 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/hello/?/you")]
    public class HelloYouController1 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Bind("/hello/*/you")]
    public class HelloYouController2 : AbstractController
    {
        public override void DoProcessRequest(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
    [Bind("/order/world/new")]
    public class OrderController1 : AbstractController
    {   [Session]
        public string c1;
        [Session, Requires]
        public string c2;
        
        public override void DoProcessRequest(IExecutionContext context)
        {
            c1 = c2 + "1";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController2 : AbstractController
    {
        [Session]
        public string c2;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c2 = c5 + "2";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController3 : AbstractController
    {
        [Session]
        public string c3;

        [Session, Requires]
        public string c2;
        [Session, Requires]
        public string c4;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c3 = c2 + c4 + c5 + "3";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController4 : AbstractController
    {
        [Session]
        public string c4;

        [Session, Requires]
        public string c1;
        [Session, Requires]
        public string c2;
        [Session, Requires]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c4 = c1 + c2 + c5 + "4";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController5 : AbstractController
    {
        [Session]
        public string c5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c5 = "5";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController6 : AbstractController
    {
        [Session]
        public string c6;
        [Session, Requires]
        public string c3;
        [Session, Requires]
        public string c7;


        public override void DoProcessRequest(IExecutionContext context)
        {
            c6 = c3 + c7 + "6";
        }
    }
    [Bind("/order/world/new")]
    public class OrderController7 : AbstractController
    {
        [Session]
        public string c7;

        public override void DoProcessRequest(IExecutionContext context)
        {
            c7 = "7";
        }
    }
    
    [Bind("/one_little_url")]
    public class littleController1 : AbstractController
    {
        [Request]
        public string l1;
        [Request, Requires]
        public string l2;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l1 = "1" + l2;
        }
    }
    [Bind("/one_little_url")]
    public class littleController2 : AbstractController
    {
        [Request]
        public string l2;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l2 = "2";
        }
    }
    [Bind("/little_url/more")]
    public class littleController3 : AbstractController
    {
        [Request]
        public string l3;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l3 = "3";
        }
    }
    [Bind("/little_url/more")]
    public class littleController4 : AbstractController
    {
        [Request]
        public string l4;
        [Request, Requires]
        public string l3;
        [Request, Requires]
        public string l5;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l4 = "4" + l3 + l5;
        }
    }
    [Bind("/little_url/more")]
    public class littleController5 : AbstractController
    {
        [Request]
        public string l5;
        [Request, Requires]
        public string l3;

        public override void DoProcessRequest(IExecutionContext context)
        {
            l5 = "5" + l3;
        }
    }
}
