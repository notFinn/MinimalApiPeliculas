using Dapper;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.Entidades;
using System.Data;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly string? connectionString;

        public RepositorioGeneros(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int> CrearGenero(GeneroEntidad genero)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var id = await conexion.QuerySingleAsync<int>(@"insert INTO dbo.Generos(nombre) values (@Nombre);
                                                                SELECT SCOPE_IDENTITY();
                                                                ", genero);
                genero.Id = id;
                return id;
            }
        }
        public async Task<List<GeneroEntidad>> ObtenerTodos()
        {

            using (var conexion = new SqlConnection(connectionString))
            {
                var generos = await conexion.QueryAsync<GeneroEntidad>(@"Generos_ObtenerTodos",commandType:CommandType.StoredProcedure);

                return generos.AsList();
            }
        }
        public async Task<GeneroEntidad?> ObtenerPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var genero = await conexion.QueryFirstOrDefaultAsync<GeneroEntidad>($"SELECT Id,Nombre FROM Generos WHERE ID =@Id", new { id });
                return genero;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>(@"
                                                IF Exists(select 1 from Generos where Id=@Id)
	                                                select 1
                                                else 
	                                                select 0", new { id });
                return existe;
            }
        }

        public async Task Actualizar(GeneroEntidad genero)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(@"
	                update Generos
	                set Nombre=@Nombre
	                where Id=@Id
                ", genero);
            }
        }

        public async Task Borrar(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(@"DELETE Generos
                                              where Id=@Id", new { id });
            }
        }
    }
}
