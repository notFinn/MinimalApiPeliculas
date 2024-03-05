
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace MinimalApiPeliculas.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string? connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage")!;
        }
        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            var ext=Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var blob = cliente.GetBlobClient(nombreArchivo);
            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString();
        }

        public async Task Borrar(string? ruta, string contenedor)
        {
            if (ruta.IsNullOrEmpty())
            {
                var cliente = new BlobContainerClient(connectionString, contenedor);
                await cliente.CreateIfNotExistsAsync();
                var nombreArchivo = Path.GetFileName(ruta);
                var blob = cliente.GetBlobClient(nombreArchivo);
                await blob.DeleteIfExistsAsync();
                
            }
        }
    }
}
