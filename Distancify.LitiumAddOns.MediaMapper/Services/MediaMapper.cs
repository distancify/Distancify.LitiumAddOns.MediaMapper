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
using Distancify.SerilogExtensions;

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

        private readonly string _uploadFolder;

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
            _uploadFolder = mediaUploadFolder?.Replace('\\', '/').Trim('/');
        }

        public void Map()
        {
            var mediaList = GetProfiledMediaFromUploadFolder().ToList();
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
            if (media.MediaEntityMappings == null) return true;

            foreach (var mapping in media.MediaEntityMappings)
            {
                var productFieldUpdate = Policy.Handle<DataException>()
                    .WaitAndRetry(MaximumProductUpdateRetries, (retryCount) => TimeSpan.FromSeconds(retryCount))
                    .ExecuteAndCapture(() => {

                        if (mapping.EntityType == EntityTypeEnum.Variant)
                        {
                            var variant = _variantService.Get(mapping.EntityId)?.MakeWritableClone();
                            if (variant != null)
                            {
                                SetField(variant.Fields, mapping.FieldId, media.File);
                                _variantService.Update(variant);
                            }
                        }
                        else if (mapping.EntityType == EntityTypeEnum.BaseProduct)
                        {
                            var product = _baseProductService.Get(mapping.EntityId)?.MakeWritableClone();
                            if (product != null)
                            {
                                SetField(product.Fields, mapping.FieldId, media.File);
                                _baseProductService.Update(product);
                            }
                        }
                    });

                if (productFieldUpdate.Outcome == OutcomeType.Failure)
                {
                    this.Log().ForContext("FileID", media.File.SystemId)
                        .ForContext("FieldID", mapping.FieldId)
                        .ForContext("Filename", media.File.Name)
                        .ForContext("EntityType", mapping.EntityType.ToString())
                        .ForContext("EntityId", mapping.EntityId)
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
            var f = string.Format("{0}/{1}", _uploadFolder, media.ArchivePath);
            _mediaArchive.EnsureFolderExists(f);
            var targetFolder = _mediaArchive.GetFolder(f);
            _mediaArchive.MoveFile(media.File.SystemId, targetFolder);
        }

        private IEnumerable<MediaProfile> GetProfiledMediaFromUploadFolder()
        {
            return _mediaArchive.GetFiles(GetUploadFolder(), false)
                .OrderBy(r => r.LastWriteTimeUtc)
                .Select(r => _mediaProfiler.GetMediaProfile(new MediaProfileBuilder(r)))
                .Where(r => r != null);
        }

        public Folder GetUploadFolder()
        {
            _mediaArchive.EnsureFolderExists(_uploadFolder);
            return _mediaArchive.GetFolder(_uploadFolder);
        }

        private void AttachMetadata(IEnumerable<MediaProfile> mediaList)
        {
            foreach (MediaProfile media in mediaList.Where(r => r.Fields != null))
            {
                var file = media.File.MakeWritableClone();

                foreach (var item in media.Fields)
                {
                    file.Fields.AddOrUpdateValue(item.Key, item.Value);
                }

                _mediaArchive.SaveChanges(file);
            }
        }
    }
}