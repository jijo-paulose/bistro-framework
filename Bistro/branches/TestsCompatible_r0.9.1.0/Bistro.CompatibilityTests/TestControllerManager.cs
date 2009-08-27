﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bistro.Controllers;
using Bistro.CompatibilityTests.Reflection;
using Bistro.Controllers.Descriptor;

namespace Bistro.CompatibilityTests
{
    public class TestControllerManager : ControllerManager
    {
        public TestControllerManager(Application app) : base(app) { }


        public override void Load()
        {
            //do nothing here

        }


        public void LoadSpecial(IEnumerable<ITypeInfo> controllersList)
        {
            foreach (ITypeInfo typeInfo in controllersList)
            {

                /*
                    try
                        {
                            // all fields that are not marked as required or depends-on are defaulted to "provided"
                            if ((!IsMarked(member, typeof(RequiresAttribute), true) &&
                                !IsMarked(member, typeof(DependsOnAttribute), true)) &&
                                (IsMarked(member, typeof(SessionAttribute), true) ||
                                IsMarked(member, typeof(RequestAttribute), true)))
                                Provides.Add(member.Name);

                            IterateAttributes<DependsOnAttribute>(member, true,
                                (attribute) => { DependsOn.Add(attribute.Name ?? member.Name); }, null);

                            IterateAttributes<RequiresAttribute>(member, true,
                                (attribute) => { Requires.Add(attribute.Name ?? member.Name); }, null);

                            IterateAttributes<ProvidesAttribute>(member, true,
                                (attribute) => { var name = attribute.Name ?? member.Name; if (!Provides.Contains(name)) Provides.Add(name); }, null);

//                            IterateAttributes<CookieFieldAttribute>(member, true,
//                                (attribute) => { CookieFields.Add(attribute.Name ?? member.Name, new CookieFieldDescriptor(member, attribute.Outbound)); }, null);

//                            IterateAttributes<FormFieldAttribute>(member, true,
//                                (attribute) => { FormFields.Add(attribute.Name ?? member.Name, member); }, null);

//                            IterateAttributes<RequestAttribute>(member, true,
//                                (attribute) => { RequestFields.Add(attribute.Name ?? member.Name, member); }, null);

//                            IterateAttributes<SessionAttribute>(member, true,
//                                (attribute) => { SessionFields.Add(attribute.Name ?? member.Name, member); }, null);
                        }
                        catch (ArgumentException ex)
                        {
                            logger.Report(Exceptions.DuplicateField, type.Name, member.Name);

                            throw ex;
                        }

                 */


                //ControllerDescriptor testDescriptor = ControllerDescriptor.CreateDescriptorRaw(
                //    null,


            }
        }

        


        protected override void LoadAssembly(System.Reflection.Assembly assm)
        {
            // do nothing
        }
        protected override void LoadType(Type t)
        {
            // also do nothing here - we'll handle all the test load in the Load()
        }

        public override void RegisterController(Bistro.Controllers.Descriptor.ControllerDescriptor descriptor)
        {
            dispatcherFactory.GetDispatcherInstance().RegisterController(descriptor);
        }

    }
}