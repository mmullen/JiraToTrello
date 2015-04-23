using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraToTrello.Credentials;
using TechTalk.JiraRestClient;

namespace JiraToTrello.JIRA
{
    //TODO: Make this inherit from an interface/base class
    class JiraConnector
    {
        private JiraCredentials _jiraCredentials;

        public JiraConnector(JiraCredentials jiraCredentials)
        {
            _jiraCredentials = jiraCredentials;
        }

        public IJiraClient<T> GetAuthorizedConnection<T>() where T : IssueFields, new()
        {
            var jira = new JiraClient<T>(_jiraCredentials.Site, _jiraCredentials.User, _jiraCredentials.Password);
            return jira;
        }

        public IJiraClient GetAuthorizedConnection()
        {
            var jira = new JiraClient(_jiraCredentials.Site, _jiraCredentials.User, _jiraCredentials.Password);
            return jira;
        } 
    }
}
