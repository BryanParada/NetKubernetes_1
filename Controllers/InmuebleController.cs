using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NetKubernetes.Data.Inmuebles;
using NetKubernetes.Dtos.InmuebleDtos;
using NetKubernetes.Middleware;
using NetKubernetes.Models;

namespace NetKubernetes.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InmuebleController : ControllerBase {

    private readonly IInmuebleRepository _repository;
    private IMapper _mapper;

    public InmuebleController(
        IInmuebleRepository repository,
        IMapper mapper
    )
    {
        _mapper = mapper;
        _repository = repository;
    }

    //metodo para obtener toda la lista de inmuebles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InmuebleResponseDto>>> GetInmuebles()
    {
        var inmuebles = await _repository.GetAllInmuebles();
        return Ok(_mapper.Map<IEnumerable<InmuebleResponseDto>>(inmuebles)); //convirtiendo la data de la claseInmuebles a una coleccion de tipo inmueblreResponseDto
    }

    [HttpGet("{id}", Name = "GetInmuebleById")]
    public async Task<ActionResult<InmuebleResponseDto>> GetInmuebleById(int id)
    {
        var inmueble = await _repository.GetInmuebleById(id);

        if(inmueble is null)
        {
            throw new MiddlewareException(
                HttpStatusCode.NotFound, 
                new {mensaje = $"No se encontró el inmueble por este id { id }"}
            );
        }

        return Ok(_mapper.Map<InmuebleResponseDto>(inmueble));

    }

    [HttpPost]
    public async Task<ActionResult<InmuebleResponseDto>> CreateInmueble( [FromBody] InmuebleRequestDto inmueble)
    {
        var inmuebleModel =  _mapper.Map<Inmueble>(inmueble);
        await _repository.CreateInmueble(inmuebleModel);
        await _repository.SaveChanges();

        //devuelve despues de grabar
        var inmuebleResponse = _mapper.Map<InmuebleResponseDto>(inmuebleModel); 

        return CreatedAtRoute(nameof(GetInmuebleById), new {inmuebleResponse.Id}, inmuebleResponse); //para que pueda ejecutar el metodo de arriba

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInmueble(int id)
    {
        await _repository.DeleteInmueble(id);
        await _repository.SaveChanges();
        return Ok();

    }



}