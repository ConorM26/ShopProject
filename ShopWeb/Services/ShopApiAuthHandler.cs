
using System.Net.Http.Headers;

namespace ShopWeb.Services;

public class ShopApiAuthHandler(
    IHttpContextAccessor httpContextAccessor,
    IShopApiTokenService tokenService) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer",
                    tokenService.CreateToken(user));
        }

        return base.SendAsync(request, cancellationToken);
    }
}
