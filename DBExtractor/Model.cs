using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extraction
{
    public class Model
    {
        public bool Serializable { get; set; }
        public bool ValidationEnable { get; set; }
        public bool RopSqlEnable { get; set; }
        public bool WcfEnable { get; set; }
        public bool JsonMinEnable { get; set; }
        public string EntityName { get; set; }
        public string EntityTable { get; set; }
        public string DisplayName { get; set; }
        public string DefaultRequiredMessage { get; set; }
        public string DefaultStringLengthMessage { get; set; }
        public List<string> Namespaces { get; set; }
        public List<string> Annotations { get; set; }
        public List<ModelAttribute> Attributes { get; set; }
    }
}
