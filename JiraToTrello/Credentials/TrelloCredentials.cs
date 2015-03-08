using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToTrello.Credentials
{
    class TrelloCredentials : ICredentials
    {
        public string User;
        public string Email;
        public string Secret;
        public string AppKey;
        public string PublicKey;
    }
}
