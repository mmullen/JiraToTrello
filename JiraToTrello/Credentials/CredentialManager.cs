using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

namespace JiraToTrello.Credentials
{
    class CredentialManager
    {
        public JiraCredentials JiraCredentials;
        public TrelloCredentials TrelloCredentials;

        public CredentialManager()
        {   
            var trello = ConfigurationManager.GetSection("TrelloCredentials") as NameValueCollection;
            var jira = ConfigurationManager.GetSection("JiraCredentials") as NameValueCollection;

            TrelloCredentials = new TrelloCredentials();
            JiraCredentials= new JiraCredentials();

            if(trello == null || jira == null)
                throw new ConfigurationErrorsException("app.config is missing required configuration");

            SetFields(JiraCredentials, jira);
            SetFields(TrelloCredentials, trello);
        }

        public void SetFields(ICredentials cred, NameValueCollection keyVals)
        {
            foreach (var key in keyVals.AllKeys)
            {
                FieldInfo field = cred.GetType().GetField(key);
                field.SetValue(cred, keyVals[key]);
            }
        }
    }
}
