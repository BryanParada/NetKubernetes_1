using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; 
using NetKubernetes.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NetKubernetes.Token;

public class JwtGenerador : IJwtGenerador
{
    public string CrearToken(Usuario usuario)
    {
        var claims = new List<Claim> { 
            new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName!),
            new Claim("userId", usuario.Id),
            new Claim("email", usuario.Email!)
        };

        //creacion llave seguridad
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Esta es Mi palabra secreta para autenticacion ha sido extendida para alcanzar mas de 512 bits"));
        //crear credenciales que tendra el token
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //instanciar el cuerpo principal del token
        var tokenDescripcion = new SecurityTokenDescriptor{
            //Propiedades del payload:
            Subject = new ClaimsIdentity(claims),
            //dias de acceso
            Expires = DateTime.Now.AddDays(30),
            SigningCredentials = credenciales
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescripcion);
        //regresamos cadena de caracteres decodificada base64
        return tokenHandler.WriteToken(token);
        

    }
}