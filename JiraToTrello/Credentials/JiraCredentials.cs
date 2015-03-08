using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraToTrello.Credentials
{
    class JiraCredentials : ICredentials
    {
        public string Site;
        public string Project;
        public string User;
        public string Password;    
    }
}
