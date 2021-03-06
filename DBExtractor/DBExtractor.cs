﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data.Extraction.Messages;

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

        private string getAttributeType(string scriptDataType, bool nullable)
        {
            string result = string.Empty;
            scriptDataType = scriptDataType.ToUpper();

            if (scriptDataType.Contains("TINYINT"))
                result = "short";
            else if (scriptDataType.Contains("BIGINT"))
                result = "long";
            else if (scriptDataType.Contains("INT"))
                result = "int";
            else if (scriptDataType.Contains("FLOAT"))
                result = "float";
            else if (scriptDataType.Contains("DOUBLE"))
                result = "double";
            else if (scriptDataType.Contains("DECIMAL"))
                result = "decimal";
            else if (scriptDataType.Contains("VARCHAR"))
                result = "string";
            else if (scriptDataType.Contains("CHAR"))
                result = "char";
            else if (scriptDataType.Contains("BIT"))
                result = "bool";
            else if (scriptDataType.Contains("DATETIME"))
                result = "DateTime";

            if (nullable && (result != "string"))
                result = string.Concat(result, "?");

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

        private ModelAttribute getCompositionEntity(string attribName, string attribColumn)
        {
            return new ModelAttribute() { Composition = true,
                                          AttributeName = getPascalCase(attribName.Replace("Id", string.Empty)),
                                          AttributeType = getPascalCase(attribName.Replace("Id", string.Empty)),
                                          AttributeColumn = attribColumn };
        }

        private List<ModelAttribute> getScriptEntityAttributes(string scriptContent)
        {
            var result = new List<ModelAttribute>();
            var attribList = scriptContent.Substring((scriptContent.IndexOf('(') + 1));
            var composition = new List<ModelAttribute>();

            if (attribList.Contains("CONSTRAINT"))
                attribList = attribList.Substring(0, attribList.IndexOf("CONSTRAINT"));
            else
                attribList = attribList.Substring(0, attribList.IndexOf("PRIMARY KEY"));

            attribList = attribList.Replace("NULL,", "NULL|");
            attribList = attribList.Replace("AUTO_INCREMENT,", "AUTO_INCREMENT|");
            attribList = attribList.Replace("b'1',", "b'1'|");
            attribList = attribList.Replace("\r\n", string.Empty);

            var attribArray = attribList.Substring(0, attribList.LastIndexOf('|')).Split('|');

            foreach (var attrib in attribArray)
            {
                var newAttribute = new ModelAttribute();
                var attribConfigArray = attrib.Trim().Split(' ');
                var requiredAttrib = attrib.Contains("NOT NULL");
                var identityAttrib = attrib.Contains("IDENTITY") || attrib.Contains("AUTO_INCREMENT");

                newAttribute.Required = requiredAttrib;
                newAttribute.AutoNumber = identityAttrib;
                newAttribute.AttributeColumn = attribConfigArray[0].Replace("`", string.Empty)
                                                                   .Replace("[", string.Empty).Replace("]", string.Empty).Trim();
                newAttribute.AttributeName = getPascalCase(newAttribute.AttributeColumn);
                newAttribute.AttributeType = getAttributeType(attribConfigArray[1], (!requiredAttrib && !identityAttrib));
                newAttribute.StringLength = getAttribStringLen(attribConfigArray[1]);

                result.Add(newAttribute);

                if (newAttribute.AttributeName.StartsWith("Id")
                    && (newAttribute.AttributeName.Length > 2))
                    composition.Add(getCompositionEntity(newAttribute.AttributeName, newAttribute.AttributeColumn));
            }

            setPrimaryKeyAttribute(scriptContent, ref result);
            result.AddRange(composition);

            return result;
        }

        private string compositeModelClass(Model modelPreConfig)
        {
            StringBuilder classBody = new StringBuilder();

            foreach (var nmspace in modelPreConfig.Namespaces)
                classBody.AppendLine(nmspace);

            classBody.AppendLine();
            classBody.Append("namespace ");
            classBody.Append(classNamespace);
            classBody.AppendLine(".Models");
            classBody.AppendLine("{");

            foreach (var annot in modelPreConfig.Annotations)
                classBody.AppendLine(string.Concat("\t", annot));

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

        private string compositeController(string modelName, bool exceptionManager, bool inMemProfileReady, bool systemRegistry, bool customWorkflow, bool gzipReady, int excepDepth)
        {
            var result = string.Empty;
            var compressTag = "[CompressResult]\r\n    ";

            if (!exceptionManager && !inMemProfileReady && !systemRegistry && !customWorkflow)
                result = ControllerTemplate.TemplateDefault.Replace("{0}", classNamespace).Replace("{1}", modelName);
            else
            {
                if (exceptionManager && !inMemProfileReady && !systemRegistry && !customWorkflow)
                    result = ControllerTemplate.TemplateWithExceptionManager.Replace("{0}", classNamespace).Replace("{1}", modelName);
                else
                    if (inMemProfileReady && !systemRegistry && !customWorkflow)
                        result = ControllerTemplate.TemplateWith_ExcepMan_AndAccessControl.Replace("{0}", classNamespace).Replace("{1}", modelName);
                    else if (!customWorkflow)
                        result = ControllerTemplate.TemplateWithExMn_AcCtl_AndRegistry.Replace("{0}", classNamespace).Replace("{1}", modelName);
                    else
                        result = ControllerTemplate.TemplateWithExMn_AcCt_Reg_AndWorkFlow.Replace("{0}", classNamespace).Replace("{1}", modelName);
            }

            result = result.Replace("{2}", gzipReady ? compressTag : string.Empty);

            result = result.Replace("{3}", (excepDepth > 0) ? excepDepth.ToString() : string.Empty);

            return result;
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

            if (scriptContent.Contains("CONSTRAINT")
                && scriptContent.IndexOf("CONSTRAINT") < scriptContent.IndexOf("PRIMARY KEY"))
            {
                constraintRange = scriptContent.Substring(scriptContent.IndexOf("PRIMARY KEY") + "PRIMARY KEY".Length);
                constraintRange = constraintRange.Substring(0, constraintRange.IndexOf("ASC"));
            }
            else
            {
                constraintRange = scriptContent.Substring(scriptContent.IndexOf("PRIMARY KEY") + "PRIMARY KEY".Length);

                int constraintEnd = constraintRange.IndexOf(",");
                if (constraintRange.IndexOf(",") < 0)
                    constraintEnd = constraintRange.IndexOf("\n");
                
                constraintRange = constraintRange.Substring(0, constraintEnd);
            }

            foreach (var attrib in attributeList)
            {
                attrib.PrimaryKey = constraintRange.Contains(string.Concat("`", attrib.AttributeColumn, "`"))
                                 || constraintRange.Contains(string.Concat("[", attrib.AttributeColumn, "]"));

                if (attrib.PrimaryKey) attrib.Required = false;
            }
        }

        private string getValidationMsg(ValidationType validationType)
        {
            var result = string.Empty;

            switch (validationType)
            {
                case ValidationType.Required:
                    result =  Resources.ValidationMessage.RequiredAttribute;
                    break;
                case ValidationType.StringLength:
                    result = Resources.ValidationMessage.StringLenExceeded;
                    break;
                case ValidationType.ForeignKey:
                    result = Resources.ValidationMessage.RequiredRelation;
                    break;
                case ValidationType.Numeric:
                    result = Resources.ValidationMessage.NumericValue;
                    break;
                case ValidationType.NumericInteger:
                    result = Resources.ValidationMessage.NumericIntegerValue;
                    break;
            }

            return result;
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

            if (modelPreConfig.NavMenu)
            {
                modelPreConfig.Namespaces.Add("using System.Security.InMemProfile.Annotations;");
                annotationList.Add(string.Concat("[DisplayName(\"", modelPreConfig.EntityName, "\")]"));
                annotationList.Add(string.Concat("[Funcionality(FuncionalityGroup = \"", modelPreConfig.FuncionalityGroup, "\",", Environment.NewLine, 
                                                 "                      FuncionalitySubGroup = \"", modelPreConfig.FuncionalitySubGroup, "\",", Environment.NewLine,
                                                 "                      FuncionalityAccess = \"/", modelPreConfig.EntityName, "\")]"));
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

                annotationList.Add(string.Concat("[DisplayName(\"", attrib.AttributeName, "\")]"));

                if (modelPreConfig.RopSqlEnable && !attrib.Composition)
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
                        if (modelPreConfig.ValidationMsgEnable)
                        {
                            var foreignKey = attrib.AttributeName.StartsWith("Id");
                            var reqMsg = string.Format(getValidationMsg(!foreignKey ? ValidationType.Required : ValidationType.ForeignKey), 
                                                                        !foreignKey ? attrib.AttributeName : 
                                                                        getCompositionEntity(attrib.AttributeName, attrib.AttributeColumn).AttributeName);
                            annotationList.Add(string.Concat("[Required(ErrorMessage = \"", reqMsg, "\")]"));
                        }
                        else
                            annotationList.Add("[Required]");

                    if (attrib.StringLength > 0)
                        if (modelPreConfig.ValidationMsgEnable)
                        {
                            var strLenMsg = string.Format(getValidationMsg(ValidationType.StringLength), attrib.AttributeName, attrib.StringLength);
                            annotationList.Add(string.Concat("[StringLength(", attrib.StringLength.ToString(), ", ErrorMessage = \"", strLenMsg, "\")]"));
                        }
                        else
                            annotationList.Add(string.Concat("[StringLength(", attrib.StringLength.ToString(), ")]"));

                    var numericTypes = new List<string> { "decimal", "float", "double" };
                    var numericTypesInt = new List<string> { "byte", "short", "int", "long" };
                    var numAttribMsg = string.Format(getValidationMsg(ValidationType.Numeric), attrib.AttributeName);
                    var numAttribMsgInt = string.Format(getValidationMsg(ValidationType.NumericInteger), attrib.AttributeName);
                    if (attrib.Required && modelPreConfig.ValidationMsgEnable)
                    {
                        if (numericTypes.Contains(attrib.AttributeType))
                            annotationList.Add(string.Concat("[RegularExpression(\"^[0-9]*\\\\.?\\\\,?[0-9]+$\", ErrorMessage = \"", numAttribMsg, "\")]"));
                        else if (numericTypesInt.Contains(attrib.AttributeType))
                            annotationList.Add(string.Concat("[RegularExpression(\"^[0-9]*$\", ErrorMessage = \"", numAttribMsgInt, "\")]"));
                    }
                }

                if (attrib.Composition)
                {
                    StringBuilder compositAnnot = new StringBuilder();
                    compositAnnot.AppendLine("[RelatedEntity(Cardinality = RelationCardinality.OneToOne,");
                    compositAnnot.AppendLine(string.Concat("\t\t\t\t\t   ForeignKeyAttribute = \"", getPascalCase(attrib.AttributeColumn), "\","));
                    compositAnnot.Append("\t\t\t\t\t   RecordableComposition = false)]"); 
                    annotationList.Add(compositAnnot.ToString());
                }

                attrib.Annotations = annotationList;
            }
        }

        #endregion

        #region Public Methods

        public string[] ExtractModelClass(bool serializable, bool validationReady, bool validationMsgReady, bool ropSqlReady, bool gzipReady, bool wcfReady, bool jsonMinReady, FuncionalityConfig funcConfig)
        {
            string[] result = new string[2];
            List<string> defaultNamespaces = new List<string>();
            
            defaultNamespaces.Add("using System;");
            Model modelPreConfig = new Model()
            {
                Serializable = serializable,
                ValidationEnable = validationReady || validationMsgReady,
                ValidationMsgEnable = validationMsgReady,
                RopSqlEnable = ropSqlReady,
                GzipEnable = gzipReady,
                WcfEnable = wcfReady,
                JsonMinEnable = jsonMinReady,
                NavMenu = (funcConfig != null),

                Namespaces = defaultNamespaces
            };

            if (modelPreConfig.NavMenu)
            {
                modelPreConfig.FuncionalityGroup = funcConfig.Group;
                modelPreConfig.FuncionalitySubGroup = funcConfig.SubGroup;
                modelPreConfig.FuncionalityAccess = funcConfig.Access;
            }

            result[0] = parseScript(modelPreConfig, scpContent);
            result[1] = modelPreConfig.EntityName;

            return result;
        }

        public string ExtractController(string modelName, bool exceptionManager, bool inMemProfileReady, bool systemRegistry, bool customWorkflow, bool gzipReady, int excepDepth)
        {
            return compositeController(modelName, exceptionManager, inMemProfileReady, systemRegistry, customWorkflow, gzipReady, excepDepth);
        }

        #endregion
    }
}
