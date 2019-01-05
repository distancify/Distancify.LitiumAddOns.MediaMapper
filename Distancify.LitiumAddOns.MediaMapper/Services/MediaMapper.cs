using System;
using System.Collections.Generic;
using System.Linq;
using Distancify.LitiumAddOns.MediaMapper.Services;
using Litium.Data;
using Litium.FieldFramework;
using Litium.Media;
using Litium.Products;
using Serilog;
using Polly;

namespace Distancify.LitiumAddOns.MediaMapper.Services
{
    public class MediaMapper : IMediaMapper
    {
        private readonly BaseProductService _baseProductService;
        private readonly VariantService _variantService;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly MediaArchive _mediaArchive;
        private readonly IMediaProfiler _mediaProfiler;        
        private readonly IList<IFieldSetter> _fieldSetters;

        private readonly string _targetFolder;

        private const int MaximumProductUpdateRetries = 2;

        public MediaMapper(BaseProductService baseProductService,
            VariantService variantService,
            FieldDefinitionService fieldDefinitionService,
            MediaArchive mediaArchive,       
            IEnumerable<IFieldSetter> fieldSetters,
            IMediaProfiler mediaProfiler,
            string mediaUploadFolder)
        {
            _baseProductService = baseProductService;
            _variantService = variantService;
            _fieldDefinitionService = fieldDefinitionService;
            _mediaArchive = mediaArchive;
            _mediaProfiler = mediaProfiler;            
            _fieldSetters = fieldSetters.ToList();
            _targetFolder = mediaUploadFolder?.Replace('\\', '/').Trim('/');
        }

        public void Map()
        {
            var mediaList = GetProfiledMediaFromTargetFolder().ToList();
            AttachMetadata(mediaList);

            foreach (var media in mediaList)
            {
                if (UpdateProducts(media))
                {
                    MoveMediaFile(media);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="media"></param>
        /// <returns>true if all products was updates successfully, otherwise false</returns>
        private bool UpdateProducts(MediaProfile media)
        {
            foreach ((string productId, bool isVariant) in media.Products)
            {
                var productFieldUpdate = Policy.Handle<DataException>()
                    .WaitAndRetry(MaximumProductUpdateRetries, (retryCount) => TimeSpan.FromSeconds(retryCount))
                    .ExecuteAndCapture(() => {

                        if (isVariant)
                        {
                            var variant = _variantService.Get(productId)?.MakeWritableClone();
                            if (variant != null)
                            {
                                SetField(variant.Fields, media.FieldId, media.File);
                                _variantService.Update(variant);
                            }
                        }
                        else
                        {
                            var product = _baseProductService.Get(productId)?.MakeWritableClone();
                            if (product != null)
                            {
                                SetField(product.Fields, media.FieldId, media.File);
                                _baseProductService.Update(product);
                            }
                        }
                    });

                if (productFieldUpdate.Outcome == OutcomeType.Failure)
                {
                    this.Log().ForContext("FileID", media.File.SystemId)
                        .ForContext("FieldID", media.FieldId)
                        .ForContext("Filename", media.File.Name)
                        .ForContext("ArticleNumber", productId)
                        .ForContext("IsVariant", isVariant)
                        .Error(productFieldUpdate.FinalException, "Could not map media file");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates a field value in 'fields' specified by 'fieldId' and 'file'
        /// </summary>
        /// <param name="fields">The list of fields to update</param>
        /// <param name="fieldId">The ID of the field to update</param>
        /// <param name="file">The file to set to the field value</param>
        private void SetField(FieldContainer fields, string fieldId, File file)
        {
            var def = _fieldDefinitionService.Get<ProductArea>(fieldId);

            var logger = this.Log().ForContext("FileID", file.SystemId)
                    .ForContext("FieldID", fieldId)
                    .ForContext("Filename", file.Name);

            if (def == null)
            {
                logger.Warning("Could not map media: Unable to find field with ID {FieldId}", fieldId);
                return;
            }

            var setter = _fieldSetters.FirstOrDefault(r => r.CanSet(def));
            if (setter == null)
            {
                logger.Warning("Could not map media: Unable to find field setter for type {FieldType}", def.FieldType);
                return;
            }

            setter.Set(fields, def, file);
        }

        private void MoveMediaFile(MediaProfile media)
        {
            var f = string.Format("{0}/{1}", _targetFolder, media.ArchivePath);
            _mediaArchive.EnsureFolderExists(f);
            var targetFolder = _mediaArchive.GetFolder(f);
            _mediaArchive.MoveFile(media.File.SystemId, targetFolder);
        }

        private IEnumerable<MediaProfile> GetProfiledMediaFromTargetFolder()
        {
            _mediaArchive.EnsureFolderExists(_targetFolder);
            var targetFolder = _mediaArchive.GetFolder(_targetFolder);
            return _mediaArchive.GetFiles(targetFolder, false)
                .OrderBy(r => r.LastWriteTimeUtc)
                .Select(r => _mediaProfiler.GetMediaProfile(r))
                .Where(r => r != null);
        }

        private void AttachMetadata(IEnumerable<MediaProfile> mediaList)
        {
            foreach (MediaProfile media in mediaList)
            {
                var file = media.File.MakeWritableClone();

                foreach (var item in media.Metadata)
                {
                    file.Fields.AddOrUpdateValue(item.Key, item.Value);
                }

                _mediaArchive.SaveChanges(file);
            }
        }
    }
}