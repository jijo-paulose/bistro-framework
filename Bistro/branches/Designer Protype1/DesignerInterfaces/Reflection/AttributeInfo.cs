using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bistro.Designer.DesignerInterfaces.Reflection
{
    [Serializable]
    public abstract class AttributeInfo
    {
        abstract public string Type { get; }

        [Serializable]
        public class Parameter
        {
            public Parameter(AttributeInfo owner, string value)
            {
                this.owner = owner;
                this.value = value;
            }
            string value;
            AttributeInfo owner;

            public Parameter(AttributeInfo owner)
            {
                this.owner = owner;
            }

            public string Value { get { return value; } }

            public string AsString() { return AsString(null); }

            private static Regex stringParser = new Regex("^\\s*\"(?'value'([^\"]*))\"\\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

            public string AsString(string @default) 
            {
                if (value == null)
                    return @default;
                Match match = stringParser.Match(value);
                if (match.Success)
                    return match.Groups["value"].Captures[0].ToString();
                return @default;
            }

            private static Regex typeofParser = new Regex("^\\s*typeof\\s*\\((?'value'([^\\)]*))\\)\\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

            public string AsType()
            {
                if (value == null)
                    return null;
                Match match = typeofParser.Match(value);
                if (match.Success)
                    return owner.ResolveType(match.Groups["value"].Captures[0].ToString());
                return null;
            }

            public bool? AsNBoolean() { return AsNBoolean(null); }

            public bool? AsNBoolean(bool? @default)
            {
                bool result;
                if (value != null)
                    if (bool.TryParse(value, out result))
                        return result;
                return @default;
            }

        }

        abstract public ParameterCollection Parameters { get; }

        [Serializable]
        public class ParameterCollection
        {
            public ParameterCollection(AttributeInfo owner, List<string> parameters, Dictionary<string, string> namedParameters)
            {
                this.owner = owner;
                this.parameters = parameters;
                this.namedParameters = namedParameters;
            }

            private AttributeInfo owner;
            private List<string> parameters;
            private Dictionary<int, Parameter> parameterObjects = new Dictionary<int, Parameter>();
            private Dictionary<string, string> namedParameters;
            private Dictionary<string, Parameter> namedParameterObjects = new Dictionary<string,Parameter>();

            public Parameter this[int index] 
            { 
                get 
                {
                    if (parameterObjects.ContainsKey(index))
                        return parameterObjects[index];
                    Parameter result = new Parameter(owner, parameters[index]);
                    parameterObjects.Add(index, result);
                    return result;
                } 
            }

            public Parameter this[string name] 
            { 
                get 
                {
                    if (namedParameterObjects.ContainsKey(name))
                        return namedParameterObjects[name];

                    string value;
                    Parameter result;
                    if (namedParameters.TryGetValue(name, out value))
                        result = new Parameter(owner, value);
                    else
                        result = new Parameter(owner);
                    namedParameterObjects.Add(name, result);
                    return result;
                } 
            }

            public int Count { get { return parameters.Count; } }

        }

        public abstract string ResolveType(string type);
    }
}
