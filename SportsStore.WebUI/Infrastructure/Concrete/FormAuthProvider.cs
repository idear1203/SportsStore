using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using SportsStore.WebUI.Infrastructure.Abstract;

namespace SportsStore.WebUI.Infrastructure.Concrete
{
    public class FormAuthProvider : IAuthProvier
    {
        public bool Authenticate(string userName, string password)
        {
            bool result = FormsAuthentication.Authenticate(userName, password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(userName, false);
            }

            return result;
        }
    }
}