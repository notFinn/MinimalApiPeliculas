using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.EndPoints
{
    public static class GenerosEndPoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            //se considera politica abierto
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("/", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/{id:int}", EliminarGenero);


            
            return group;
        }
        static async Task<Ok<List<GeneroDto>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();

            var generosDto = mapper.Map<List<GeneroDto>>(generos);
            return TypedResults.Ok(generosDto);
        }

        static async Task<Results<Ok<GeneroDto>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDto=mapper.Map<GeneroDto>(genero);
            return TypedResults.Ok(generoDto);
        }
        static async Task<Created<GeneroDto>> CrearGenero(CrearGeneroDTO genero,
            IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore,IMapper mapper)
        {
            var generoEntity = mapper.Map<GeneroEntidad>(genero);

            var id = await repositorioGeneros.CrearGenero(generoEntity);
            //limpio el cache
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDto =mapper.Map<GeneroDto>(generoEntity);
            return TypedResults.Created($"/generos/{id}", generoDto);
        }
        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDTO genero, IRepositorioGeneros repositorio,
            IOutputCacheStore outPutcacheStore, IMapper mapper)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            var generoEntity = mapper.Map<GeneroEntidad>(genero);
            generoEntity.Id = id;
            await repositorio.Actualizar(generoEntity);
            await outPutcacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent, NotFound>> EliminarGenero(int id, IRepositorioGeneros repositorio,
            IOutputCacheStore outPutcacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            await repositorio.Borrar(id);
            await outPutcacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
