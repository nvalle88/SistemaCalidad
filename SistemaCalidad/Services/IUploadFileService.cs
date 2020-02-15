using SistemaCalidad.Models.Utiles;
using System.Threading.Tasks;

namespace SistemaCalidad.Services
{
    public interface IUploadFileService
    {
        Task<bool> UploadFile(byte[] file, string folder, string fileName);
        Task<bool> UploadFiles(DocumentoTransfer documentoRequisitoTransfer);
        bool DeleteFile(string url);
        string FileExtension(string fileName);
        DocumentoTransfer GetFileDocumentoRequisito(string folder, int idDocumentoRequisito, string fileName);
        Task<DocumentoTransfer> GetFileDocumentoRequisito(int idDocumentoRequisito);
        Task<bool> DeleteDocumentoRequisito(int idDocumentoRequisito);
    }
}
