using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Geonorge.MinSide.Utils
{
    public class OpenIdConnectConfigFromFile : IConfigurationManager<OpenIdConnectConfiguration>
    {
        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            var json = await File.ReadAllTextAsync("openid-configuration.json", cancel);
            return new OpenIdConnectConfiguration(json);
        }

        public void RequestRefresh()
        {
        }
    }
}