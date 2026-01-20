using FSH.Starter.Blazor.Infrastructure.NoonaApi;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace FSH.Starter.Blazor.Infrastructure.Auth.Jwt;
public class NoonaJwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly string token;

    public NoonaJwtAuthenticationHeaderHandler(IConfiguration config)
    {
        token = config["NoonaApiToken"] ?? string.Empty;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
