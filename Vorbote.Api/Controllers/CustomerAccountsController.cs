using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Vorbote.Accounts;

namespace Vorbote.Api.Controllers
{
    public class CustomerAccountsController : ApiController
    {
        public MoqMailAccount Get(string accountname)
        {
            return AccountsRepository.Instance.GetAccount(accountname);
        }
    }
}