using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.JiraRestClient;

namespace JiraToTrello.JIRA
{
    class TCIssueFields: IssueFields
    {
        /// <summary>
        /// What Actually Happened field in tcjira
        /// </summary>
        [FieldData("What Actually Happened", CustomFieldType.Description)]
        public string customfield_10056 { get; set; }

        /// <summary>
        /// Steps to Reproduce field in tcjira
        /// </summary>
        [FieldData("Steps to Reproduce", CustomFieldType.Description)]
        public string customfield_10054 { get; set; }

        /// <summary>
        /// Expectation field in tcjra
        /// </summary>
        [FieldData("Expectation", CustomFieldType.Description)]
        public string customfield_10055 { get; set; }

        /// <summary>
        /// Recommended Resolution field in tcjira
        /// </summary>
        [FieldData("Recommended Resolution", CustomFieldType.Description)]
        public string customfield_10071 { get; set; }

        /// <summary>
        /// Assigned implementor in tcjira
        /// </summary>
        [FieldData("Assigned Implementor", CustomFieldType.Description)]
        public Implementor customfield_10101 { get; set; }

        /// <summary>
        /// Assigned reviewer in tcjira
        /// </summary>
        [FieldData("Assigned Reviewer", CustomFieldType.Description)]
        public Reviewer customfield_10072 { get; set; }

        /// <summary>
        /// Override (hackily) of comments to capture commentor in tcjira
        /// </summary>
        [FieldData(CustomFieldType.Comment)]
        public Comment comment { get; set; }
    }

    class Implementor
    {
        [FieldData("Name", CustomFieldType.Description)]
        public string displayName { get; set; }
    }

    class Reviewer
    {
        [FieldData("Name", CustomFieldType.Description)]
        public string displayName { get; set; }
    }

    class Comment
    {
        [FieldData(CustomFieldType.Description)]
        public List<CommentList> comments { get; set; } 
    }

    class CommentList
    {
        [FieldData("Comment", CustomFieldType.Comment)]
        public string body { get; set; }
        [FieldData("Author", CustomFieldType.Comment)]
        public Author author { get; set; }
        [FieldData("Created", CustomFieldType.Comment)]
        public string created { get; set; }
    }

    class Author
    {
        [FieldData("Name", CustomFieldType.Comment)]
        public string displayName { get; set; }
    }
}
