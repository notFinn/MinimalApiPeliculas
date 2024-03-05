using AutoMapper;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearGeneroDTO, GeneroEntidad>();
            CreateMap<GeneroEntidad, GeneroDto>();

            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x=>x.Foto,opciones=>opciones.Ignore());
            CreateMap<Actor, ActorDTO>();
        }

    }
}
