using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Extraction
{
    public class ModelAttribute
    {
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool AutoNumber { get; set; }
        public bool Composition { get; set; }
        public int StringLength { get; set; }
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public string AttributeColumn { get; set; }
        public string DisplayName { get; set; }
        public List<string> Annotations { get; set; }
    }
}
