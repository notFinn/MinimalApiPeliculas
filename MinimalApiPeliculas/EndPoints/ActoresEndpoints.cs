﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPeliculas.EndPoints
{
    public static class ActoresEndpoints
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("obtenerPorNombre/{nombre}", ObtenerPorNombre);
            group.MapPost("/", Crear).DisableAntiforgery();
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio, IMapper mapper,
            int pagina=1,int recordsPorPagina =10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina,RecordsPorPagina=recordsPorPagina }; 
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDto = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDto);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(int id,IRepositorioActores repositorio,IMapper mapper)
        {
            var actor = await repositorio.ObtenerPorId(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }
            var actorDto=mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDto);
        }
        static async Task<Ok<List<ActorDTO>>> ObtenerPorNombre(string nombre,IRepositorioActores repositorio, IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            var actoresDto = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDto);
        }
        static async Task<Created<ActorDTO>> Crear([FromForm] CrearActorDTO crearActorDTO,
            IRepositorioActores repositorio, IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            var actor = mapper.Map<Actor>(crearActorDTO);
            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }
            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actores/{id}", actorDto);
        }
    }
}
