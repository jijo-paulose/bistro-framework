using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Bistro.Extensions.Validation
{
    /// <summary>
    /// A class member to to be validated
    /// </summary>
    /// <typeparam name="T">the type of the owner class</typeparam>
    /// <typeparam name="K">the type of the member to be validated</typeparam>
    public class ValidationSite<T,K>
    {
        MemberInfo member;

        public ValidationSite(Expression<Func<T, K>> expr)
        {
            var body = expr.Body as MemberExpression;
            var members = body.Member.DeclaringType.GetMember(
                            body.Member.Name, 
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if (members.Length > 0)
                throw new ApplicationException(String.Format("'{0}' is a non-unique identifier, and cannot be used as a validation site", body.ToString()));

            member = members[0];
        }
    }
}
