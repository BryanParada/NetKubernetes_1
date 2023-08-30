using System.Net;

namespace NetKubernetes.Middleware;

//Exception ya es una clase que existe en .net
public class MiddlewareException : Exception 
{
    public HttpStatusCode Codigo { get; set; }

    public object? Errores { get; set; }

    public MiddlewareException(HttpStatusCode codigo, object? errores = null)
    {
        Codigo = codigo;
        Errores = errores;
    }
}