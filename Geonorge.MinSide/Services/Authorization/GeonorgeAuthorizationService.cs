using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Geonorge.MinSide.Services.Authorization;
using Serilog;

namespace Geonorge.MinSide.Utils
{
    /// <summary>
    /// Retrieves information from Geonorges authorization service, also known as BAAT.
    /// </summary>
    public class GeonorgeAuthorizationService : IGeonorgeAuthorizationService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public const string ClaimIdentifierRole = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        public const string ClaimIdentifierUsername = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string GeonorgeRoleNamePrefix = "nd.";

        private readonly IBaatAuthzApi _baatAuthzApi;

        public GeonorgeAuthorizationService(IBaatAuthzApi baatAuthzApi)
        {
            _baatAuthzApi = baatAuthzApi;
        }

        /// <summary>
        ///     Returning claims for the given user from BaatAuthzApi.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<List<Claim>> GetClaims(ClaimsIdentity identity)
        {
            Claim usernameClaim =
                identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            BaatAuthzUserInfoResponse response = await _baatAuthzApi.Info(usernameClaim.Value);

            var claims = new List<Claim>();

            if (response == BaatAuthzUserInfoResponse.Empty)
                Log.Warning("Empty response from BaatAuthzApi - no claims appended to user");
            else
                claims.AddRange(new List<Claim>
                {
                    new Claim("Name", string.IsNullOrEmpty(response?.Name) ? "" : response.Name),
                    new Claim("Email", response.Email),
                    new Claim("AuthorizedFrom", response.AuthorizedFrom),
                    new Claim("AuthorizedUntil", response.AuthorizedUntil),
                    new Claim("OrganizationName", response.Organization?.Name),
                    new Claim("OrganizationOrgnr", response.Organization?.Orgnr),
                    new Claim("OrganizationContactName", response.Organization?.ContactName),
                    new Claim("OrganizationContactEmail", response.Organization?.ContactEmail),
                    new Claim("OrganizationContactPhone", response.Organization?.ContactPhone)
                });

            await AppendRoles(usernameClaim.Value, claims);

            AppendFakeRolesForDemoUser(usernameClaim.Value, claims); 

            return claims;
        }

        private async Task AppendRoles(string username, List<Claim> claims)
        {
            BaatAuthzUserRolesResponse response = await _baatAuthzApi.GetRoles(username);

            if(response.Services != null) { 
            response.Services
                .Where(role => role.StartsWith(GeonorgeRoleNamePrefix))
                .ToList()
                .ForEach(role => claims.Add(new Claim(ClaimIdentifierRole, role)));
            }
        }

        private void AppendFakeRolesForDemoUser(string username, List<Claim> claims)
        {
            if (username == "esk_demobruker")
            {
                claims.Add(new Claim(ClaimIdentifierRole, GeonorgeRoles.MetadataAdmin));
                claims.Add(new Claim(ClaimIdentifierRole, GeonorgeRoles.MetadataEditor));
                claims.Add(new Claim("Name", "Dag Olav Dahle"));
                claims.Add(new Claim("Email", "dagolav@arkitektum.no"));
                claims.Add(new Claim("AuthorizedFrom", ""));
                claims.Add(new Claim("AuthorizedUntil", ""));
                claims.Add(new Claim("OrganizationName", "Kartverket"));
                claims.Add(new Claim("OrganizationOrgnr", "971040238"));
                claims.Add(new Claim("OrganizationContactName", ""));
                claims.Add(new Claim("OrganizationContactEmail", ""));
                claims.Add(new Claim("OrganizationContactPhone", ""));
            }
        }
    }
}