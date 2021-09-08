using System;
using Distancify.SerilogExtensions;
using System.Threading.Tasks;
using Hangfire;
using Distancify.LitiumAddOns.MediaMapper;
using Distancify.LitiumAddOns.MediaMapper.Services;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Products;
using Litium.Foundation;

namespace Distancify.LitiumAddOns.MediaMapper.Jobs
{
    public class MediaMappingJob
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 10)]
        public Task Execute(Guid folderSystemId, string mediaProfilerName)
        {
            var logger = this.Log()
                .ForContext("FolderSystemID", folderSystemId)
                .ForContext("MediaProfilerName", mediaProfilerName);
            logger.Information("Running media mapping job ..");

            try
            {
                using (Solution.Instance.SystemToken.Use())
                {
                    Map(folderSystemId, mediaProfilerName);
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

        private void Map(Guid folderSystemId, string mediaProfilerName)
        {
            var uploadFolder = GetUploadFolder(folderSystemId);
            var mediaProfiler = GetInstance(mediaProfilerName);
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

        private IMediaProfiler GetInstance(string typeName)
        {
            Type type = Type.GetType(typeName);
            return (IMediaProfiler)Activator.CreateInstance(type);
        }
    }
}