using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.MinSide.Web.Controllers
{
    public class BaseController : Controller
    {
        protected string GetCurrentUserOrganizationName()
        {
            return ClaimsPrincipalUtility.GetCurrentUserOrganizationName(User);
        }

        protected string GetUsername()
        {
            return ClaimsPrincipalUtility.GetUsername(User);
        }

        protected bool UserHasMetadataAdminRole()
        {
            return ClaimsPrincipalUtility.UserHasMetadataAdminRole(User);
        }

        protected bool UserHasEditorRole()
        {
            return ClaimsPrincipalUtility.UserHasMetadataEditorRole(User);
        }
    }
}