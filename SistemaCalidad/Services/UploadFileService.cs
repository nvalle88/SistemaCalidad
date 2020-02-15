using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SistemaCalidad.Data;
using SistemaCalidad.Models.Utiles;
using SistemaCalidad.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SistemaCalidad.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CALIDADContext db;

        public UploadFileService(IHostingEnvironment environment, CALIDADContext db)
        {
            _hostingEnvironment = environment;
            this.db = db;
        }

        #region Métodos internos
        public async Task<bool> UploadFile(byte[] file, string folder, string fileName)
        {
            try
            {
                var stream = new MemoryStream(file);
                var targetDirectory = Path.Combine(_hostingEnvironment.WebRootPath, folder);
                var targetFile = Path.Combine(targetDirectory, fileName);

                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                using (var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                    await stream.CopyToAsync(fileStream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFile(string url)
        {
            try
            {
                var targetDirectory = Path.Combine(_hostingEnvironment.WebRootPath, url);
                if (File.Exists(targetDirectory))
                {
                    File.Delete(targetDirectory);
                    return true;
                }
            }
            catch (Exception)
            { }
            return false;
        }

        public string FileExtension(string fileName)
        {
            try
            {
                string[] arr = fileName.Split('.');
                return $".{arr[arr.Length - 1]}";
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        private byte[] GetBytesFromFile(string folder, string fileName)
        {
            try
            {
                var targetDirectory = Path.Combine(_hostingEnvironment.WebRootPath, $"{folder}\\{fileName}");
                var file = new FileStream(targetDirectory, FileMode.Open, FileAccess.Read);

                byte[] data;
                using (var br = new BinaryReader(file))
                    data = br.ReadBytes((int)file.Length);

                return data;
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        public DocumentoTransfer GetFileDocumentoRequisito(string folder, int idDocumentoRequisito, string fileName)
        {
            try
            {
                string extensionFile = FileExtension(fileName);
                byte[] data = GetBytesFromFile(folder, $"{idDocumentoRequisito}{extensionFile}");
                if (data.Length > 0)
                {
                    return new DocumentoTransfer
                    {
                        Nombre = fileName,
                        Fichero = data
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Métodos externos (se utilizan para llamarlos desde los controladores)
        public async Task<bool> UploadFiles(DocumentoTransfer documentoTransfer)
        {
            try
            {


                string extensionFile = FileExtension(documentoTransfer.Nombre);
                await UploadFile(documentoTransfer.Fichero, Mensaje.CarpetaDocumento, $"{documentoTransfer.IdRequisito}{extensionFile}");

                var seleccionado = await db.Certificado.FindAsync(documentoTransfer.IdRequisito);
                seleccionado.FileUrl = $"{Mensaje.CarpetaDocumento}/{documentoTransfer.IdRequisito}{extensionFile}";
                db.Certificado.Update(seleccionado);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                Debug.Write(ex.Message);

            }
            return false;
        }

        public async Task<DocumentoTransfer> GetFileDocumentoRequisito(int idDocumento)
        {
            try
            {
                var documentoRequisito = await db.Certificado.FirstOrDefaultAsync(c => c.CertificadoId == idDocumento);
                return GetFileDocumentoRequisito(Mensaje.CarpetaDocumento, idDocumento, documentoRequisito.FileUrl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteDocumentoRequisito(int idDocumentoRequisito)
        {
            try
            {
                //var respuesta = await db.DocumentoRequisito.SingleOrDefaultAsync(m => m.IdDocumentoRequisito == idDocumentoRequisito);
                //if (respuesta == null)
                //    return false;

                //var respuestaFile = DeleteFile(respuesta.Url);
                //db.DocumentoRequisito.Remove(respuesta);
                //await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}