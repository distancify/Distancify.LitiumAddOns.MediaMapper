using System;
using Distancify.SerilogExtensions;
using System.Threading.Tasks;
using Hangfire;
using Distancify.LitiumAddOns.MediaMapper.Services;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Products;
using Litium.Foundation;
using Litium;

namespace Distancify.LitiumAddOns.MediaMapper.Jobs
{
    public class MediaMappingJob
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 10)]
        public Task Execute(Guid folderSystemId)
        {
            var logger = this.Serilog()
                .ForContext("FolderSystemID", folderSystemId);
            logger.Information("Running media mapping job ..");

            try
            {
                using (Solution.Instance.SystemToken.Use())
                {
                    foreach (var p in IoC.ResolveAll<IMediaProfiler>())
                    {
                        Map(folderSystemId, p);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An exception occurred in the media mapping job.");
                throw;
            }

            logger.Information("Finished Running media mapping job.");

            return Task.CompletedTask;
        }

        private void Map(Guid folderSystemId, IMediaProfiler mediaProfiler)
        {
            var uploadFolder = GetUploadFolder(folderSystemId);
            var mediaMapper = GetMediaMapper(mediaProfiler, uploadFolder);
            mediaMapper.Map(includeSubFolders: true);
        }

        private string GetUploadFolder(Guid folderSystemId)
        {
            var folderService = Litium.IoC.Resolve<FolderService>();
            var uploadFolder = string.Empty;

            while (!folderSystemId.Equals(Guid.Empty))
            {
                var folder = folderService.Get(folderSystemId);
                uploadFolder = $"/{folder.Name}{uploadFolder}";
                folderSystemId = folder.ParentFolderSystemId;
            }

            return uploadFolder;
        }

        private Services.MediaMapper GetMediaMapper(IMediaProfiler mediaProfiler, string uploadFolder)
        {
            return new Services.MediaMapper(
                Litium.IoC.Resolve<BaseProductService>(),
                Litium.IoC.Resolve<VariantService>(),
                Litium.IoC.Resolve<FieldDefinitionService>(),
                Litium.IoC.Resolve<MediaArchive>(),
                Litium.IoC.ResolveAll<IFieldSetter>(),
                mediaProfiler,
                uploadFolder);
        }
    }
}