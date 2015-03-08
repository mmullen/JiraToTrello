using System;
using JiraToTrello.Credentials;
using TrelloNet;

namespace JiraToTrello.Trello
{
    //TODO: Make this inherit from an interface/base class
    class TrelloConnector
    {
        private TrelloCredentials trelloCredentials;
        public TrelloConnector(TrelloCredentials trelloCredentials)
        {
            this.trelloCredentials = trelloCredentials;
        }

        public ITrello GetAuthorizedConnection()
        {
            var trello = new TrelloNet.Trello(trelloCredentials.PublicKey);

            var url = trello.GetAuthorizationUrl("JiraToTrello", Scope.ReadWrite, Expiration.Never);
            Console.WriteLine("If you have not already set it, go to the following link and " +
                                            "set the appkey to the value given by following this link" +
                                            "while logged into the 'bot' account: {0}", url);

            trello.Authorize(trelloCredentials.AppKey);

            

            return trello;
        }
    }
}
