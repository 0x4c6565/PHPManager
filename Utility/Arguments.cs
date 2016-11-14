using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web.Management.PHP.Utility
{
    public class Arguments
    {
        private StringDictionary Parameters { get; set; }
        private string WaitingParameter { get; set; }
        private Regex Splitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private Regex Remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        public Arguments(string[] args)
        {
            this.Parameters = new StringDictionary();
            string[] parts;

            // Valid parameter format:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=value2 -param5 '--=test=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                parts = Splitter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        AddWaitingParameterUnsanitizedValue(parts[0]);
                        break;

                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting with no value, set it to true.
                        AddWaitingParameterValue("true");
                        WaitingParameter = parts[1];
                        break;

                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting with no value, set it to true.
                        AddWaitingParameterValue("true");
                        WaitingParameter = parts[1];

                        // Remove possible enclosing characters (",') and add value
                        AddWaitingParameterUnsanitizedValue(parts[2]);
                        break;
                }
            }

            // In case a parameter is still waiting
            AddWaitingParameterValue("true");
        }

        public void AddWaitingParameterUnsanitizedValue(string value)
        {
            AddWaitingParameterValue(Remover.Replace(value, "$1"));
        }

        public void AddWaitingParameterValue(string value)
        {
            if (this.WaitingParameter != null)
            {
                Add(WaitingParameter, value);
                WaitingParameter = null;
            }
        }

        public void Add(string parameter, string value)
        {
            if (!Parameters.ContainsKey(parameter))
            {
                Parameters.Add(parameter, value);
            }
        }

        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string Param]
        {
            get
            {
                return (Parameters[Param]);
            }
        }
    }
}
