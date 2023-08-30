using NetKubernetes.Dtos.UsuarioDtos;

namespace NetKubernetes.Data.Usuarios;

public interface IUsuarioRepository {

    Task<UsuarioResponseDto> GetUsuario(); //no es necesario mandar un parametro en el payload del request ya que lo que envia el header, es el token de seguridad. leera el username del token

    //metodo login
    Task<UsuarioResponseDto> Login(UsuarioLoginRequestDto request);

    Task<UsuarioResponseDto> RegistroUsuario(UsuarioRegistroRequestDto request);

}