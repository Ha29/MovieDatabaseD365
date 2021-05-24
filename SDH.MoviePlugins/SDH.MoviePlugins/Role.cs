using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDH.MoviePlugins
{
    public class Role
    {
        public string PersonName { get; set; }
        public EntityReference Movie { get; set; }
        public EntityReference Person { get; set; }
        public OptionSetValue Position { get; set; }
    }
}
