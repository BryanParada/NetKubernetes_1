using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetKubernetes.Data;
using NetKubernetes.Data.Inmuebles;
using NetKubernetes.Data.Usuarios;
using NetKubernetes.Middleware;
using NetKubernetes.Models;
using NetKubernetes.Profiles;
using NetKubernetes.Token;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => {
    opt.LogTo(Console.WriteLine, new [] {
        DbLoggerCategory.Database.Command.Name},
        LogLevel.Information).EnableSensitiveDataLogging();

    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")!);
});

//insertar repositorio de inmueble
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();


// Add services to the container.

builder.Services.AddControllers( opt => {
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));

});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile( new InmuebleProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var builderSecurity = builder.Services.AddIdentityCore<Usuario>();
var identityBuilder = new IdentityBuilder(builderSecurity.UserType, builder.Services);
identityBuilder.AddEntityFrameworkStores<AppDbContext>();
identityBuilder.AddSignInManager<SignInManager<Usuario>>();
//registar la hora cuando se crean los usuarios
builder.Services.AddSingleton<ISystemClock, SystemClock>();
//generador de tokens
builder.Services.AddScoped<IJwtGenerador, JwtGenerador>();
//inyectar al usuario sesion
builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();
//inyectar al repositorio de usuario
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

//setear el esquema de seguridad para que el usuario ingrese con el token - configuracion de seguridad de tokens
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Esta es Mi palabra secreta para autenticacion ha sido extendida para alcanzar mas de 512 bits")); // llave en JwtGenerador.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key, //key
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };

                });

//configuracion de CORS, para aceptar POST, PUT DELETE, ETC
builder.Services.AddCors( o => o.AddPolicy("corsapp", builder =>{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); //acepta cualquier cliente a nuestros metodos controllers - "AllowAnyMethod" (post,put,etc)
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ManagerMiddleware>();

app.UseAuthentication();
app.UseCors("corsapp");

app.UseAuthorization();

app.MapControllers();

using (var ambiente = app.Services.CreateScope())
{
    var services = ambiente.ServiceProvider;

    try{
       var userManager = services.GetRequiredService<UserManager<Usuario>>();
       var context = services.GetRequiredService<AppDbContext>();
       await context.Database.MigrateAsync(); 
       await LoadDatabase.InsertarData(context, userManager);
    }
    catch (Exception e)
    {
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(e, "Ocurrió un error en la migración");
    }

}




app.Run();
