using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureApimAdminSso.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private string groupToValidate = ConfigurationManager.AppSettings["ApimGroupClaim"];


        public ActionResult Index()
        {

            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            ViewBag.HasAccess = false;

            if (claimsPrincipal != null)
            {
                Claim groupApimAdmin = claimsPrincipal.Claims.FirstOrDefault(
                c => c.Type == "groups" &&
                    c.Value.Equals(groupToValidate, StringComparison.CurrentCultureIgnoreCase));

                if (null != groupApimAdmin)
                {
                    ViewBag.HasAccess = true;
                }
            }

            return View();
        }

        public async Task<ActionResult> Login()
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            string loginUrl = "";
            string ssoResult;
            JObject ssoObject;
            ViewBag.HasAccess = false;

            if (claimsPrincipal != null)
            {
                Claim groupApimAdmin = claimsPrincipal.Claims.FirstOrDefault(
                c => c.Type == "groups" &&
                    c.Value.Equals(groupToValidate, StringComparison.CurrentCultureIgnoreCase));

                if (null != groupApimAdmin)
                {
                    try
                    {
                        ssoResult = await new Helpers.ApimHelper().RetrieveSsoUrl();

                        ssoObject = JObject.Parse(ssoResult);
                        loginUrl = (string)ssoObject["value"];

                        return Redirect(loginUrl);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(string.Format("NS APIM Error: {0}, on Login to APIM.", e.Message));
                    }

                }
            }

            return RedirectToAction("Index");

        }
    }
}