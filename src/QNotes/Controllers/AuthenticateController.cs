using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QNotes.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authType = HttpContext.Request.Query["authscheme"];
            if (!string.IsNullOrEmpty(authType))
            {
                // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
                // send them to the home page instead (/).
                await HttpContext.Authentication.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
               
            }

            HttpContext.Response.ContentType = "text/html";
            await HttpContext.Response.WriteAsync("<html><body>");
            await HttpContext.Response.WriteAsync("Choose an authentication scheme: <br>");
            foreach (var type in HttpContext.Authentication.GetAuthenticationSchemes())
            {
                if (type.AuthenticationScheme.ToLower() == "google")
                    await HttpContext.Response.WriteAsync("<a href=\"?authscheme=" + type.AuthenticationScheme + "\">" + (type.DisplayName ?? "(suppressed)") + "</a><br>");
            }
            await HttpContext.Response.WriteAsync("</body></html>");
        }
    }
}
