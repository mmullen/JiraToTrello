using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToTrello.JIRA
{
    public enum CustomFieldType
    {
        Description,
        Comment
    }

    /// <summary>
    /// Supplies metadata (Name of the field and where to put it in trello) to a defined custom field
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldDataAttribute : Attribute
    {
        protected string fieldName;
        protected CustomFieldType fieldType;

        public string FieldName
        {
            get { return fieldName; }
        }

        public CustomFieldType FieldType
        {
            get { return fieldType; }
        }

        /// <summary>
        /// Creates a new FieldData attribute. Takes in the name of the field being used, and where to put it
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        public FieldDataAttribute(String fieldName, CustomFieldType fieldType)
        {
            this.fieldName = fieldName;
            this.fieldType = fieldType;
        }

        /// <summary>
        /// Creates a new FieldData attribute. Defaults the field name to empty
        /// </summary>
        /// <param name="fieldType"></param>
        public FieldDataAttribute(CustomFieldType fieldType)
        {
            this.fieldName = String.Empty;
            this.fieldType = fieldType;
        }
    }
}
