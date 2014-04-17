using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace System.Data.Extraction
{
    public class DBScriptExtractor
    {
        #region Declarations

        StreamReader fileReader = null;
        string scpContent = string.Empty;
        string classNamespace = string.Empty;

        #endregion

        #region Constructors

        public DBScriptExtractor()
        {
        }

        public DBScriptExtractor(string modelNamespace)
        {
            classNamespace = modelNamespace;
        }

        public DBScriptExtractor(string scriptPath, string modelNamespace)
        {
            fileReader = new StreamReader(scriptPath);
            scpContent = fileReader.ReadToEnd();
            classNamespace = modelNamespace;
        }

        public DBScriptExtractor(object scriptContent, string modelNamespace)
        {
            scpContent = scriptContent as string;
            classNamespace = modelNamespace;
        }

        #endregion

        #region Helper Methods

        private string getPascalCase(string word)
        {
            if (word.Contains("."))
                word = word.Substring(word.IndexOf('.') + 1);

            return string.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1));
        }

        private string getScriptEntityName(string scriptContent)
        {
            string tblPrefix = "TABLE ";
            string entityName = scriptContent.Substring(scriptContent.IndexOf(tblPrefix) + tblPrefix.Length);
            entityName = entityName.Substring(0, entityName.IndexOf("(") - 1)
                                   .Replace("`", string.Empty).Replace("'", string.Empty)
                                   .Replace("[", string.Empty).Replace("]", string.Empty)
                                   .Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

            return entityName;
        }

        private string getAttributeType(string scriptDataType)
        {
            string result = string.Empty;
            scriptDataType = scriptDataType.ToUpper();

            if (scriptDataType.Contains("TINYINT"))
                result = "short";
            else if (scriptDataType.Contains("BIGINT"))
                result = "long";
            else if (scriptDataType.Contains("INT"))
                result = "int";
            else if (scriptDataType.Contains("VARCHAR"))
                result = "string";
            else if (scriptDataType.Contains("CHAR"))
                result = "char";
            else if (scriptDataType.Contains("BIT"))
                result = "bool";

            return result;
        }

        private int getAttribStringLen(string attribDataType)
        {
            int result = 0;
            attribDataType = attribDataType.ToUpper();

            if (attribDataType.Contains("VARCHAR"))
            {
                string attribLen = attribDataType.Replace("[", string.Empty).Replace("]", string.Empty)
                                                 .Replace("VARCHAR(", string.Empty).Replace(")", string.Empty);
                
                int.TryParse(attribLen, out result);
            }

            return result;
        }

        private List<ModelAttribute> getScriptEntityAttributes(string scriptContent)
        {
            var result = new List<ModelAttribute>();
            var attribList = scriptContent.Substring((scriptContent.IndexOf('(') + 1));
            
            if (attribList.Contains("CONSTRAINT"))
                attribList = attribList.Substring(0, attribList.IndexOf("CONSTRAINT"));
            else
                attribList = attribList.Substring(0, attribList.IndexOf("PRIMARY KEY"));

            attribList = attribList.Replace("NULL,", "NULL|");
            attribList = attribList.Replace("AUTO_INCREMENT,", "AUTO_INCREMENT|");
            attribList = attribList.Replace("\r\n", string.Empty);

            var attribArray = attribList.Substring(0, attribList.LastIndexOf('|')).Split('|');

            foreach (var attrib in attribArray)
            {
                var newAttribute = new ModelAttribute();
                var attribConfigArray = attrib.Trim().Split(' ');

                newAttribute.AttributeColumn = attribConfigArray[0].Replace("`", string.Empty)
                                                                   .Replace("[", string.Empty).Replace("]", string.Empty).Trim();
                newAttribute.AttributeName = getPascalCase(newAttribute.AttributeColumn);
                newAttribute.AttributeType = getAttributeType(attribConfigArray[1]);
                newAttribute.Required = attrib.Contains("NOT NULL");
                newAttribute.AutoNumber = attrib.Contains("IDENTITY") || attrib.Contains("AUTO_INCREMENT");
                newAttribute.StringLength = getAttribStringLen(attribConfigArray[1]);

                result.Add(newAttribute);
            }

            setPrimaryKeyAttribute(scriptContent, ref result);

            return result;
        }

        private string compositeModelClass(Model modelPreConfig)
        {
            StringBuilder classBody = new StringBuilder();

            foreach (var nmspace in modelPreConfig.Namespaces)
                classBody.AppendLine(nmspace);

            classBody.AppendLine();
            classBody.Append("namespace ");
            classBody.AppendLine(classNamespace);
            classBody.AppendLine("{");

            classBody.Append("\tpublic class ");
            classBody.AppendLine(modelPreConfig.EntityName);
            classBody.AppendLine("\t{");

            foreach (var attrib in modelPreConfig.Attributes)
            {
                if (attrib.Annotations.Count > 0)
                    classBody.AppendLine();
                foreach(var anot in attrib.Annotations)
                    classBody.AppendLine(string.Concat("\t\t", anot));

                classBody.Append("\t\tpublic ");
                classBody.Append(string.Concat(attrib.AttributeType, " "));
                classBody.Append(string.Concat(attrib.AttributeName));
                classBody.AppendLine(" { get; set; }");
            }

            classBody.AppendLine("\t}");
            classBody.AppendLine("}");

            return classBody.ToString();
        }

        private string compositeController(string entityName, bool inMemProfileReady)
        {
            if (!inMemProfileReady)
                return ControllerTemplate.TemplateDefault.Replace("{0}", classNamespace).Replace("{1}", entityName);
            else
                return ControllerTemplate.TemplateWithAccessControl.Replace("{0}", classNamespace).Replace("{1}", entityName);
        }

        private string parseScript(Model modelPreConfig, string scriptContent)
        {
            modelPreConfig.EntityTable = getScriptEntityName(scriptContent);
            modelPreConfig.EntityName = getPascalCase(modelPreConfig.EntityTable);
            var attributeList = getScriptEntityAttributes(scriptContent);
            
            setEntityAnnotations(modelPreConfig);
            setEntityAttributeAnnotations(modelPreConfig, ref attributeList);

            modelPreConfig.Attributes = attributeList;

            return compositeModelClass(modelPreConfig);
        }

        private void setPrimaryKeyAttribute(string scriptContent, ref List<ModelAttribute> attributeList)
        {
            string constraintRange = string.Empty;

            if (scriptContent.Contains("CONSTRAINT"))
            {
                constraintRange = scriptContent.Substring(scriptContent.IndexOf("PRIMARY KEY") + "PRIMARY KEY".Length);
                constraintRange = constraintRange.Substring(0, constraintRange.IndexOf("ASC"));
            }
            else
            {
                constraintRange = scriptContent.Substring(scriptContent.IndexOf("PRIMARY KEY") + "PRIMARY KEY".Length);
                constraintRange = constraintRange.Substring(0, constraintRange.IndexOf(","));
            }

            foreach (var attrib in attributeList)
            {
                attrib.PrimaryKey = constraintRange.Contains(string.Concat("`", attrib.AttributeColumn, "`"))
                                 || constraintRange.Contains(string.Concat("[", attrib.AttributeColumn, "]"));

                attrib.Required = !attrib.PrimaryKey;
            }
        }

        private void setEntityAnnotations(Model modelPreConfig)
        {
            List<string> annotationList = new List<string>();

            if (modelPreConfig.Serializable)
                annotationList.Add("[Serializable]");

            if (modelPreConfig.ValidationEnable)
            {
                modelPreConfig.Namespaces.Add("using System.ComponentModel;");
                modelPreConfig.Namespaces.Add("using System.ComponentModel.DataAnnotations;");
            }

            if (modelPreConfig.WcfEnable)
            {
                modelPreConfig.Namespaces.Add("using System.Runtime.Serialization;");
                annotationList.Add("[DataContract(IsReference = true)]");
            }

            if (modelPreConfig.RopSqlEnable)
            {
                modelPreConfig.Namespaces.Add("using System.Data.RopSql.DataAnnotations;");
                annotationList.Add(string.Concat("[DataTable(TableName = \"", modelPreConfig.EntityTable, "\")]"));
            }

            modelPreConfig.Annotations = annotationList;
        }

        private void setEntityAttributeAnnotations(Model modelPreConfig, ref List<ModelAttribute> attributeList)
        {
            List<string> annotationList = null;

            int contAlpha = 97;
            int contComplem = 1;
            foreach (var attrib in attributeList)
            {
                if (contAlpha == 123)
                {
                    contAlpha = 97;
                    contComplem = 1;
                }

                annotationList = new List<string>();

                if (modelPreConfig.WcfEnable)
                    annotationList.Add("[DataMember]");

                if (!string.IsNullOrEmpty(attrib.DisplayName))
                    annotationList.Add(string.Concat("[DisplayName(\"", attrib.DisplayName, "\")]"));

                if (modelPreConfig.RopSqlEnable)
                {
                    var dataColumnConfig = string.Concat("[DataColumn(ColumnName = \"", attrib.AttributeColumn, "\"",
                                                         attrib.PrimaryKey ? ", PrimaryKey = true" : string.Empty,
                                                         attrib.AutoNumber ? ", AutoNumbering = true" : string.Empty,
                                                         attrib.Required ? ", Required = true" : string.Empty, ")]");

                    annotationList.Add(dataColumnConfig);
                }

                if (modelPreConfig.Serializable && modelPreConfig.JsonMinEnable)
                    annotationList.Add(string.Format("[JsonProperty(PropertyName = \"{0}\")]",
                                                    ((char)contAlpha++).ToString(), contComplem++.ToString()));

                if (modelPreConfig.ValidationEnable)
                {
                    if (attrib.Required)
                    {
                        if (!string.IsNullOrEmpty(modelPreConfig.DefaultRequiredMessage)
                            && !string.IsNullOrEmpty(attrib.DisplayName))
                        {
                            var reqMsg = string.Format(modelPreConfig.DefaultRequiredMessage, attrib.DisplayName);
                            annotationList.Add(string.Concat("[Required(\"ErrorMessage = \"", reqMsg, "\")]"));
                        }
                        else
                            annotationList.Add("[Required]");
                    }

                    if (attrib.StringLength > 0)
                        if (!string.IsNullOrEmpty(modelPreConfig.DefaultStringLengthMessage)
                            && !string.IsNullOrEmpty(attrib.DisplayName))
                        {
                            var strLenMsg = string.Format(modelPreConfig.DefaultStringLengthMessage, attrib.DisplayName);
                            annotationList.Add(string.Concat("[StringLength(", attrib.StringLength.ToString(), ", ErrorMessage = \"", strLenMsg, "\")]"));
                        }
                        else
                            annotationList.Add(string.Concat("[StringLength(", attrib.StringLength.ToString(), ")]"));
                }

                attrib.Annotations = annotationList;
            }
        }

        #endregion

        #region Public Methods

        public string[] ExtractModelClass(bool serializable, bool validationReady, bool ropSqlReady, bool wcfReady, bool jsonMinReady)
        {
            string[] result = new string[2];
            List<string> defaultNamespaces = new List<string>();
            
            defaultNamespaces.Add("using System;");
            Model modelPreConfig = new Model()
            {
                Serializable = serializable,
                ValidationEnable = validationReady,
                RopSqlEnable = ropSqlReady,
                WcfEnable = wcfReady,
                JsonMinEnable = jsonMinReady,

                Namespaces = defaultNamespaces
            };

            result[0] = parseScript(modelPreConfig, scpContent);
            result[1] = modelPreConfig.EntityName;

            return result;
        }

        public string ExtractController(string modelName, bool inMemProfileReady)
        {
            return compositeController(modelName, inMemProfileReady);
        }

        #endregion
    }
}
