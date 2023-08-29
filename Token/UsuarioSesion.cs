using System.Security.Claims;
using Azure.Identity;

namespace NetKubernetes.Token;

public class UsuarioSesion : IUsuarioSesion
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UsuarioSesion(IHttpContextAccessor httpContextAccesor)
    {
        _httpContextAccessor = httpContextAccesor;
    }
    public string ObtenerUsuarioSesion()
    {
        var userName = _httpContextAccessor.HttpContext!.User?.Claims?
                            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        return userName!;
    }
}