namespace MinimalApiPeliculas.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        public int recordsPorPagina = 1;
        public readonly int cantidadMaximaRecordPorPagina = 50;
        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value>cantidadMaximaRecordPorPagina)?cantidadMaximaRecordPorPagina:value; }
        }
    }
}
