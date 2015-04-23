using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TechTalk.JiraRestClient;

namespace JiraToTrello.JIRA
{
    //TODO: These should inherit from a parent
    class JiraBot<T> where T : IssueFields, new()
    {
        private readonly IJiraClient<T> _jira;

        public JiraBot(IJiraClient<T> jira)
        {
            _jira = jira;
        }

        public IList<Issue<T>> GetIssueDetails(IList<string> issues)
        {
            var foundIssues = new List<Issue<T>>();
            foreach (var issue in issues)
            {
                foundIssues.Add(_jira.LoadIssue(issue));
            }
            return foundIssues;
        }

    }

    class JiraBot
    {
        private readonly IJiraClient _jira;

        public JiraBot(IJiraClient jira)
        {
            _jira = jira;
        }

        public IList<Issue> GetIssueDetails(IList<string> issues)
        {
            var foundIssues = new List<Issue>();
            foreach (var issue in issues)
            {
                foundIssues.Add(_jira.LoadIssue(issue));
            }

            return foundIssues;
        }

    }
}
