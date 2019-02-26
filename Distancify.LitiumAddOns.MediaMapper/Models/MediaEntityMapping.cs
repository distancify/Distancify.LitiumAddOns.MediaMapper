namespace Distancify.LitiumAddOns.MediaMapper.Models
{
    public struct MediaEntityMapping
    {
        public MediaEntityMapping(EntityTypeEnum type, string entityId, string fieldId)
        {
            EntityType = type;
            EntityId = entityId;
            FieldId = fieldId;
        }

        public EntityTypeEnum EntityType { get; private set; }
        public string EntityId { get; private set; }
        public string FieldId { get; private set; }

    }
}
