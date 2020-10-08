using Distancify.LitiumAddOns.MediaMapper.Models;
using Litium.FieldFramework;
using Litium.Media;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Distancify.LitiumAddOns.MediaMapper
{
    public class MediaProfileBuilder
    {
        private IList<MediaEntityMapping> _mappings = new List<MediaEntityMapping>();
        private string _archivePath;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();

        public File File { get; private set; }

        public MediaProfileBuilder(File file)
        {
            File = file;
        }

        public MediaProfileBuilder MapToBaseProductImages(string articleNumber)
        {
            _mappings.Add(new MediaEntityMapping(EntityTypeEnum.BaseProduct, articleNumber, SystemFieldDefinitionConstants.Images));
            return this;
        }

        public MediaProfileBuilder MapToVariantImages(string articleNumber)
        {
            _mappings.Add(new MediaEntityMapping(EntityTypeEnum.Variant, articleNumber, SystemFieldDefinitionConstants.Images));
            return this;
        }


        /// <summary>
        /// Instructs the media mapper to link the media item to the given reference field on the entity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityIds"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public MediaProfileBuilder MapTo(EntityTypeEnum type, string entityId, string fieldId)
        {
            _mappings.Add(new MediaEntityMapping(type, entityId, fieldId));
            return this;
        }

        /// <summary>
        /// Instructs the media mapper to link the media item to the given reference field on the entity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityId"></param>
        /// <param name="fieldIds"></param>
        /// <returns></returns>
        public MediaProfileBuilder MapTo(EntityTypeEnum type, string entityId, IEnumerable<string> fieldIds)
        {
            foreach (var fieldId in fieldIds)
            {
                _mappings.Add(new MediaEntityMapping(type, entityId, fieldId));
            }
            return this;
        }

        /// <summary>
        /// Instructs the media mapper to link the media item to the given reference field on the entity
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entityIds"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        public MediaProfileBuilder MapTo(EntityTypeEnum type, IEnumerable<string> entityIds, string fieldId)
        {
            foreach (var id in entityIds)
            {
                _mappings.Add(new MediaEntityMapping(type, id, fieldId));
            }
            return this;
        }

        /// <summary>
        /// Instructs the media mapper to set the given value to the specific field on the media item
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public MediaProfileBuilder SetField(string fieldId, object value)
        {
            _fields.Add(fieldId, value);
            return this;
        }

        public MediaProfileBuilder ArchiveTo(string relativePath)
        {
            _archivePath = relativePath;
            return this;
        }

        public MediaProfile Create()
        {
            var archivePath = _archivePath;
            if (string.IsNullOrWhiteSpace(archivePath))
            {
                archivePath = "Archive";
            }
            return new MediaProfile(File, _mappings.ToImmutableList(), _fields.ToImmutableDictionary(), archivePath);
        }

    }
}
