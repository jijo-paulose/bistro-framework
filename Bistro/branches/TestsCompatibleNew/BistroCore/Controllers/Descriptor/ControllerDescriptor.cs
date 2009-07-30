/****************************************************************************
 * 
 *  Bistro Framework Copyright © 2003-2009 Hill30 Inc
 *
 *  This file is part of Bistro Framework.
 *
 *  Bistro Framework is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Bistro Framework is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Bistro Framework.  If not, see <http://www.gnu.org/licenses/>.
 *  
 ***************************************************************************/

using System;
using System.Text;
using System.Reflection;
using Bistro.Controllers.Descriptor.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bistro.Configuration.Logging;
using Bistro.Special.Reflection;
using System.Linq;

namespace Bistro.Controllers.Descriptor
{
    /// <summary>
    /// Collection of utility methods for dealing with bind points.
    /// </summary>
    public static class BindPointUtilities
    {
        /// <summary>
        /// Regular expression for Bind structure
        /// </summary>
        private static Regex bindExpr = new Regex(@"/|\?(?!$|/)|&", RegexOptions.Compiled);

        /// <summary>
        /// Regular expression for the path-part of a bind point
        /// </summary>
        private static Regex bindPathExpr = new Regex(@"\?(?!$|/).*", RegexOptions.Compiled);

        /// <summary>
        /// A list of accepted http verbs
        /// </summary>
        public static ICollection<string> HttpVerbs = new List<string>(new string[] { "GET", "POST", "PUT", "DELETE" });

        /// <summary>
        /// Makes sure the url is [VERB/url], not [VERB url].
        /// Note that the url must be verb normalized, as this 
        /// method works off relative indices, and not actual verbs.
        /// </summary>
        /// <param name="url">The verb-qualified URL.</param>
        /// <returns></returns>
        public static string VerbNormalize(string url)
        {
            foreach (string verb in HttpVerbs)
            {
                if (!url.StartsWith(verb, StringComparison.OrdinalIgnoreCase))
                    continue;

                var remainder = url.Substring(verb.Length);
                return verb + "/" + remainder.Trim(' ', '/');
            }

            throw new ApplicationException(String.Format("\"{0}\" is not verb-qualified", url));
        }

        /// <summary>
        /// Makes sure tha the url is verb-qualified and normalized. If not qualified,
        /// the value of defaultVerb will be used to qualify the url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="defaultVerb">The default verb.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the default verb is not a valid or supported http verb.</exception>
        public static string VerbQualify(string url, string defaultVerb)
        {
            if (IsVerbQualified(url))
                return VerbNormalize(url);

            var cleanedVerb = defaultVerb.ToUpper().Trim();
            if (!HttpVerbs.Contains(cleanedVerb))
                throw new ArgumentException(String.Format("\"{0}\" is not a valid HTTP verb", cleanedVerb));

            return Combine(cleanedVerb, url);
        }

        /// <summary>
        /// Determines whether the target bind site is prefixed with an HTTP verb.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// 	<c>true</c> if the url is verb-qualified; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVerbQualified(string target)
        {
            // the verb can be specified as either "VERB url" or "VERB/url"
            var index = target.IndexOfAny(new char[] { ' ', '/' });

            // we don't want stuff that starts with a leading slash either.
            // that implies a url starting with a verb (e.g. - something.com/get/something)
            return (index > 0) && HttpVerbs.Contains(target.Substring(0, index).ToUpper());
        }

        /// <summary>
        /// Combines the specified uri1.
        /// </summary>
        /// <param name="uri1">The uri1.</param>
        /// <param name="uri2">The uri2.</param>
        /// <returns></returns>
        public static string Combine(string uri1, string uri2)
        {
            return uri1.TrimEnd('/', ' ') + '/' + uri2.TrimStart('/', ' ');
        }

        /// <summary>
        /// Gets the individual components of a Bind point
        /// </summary>
        /// <param name="bindPoint">The bind point.</param>
        /// <returns></returns>
        public static string[] GetBindComponents(string bindPoint)
        {
            return bindExpr.Split(bindPoint);
        }

        /// <summary>
        /// Trims off the query string part of a bind point, if any
        /// </summary>
        /// <param name="bindPoint">The bind point.</param>
        /// <returns></returns>
        public static string GetBindPath(string bindPoint)
        {
            return bindPathExpr.Replace(bindPoint, String.Empty);
        }
    }

    /// <summary>
    /// Manages information about a single controller. All bind matches for the same controller
    /// will be represented within a single descriptor class.
    /// </summary>
    public class ControllerDescriptor: IComparable
    {
        /// <summary>
        /// A single bind point. This struct maintains a many to one relationship with a single 
        /// controller class and describes the contents of all Bind attributes attached to it.
        /// </summary>
        public struct BindPointDescriptor
        {
            /// <summary>
            /// Gets or sets the target bind url.
            /// </summary>
            /// <value>The target.</value>
            public string Target { get; private set; }

            private string[] targetComponents;

            /// <summary>
            /// Gets or sets the controller bind type.
            /// </summary>
            /// <value>The type of the controller bind.</value>
            public BindType ControllerBindType { get; private set; }

            /// <summary>
            /// Gets or sets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public int Priority { get; private set; }

            /// <summary>
            /// Gets or sets the parameter fields.
            /// </summary>
            /// <value>The parameter fields.</value>
            public Dictionary<string, IMemberInfo> ParameterFields { get; private set; }

            public ControllerDescriptor Controller { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="BindPoint"/> struct.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="bindType">Type of the bind.</param>
            /// <param name="priority">The priority.</param>
            public BindPointDescriptor(string target, BindType bindType, int priority, ControllerDescriptor controller)
                : this()
            {
                targetComponents = BindPointUtilities.GetBindComponents(target);

                Target = BindPointUtilities.GetBindPath(target);
                ControllerBindType = bindType;
                Priority = priority;
                Controller = controller;

                parseTarget();
            }

            /// <summary>
            /// Parses the target bind url for parameter requests.
            /// </summary>
            private void parseTarget()
            {
                ParameterFields = new Dictionary<string, IMemberInfo>(); 

                for (int i = 0; i < targetComponents.Length; i++)
                {
                    string token = targetComponents[i];
                    var type = Controller.ControllerType;

                    if (token.StartsWith("{") && token.EndsWith("}"))
                    {
                        string tokenName = token.Substring(1, token.Length - 2);
                        if (type != null)
                        {
                            IMemberInfo[] members =
                                type.GetMember(
                                    tokenName,
                                    MemberTypes.Property | MemberTypes.Field,
                                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToArray();

                            if (members.Length >= 1)
                                ParameterFields.Add(tokenName, members[0]);
                            else
                            {
                                // F# members are mangled by appending an @ followed by an address
                                var mangledToken = tokenName + '@';
                                foreach (IMemberInfo member in
                                    type.GetMembers(
                                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                                {
                                    if (member.Name.StartsWith(mangledToken))
                                    {
                                        ParameterFields.Add(tokenName, member);
                                        break;
                                    }
                                }

                            }
                        }
                        else
                            // if not found, still add it, maybe someone can make use of it later.
                            ParameterFields.Add(tokenName, null);
                    }
//                        string tokenName = token.Substring(1, token.Length - 2);
//                        if (type != null)
//                        {
//                            var firstMember = type.Fields.OfType<IMemberInfo>().Union(type.Properties.OfType<IMemberInfo>()).FirstOrDefault(member => { return member.Name == tokenName; });
                            
//                                //type.GetMember(
//                                //    tokenName,
//                                //    MemberTypes.Property | MemberTypes.Field,
//                                //    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
//                            ParameterFields.Add(tokenName, firstMember);

//                            //if (members.Length >= 1)
//                            //    ParameterFields.Add(tokenName, members[0]);
//                            //else
//                            //{
//                            //    // F# members are mangled by appending an @ followed by an address
//                            //    var mangledToken = tokenName + '@';
//                            //    foreach (MemberInfo member in
//                            //        type.GetMembers(
//                            //            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
//                            //    {
//                            //        if (member.Name.StartsWith(mangledToken))
//                            //        {
//                            //            ParameterFields.Add(tokenName, member);
//                            //            break;
//                            //        }
//                            //    }

//                            //}
//                        }
//                        else
//                            // if not found, still add it, maybe someone can make use of it later.
//#warning Pay attention during debugging - when ControllerType is null?
//                            ParameterFields.Add(tokenName, null);
//                    }
                }
            }
        }

        /// <summary>
        /// A Tuple of a MemberInfo and bool describing a CookieField attribute
        /// </summary>
        public struct CookieFieldDescriptor
        {
            /// <summary>
            /// Gets or sets the field.
            /// </summary>
            /// <value>The field.</value>
            public IMemberInfo Field { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="CookieFieldDescriptor"/> is outbound.
            /// </summary>
            /// <value><c>true</c> if outbound; otherwise, <c>false</c>.</value>
            public bool Outbound { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CookieFieldDescriptor"/> struct.
            /// </summary>
            /// <param name="field">The field.</param>
            /// <param name="outbound">if set to <c>true</c> [outbound].</param>
            public CookieFieldDescriptor(IMemberInfo field, bool outbound)
                : this()
            {
                Field = field;
                Outbound = outbound;
            }
        }

        enum Messages
        {
            [DefaultMessage("{0} resource {1}.{2} doesn't have a scope specified. Defaulting to Request.")]
            UnspecifiedScope
        }

        enum Exceptions
        {
            [DefaultMessage("{0}.{1} is a duplicate field or property. Check the base classes of the controller for members with the same name.")]
            DuplicateField
        }

        /// <summary>
        /// A list of bind points linked to this controller.
        /// </summary>
        public List<BindPointDescriptor> Targets { get; protected set; }

        /// <summary>
        /// A list of context variables that affect the operation of this controller
        /// </summary>
        public List<string> DependsOn { get; protected set; }

        /// <summary>
        /// A list of context variables that this controller requires for operation
        /// </summary>
        public List<string> Requires { get; protected set; }

        /// <summary>
        /// A list of context variables that this controller places onto the context
        /// </summary>
        public List<string> Provides { get; protected set; }

        /// <summary>
        /// A list of fields marked as FormFields
        /// </summary>
        public Dictionary<string, IMemberInfo> FormFields { get; protected set; }

        /// <summary>
        /// A list of fields marked as FormFields
        /// </summary>
        public Dictionary<string, IMemberInfo> RequestFields { get; protected set; }

        /// <summary>
        /// A list of fields marked as FormFields
        /// </summary>
        public Dictionary<string, IMemberInfo> SessionFields { get; protected set; }

        /// <summary>
        /// A list of fields marked as CookieFields, along with auxilliary information (Outbound flag)
        /// </summary>
        public Dictionary<string, CookieFieldDescriptor> CookieFields { get; protected set; }

        /// <summary>
        /// The controller type
        /// </summary>
//        public MemberInfo ControllerType { get; private set; }
        public ITypeInfo ControllerType { get; private set; }


        /// <summary>
        /// Gets the name of the controller type.
        /// </summary>
        /// <value>The name of the controller type.</value>
        #warning What to do with ControllerType.Name?
//        public string ControllerTypeName { get { var tp = ControllerType as Type; if (tp == null) return ControllerType.Name; else return tp.FullName; } }
        public string ControllerTypeName { get { return ControllerType.FullName; } }

        /// <summary>
        /// Gets or sets the default template.
        /// </summary>
        /// <value>The default template.</value>
        public string DefaultTemplate { get; private set;}

        /// <summary>
        /// Our logger
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerDescriptor"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        internal ControllerDescriptor(ILogger logger)
        {
            Targets = new List<BindPointDescriptor>();
            DependsOn = new List<string>();
            Provides = new List<string>();
            Requires = new List<string>();
            FormFields = new Dictionary<string, IMemberInfo>();
            RequestFields = new Dictionary<string, IMemberInfo>();
            SessionFields = new Dictionary<string, IMemberInfo>();

            CookieFields = new Dictionary<string, CookieFieldDescriptor>();

            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerDescriptor"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        protected ControllerDescriptor(ITypeInfo t, ILogger logger)
            : this(logger)
        {
            ControllerType = t;

            // load the renderwith attribute
            IterateAttributes<RenderWithAttribute>(t,false,
                (attrib) => { DefaultTemplate = attrib.Properties["Template"].AsString(); }, null);

            // load all of the Bind attributes
            IterateAttributes<BindAttribute>(t, false,
                (attribute) => 
                    {
                        BindType bndType = (BindType)(attribute.Properties["ControllerBindType"].AsEnum());
                        int priority = (int)(attribute.Properties["Priority"].AsNInt32());
                            

                        if (BindPointUtilities.IsVerbQualified(attribute.Properties["Target"].AsString()))
                            Targets.Add(new BindPointDescriptor(
                                            BindPointUtilities.VerbNormalize(attribute.Properties["Target"].AsString()),
                                                bndType,
                                                priority,
                                            this)); 
                        else
                            // if not verb qualified, make it work for all verbs
                            foreach (string verb in BindPointUtilities.HttpVerbs)
                                Targets.Add(new BindPointDescriptor(
                                                BindPointUtilities.Combine(verb, attribute.Properties["Target"].AsString()),
                                                bndType,
                                                priority,
                                                this));
                    },
                () => 
                    {
                        foreach (string verb in BindPointUtilities.HttpVerbs)
                            Targets.Add(new BindPointDescriptor(
                                            BindPointUtilities.Combine(verb, ControllerTypeName.Replace('.', '/')), 
                                            BindType.Before, 
                                            -1, 
                                            this)); 
                    });
        }

        /// <summary>
        /// Iterates over a list of attributes taken from the targetType that are
        /// of type attributeType, with the supplied "inherit" flag.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to iterate over. This type parameter will be passed a the parameter to the nonEmpty delegate</typeparam>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <param name="nonEmpty">The statement to invoke on the elements of a non-empty result set.</param>
        /// <param name="empty">The statement to invoke on an empty result set.</param>
        //static private void IterateAttributes<T>(ITypeInfo targetType, bool inherit, Action<T> nonEmpty, Action empty)
        //{
        //    T[] attribs = targetType.GetCustomAttributes(typeof(T), inherit) as T[];

        //    if (attribs.Length > 0)
        //        foreach (T attribute in attribs)
        //            nonEmpty(attribute);
        //    else if (empty != null)
        //        empty();
        //}
        static private void IterateAttributes<T>(IHasAttributes targetType, bool inherit, Action<IAttributeInfo> nonEmpty, Action empty)
        {
//            var filteredAttrs = targetType.Attributes.Where(attrInfo => {return attrInfo.Type == typeof(T).FullName; });
            IEnumerable<IAttributeInfo> attribs = targetType.GetCustomAttributes(typeof(T), inherit);
            bool isEmpty = true;
            foreach(IAttributeInfo attribute in attribs)
            {
                nonEmpty(attribute);
                isEmpty = false;
            }
            if (isEmpty && (empty != null))
                empty();
        }




        /// <summary>
        /// Determines whether the specified target type is marked by the attribute.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="markerAttribute">The marker attribute.</param>
        /// <returns>
        /// 	<c>true</c> if the specified target type is marked; otherwise, <c>false</c>.
        /// </returns>
        static private bool IsMarked(IHasAttributes targetType, Type markerAttribute, bool inherit)
        {
            return targetType.GetCustomAttributes(markerAttribute, inherit).Count() > 0;
//            return targetType.Attributes.Any(attrInfo => { return attrInfo.Type == markerAttribute; });
        }

        /// <summary>
        /// Iterates over a list of members taken off of the target parameter based on the binding flags supplied
        /// </summary>
        /// <param name="target">the target type</param>
        /// <param name="flags">the binding flags to filter the members by</param>
        /// <param name="nonEmpty">the statement to apply to each found member</param>
        static private void IterateMembers(ITypeInfo targetClass, BindingFlags flags, Action<IMemberInfo> nonEmpty)
        {
            foreach (IMemberInfo info in targetClass.GetMembers(flags))
                nonEmpty(info);
        }
        //static private void IterateMembers(ITypeInfo targetClass, Action<IMemberInfo> nonEmpty)
        //{
        //    foreach (IMemberInfo info in targetClass.Properties)
        //        nonEmpty(info);
        //    foreach (IMemberInfo info in targetClass.Fields)
        //        nonEmpty(info);
        //}


        /// <summary>
        /// Populates the descriptor. This method will load all available data off of the 
        /// metadata placed onto the given controller type.
        /// </summary>
        /// <param name="t">The t.</param>
//        public void PopulateDescriptor()
//        {
////            var type = ControllerType as Type;
//            ITypeInfo type = ControllerType;
//            if (type == null)
//                throw new ApplicationException("This method should not be called for non-class controllers.");

//                IterateMembers(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
//                    (member) =>
//                    {
//                        try
//                        {
//                            // all fields that are not marked as required or depends-on are defaulted to "provided"
//                            if ((!IsMarked(member, typeof(RequiresAttribute), true) &&
//                                !IsMarked(member, typeof(DependsOnAttribute), true)) &&
//                                (IsMarked(member, typeof(SessionAttribute), true) ||
//                                IsMarked(member, typeof(RequestAttribute), true)))
//                                Provides.Add(member.Name);

//                            IterateAttributes<DependsOnAttribute>(member, true,
//                                (attribute) => { DependsOn.Add(attribute.Name ?? member.Name); }, null);

//                            IterateAttributes<RequiresAttribute>(member, true,
//                                (attribute) => { Requires.Add(attribute.Name ?? member.Name); }, null);

//                            IterateAttributes<ProvidesAttribute>(member, true,
//                                (attribute) => { var name = attribute.Name ?? member.Name; if (!Provides.Contains(name)) Provides.Add(name); }, null);

//                            IterateAttributes<CookieFieldAttribute>(member, true,
//                                (attribute) => { CookieFields.Add(attribute.Name ?? member.Name, new CookieFieldDescriptor(member, attribute.Outbound)); }, null);

//                            IterateAttributes<FormFieldAttribute>(member, true,
//                                (attribute) => { FormFields.Add(attribute.Name ?? member.Name, member); }, null);

//                            IterateAttributes<RequestAttribute>(member, true,
//                                (attribute) => { RequestFields.Add(attribute.Name ?? member.Name, member); }, null);

//                            IterateAttributes<SessionAttribute>(member, true,
//                                (attribute) => { SessionFields.Add(attribute.Name ?? member.Name, member); }, null);
//                        }
//                        catch (ArgumentException ex)
//                        {
//                            logger.Report(Exceptions.DuplicateField, type.Name, member.Name);

//                            throw ex;
//                        }
//                    });

//            SetDefaultResourceScope(Provides, "Provided");
//            SetDefaultResourceScope(DependsOn, "Optional");
//            SetDefaultResourceScope(Requires, "Required");
//        }
        public void PopulateDescriptor()
        {
            //            var type = ControllerType as Type;
            ITypeInfo typeInfo = ControllerType;
            if (typeInfo == null)
                throw new ApplicationException("This method should not be called for non-class controllers.");

            IterateMembers(typeInfo, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                (member) =>
                {
                    try
                    {
                        Func<IAttributeInfo, IMemberInfo, string> getName = (attr, membr) => { return (attr.Properties["Name"].AsString()) ?? membr.Name; };
                        
                        // all fields that are not marked as required or depends-on are defaulted to "provided"
                        if ((!IsMarked(member, typeof(RequiresAttribute), true) &&
                            !IsMarked(member, typeof(DependsOnAttribute), true)) &&
                            (IsMarked(member, typeof(SessionAttribute), true) ||
                            IsMarked(member, typeof(RequestAttribute), true)))
                            Provides.Add(member.Name);

                        IterateAttributes<DependsOnAttribute>(member,true,
                            (attribute) => { DependsOn.Add(getName(attribute,member)); }, null);

                        IterateAttributes<RequiresAttribute>(member,true,
                            (attribute) => { Requires.Add(getName(attribute,member)); }, null);

                        IterateAttributes<ProvidesAttribute>(member,true,
                            (attribute) => { var name = (getName(attribute,member)); if (!Provides.Contains(name)) Provides.Add(name); }, null);

                        IterateAttributes<CookieFieldAttribute>(member,true,
                            (attribute) => { CookieFields.Add(getName(attribute,member), new CookieFieldDescriptor(member, (bool)(attribute.Properties["Outbound"].AsNBoolean()))); }, null);

                        IterateAttributes<FormFieldAttribute>(member,true,
                            (attribute) => { FormFields.Add(getName(attribute,member), member); }, null);

                        IterateAttributes<RequestAttribute>(member,true,
                            (attribute) => { RequestFields.Add(getName(attribute,member), member); }, null);

                        IterateAttributes<SessionAttribute>(member,true,
                            (attribute) => { SessionFields.Add(getName(attribute,member), member); }, null);
                    }
                    catch (ArgumentException ex)
                    {
                        logger.Report(Exceptions.DuplicateField, typeInfo.FullName, member.Name);

                        throw ex;
                    }
                });

            SetDefaultResourceScope(Provides, "Provided");
            SetDefaultResourceScope(DependsOn, "Optional");
            SetDefaultResourceScope(Requires, "Required");
        }




        /// <summary>
        /// Sets the default resource scope for resources without one
        /// </summary>
        //private void SetDefaultResourceScope(List<string> resourceList, string resourceType)
        //{
        //    foreach (string resource in resourceList)
        //        if (!RequestFields.ContainsKey(resource) &&
        //            !SessionFields.ContainsKey(resource) &&
        //            !FormFields.ContainsKey(resource) &&
        //            !CookieFields.ContainsKey(resource))
        //        {
        //            MemberInfo[] member = ((Type)ControllerType).GetMember(resource, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        //            // it's an aliased field, nothing to do here.
        //            if (member == null || member.Length != 1)
        //                continue;

        //            logger.Report(Messages.UnspecifiedScope, resourceType, ControllerTypeName, resource);
        //            RequestFields.Add(resource, member[0]);
        //        }
        //}
        private void SetDefaultResourceScope(List<string> resourceList, string resourceType)
        {
            foreach (string resource in resourceList)
                if (!RequestFields.ContainsKey(resource) &&
                    !SessionFields.ContainsKey(resource) &&
                    !FormFields.ContainsKey(resource) &&
                    !CookieFields.ContainsKey(resource))
                {
                    IMemberInfo[] member = ControllerType.GetMember(resource, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToArray();

                    // it's an aliased field, nothing to do here.
                    if (member == null || member.Length != 1)
                        continue;

                    logger.Report(Messages.UnspecifiedScope, resourceType, ControllerTypeName, resource);
                    RequestFields.Add(resource, member[0]);
                }
        }

        /// <summary>
        /// Creates the descriptor.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static ControllerDescriptor CreateDescriptor(ITypeInfo t, ILogger logger)
        {
            ControllerDescriptor ret = new ControllerDescriptor(t, logger);

            ret.PopulateDescriptor();

            return ret;
        }

        /// <summary>
        /// Creates the descriptor.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="binds">The binds.</param>
        /// <param name="dependsOn">The depends on.</param>
        /// <param name="requires">The requires.</param>
        /// <param name="provides">The provides.</param>
        /// <param name="cookieFields">The cookie fields.</param>
        /// <param name="formFields">The form fields.</param>
        /// <param name="requestFields">The request fields.</param>
        /// <param name="sessionFields">The session fields.</param>
        /// <returns></returns>
        public static ControllerDescriptor CreateDescriptorRaw(ITypeInfo t, IEnumerable<string> dependsOn, IEnumerable<string> requires, 
            IEnumerable<string> provides, IDictionary<string, CookieFieldDescriptor> cookieFields, 
            IDictionary<string, IMemberInfo> formFields, IDictionary<string, IMemberInfo>
            requestFields, IDictionary<string, IMemberInfo> sessionFields, ILogger logger)
        {
            ControllerDescriptor ret = new ControllerDescriptor(t, logger);

            Action<IList<string>, IEnumerable<string>> copyList = (target, source) => 
                { if (source == null) return; foreach (string i in source) target.Add(i); };

            Action<IDictionary<string, IMemberInfo>, IDictionary<string, IMemberInfo>> copyDict = (source, target) =>
                { if (source == null) return; foreach (string key in source.Keys) target.Add(key, source[key]); };

            Action<IDictionary<string, CookieFieldDescriptor>, IDictionary<string, CookieFieldDescriptor>> copyCookieDict = (source, target) =>
                { if (source == null) return; foreach (string key in source.Keys) target.Add(key, source[key]); };

            copyList(ret.DependsOn, dependsOn);
            copyList(ret.Requires, requires);
            copyList(ret.Provides, provides);

            copyDict(ret.FormFields, formFields);
            copyDict(ret.RequestFields, requestFields);
            copyDict(ret.SessionFields, sessionFields);

            copyCookieDict(ret.CookieFields, cookieFields);

            return ret;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            var o = obj as ControllerDescriptor;

            return o.ControllerTypeName.CompareTo(ControllerTypeName);
        }
    }
}
