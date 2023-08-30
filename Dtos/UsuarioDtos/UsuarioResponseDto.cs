namespace NetKubernetes.Dtos.UsuarioDtos;

public class UsuarioResponseDto
{
    //Esta es la data que le quiero enviar al cliente despues de registrar un nuevo usuario, o al hacer un login
    public string? Id { get; set; }
    public string? Nombre { get; set; }

    public string? Token { get; set; }

    public string? Apellido { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Telefono { get; set; }

}