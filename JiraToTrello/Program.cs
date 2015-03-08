using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JiraToTrello.Credentials;
using JiraToTrello.JIRA;
using JiraToTrello.Trello;
using TechTalk.JiraRestClient;
using TrelloNet;

namespace JiraToTrello
{
    class Program
    {
        static void Main(string[] args)
        {
            //Connect to trello,using credentials from a config file
            //Find all comments mentioning the connected user
            //See which SER (or whatever) that they mention - Track the card+issue it mentions
            //Connect to JIRA using credentials from a config file
            //For each mentioned issue in Trello, get the corresponding issue in JIRA, and populate the card.


            var credManager = new CredentialManager();

            var trelloConnector = new TrelloConnector(credManager.TrelloCredentials);
            var jiraConnector = new JiraConnector(credManager.JiraCredentials);

            ITrello trello = trelloConnector.GetAuthorizedConnection();
            IJiraClient jira = jiraConnector.GetAuthorizedConnection();

            var trelloBot = new TrelloBot(trello, credManager.JiraCredentials.Project);

            var issues = trelloBot.GetIssuesToTrack();

            

            Thread.Sleep(10000);
        }
    }
}
