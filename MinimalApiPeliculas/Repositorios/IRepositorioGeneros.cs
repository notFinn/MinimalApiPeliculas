using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task Actualizar(GeneroEntidad genero);
        Task Borrar(int id);
        Task<int> CrearGenero(GeneroEntidad genero);
        Task<bool> Existe(int id);
        Task<GeneroEntidad?> ObtenerPorId(int id);
        Task<List<GeneroEntidad>> ObtenerTodos();
    }
}