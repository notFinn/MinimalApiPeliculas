using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using System.Data;
using System.Linq;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly string connectionString;
        private readonly HttpContext httpContext;

        public RepositorioActores(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = httpContextAccessor.HttpContext!;
        }
        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacion)
        {
            using var cnn = new SqlConnection(connectionString);
            var result = await cnn.QueryAsync<Actor>("Actores_ObtenerTodos",new { paginacion.Pagina,paginacion.RecordsPorPagina }, commandType: CommandType.StoredProcedure);
            var cantidadActores = await cnn.QuerySingleAsync<int>("Actores_Cantidad", commandType: CommandType.StoredProcedure);
            httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidadActores.ToString());

            return result.AsList();
        }
        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            using var cnn = new SqlConnection(connectionString);
            var result = await cnn.QueryAsync<Actor>("Actores_ObtenerPorNombre",new { nombre }, commandType: CommandType.StoredProcedure);
            return result.AsList();
        }
        public async Task<Actor?> ObtenerPorId(int id)
        {
            using var cnn = new SqlConnection(connectionString);
            var actor = await cnn.QueryFirstAsync<Actor>("Actores_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure);
            return actor;
        }
        public async Task<int> Crear(Actor actor)
        {
            using var cnn = new SqlConnection(connectionString);
            var id = await cnn.QuerySingleAsync<int>("Actores_Crear",
                new { actor.Nombre, actor.FechaNacimiento, actor.Foto }, commandType: CommandType.StoredProcedure);
            actor.Id = id;
            return id;
        }

        public async Task Actualizar(Actor actor)
        {
            using var cnn = new SqlConnection(connectionString);
            await cnn.ExecuteAsync("Actores_Actualizar",
                actor, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> Existe(int id)
        {
            using var cnn = new SqlConnection(connectionString);
            var existe = await cnn.QuerySingleAsync<bool>("Actores_ExistePorId", new { id }, commandType: CommandType.StoredProcedure);
            return existe;
        }

        public async Task Borrar(int id)
        {
            using var cnn = new SqlConnection(connectionString);
            await cnn.ExecuteAsync("Actores_Borrar",
               new { id }, commandType: CommandType.StoredProcedure);
        }
    }
}


