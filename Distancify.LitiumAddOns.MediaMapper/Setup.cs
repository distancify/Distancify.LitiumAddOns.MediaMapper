using Litium.FieldFramework;
using Litium.Foundation;
using Litium.Globalization;
using Litium.Media;
using Litium.Runtime;
using Serilog;
using System.Linq;

namespace Distancify.LitiumAddOns.MediaMapper
{
    [Autostart]
    public class Setup
    {
        private const string MediaMapper = "MediaMapper";

        public Setup(
            LanguageService languageService,
            FieldTemplateService templateService,
            FieldDefinitionService definitionService)
        {
            using (Solution.Instance.SystemToken.Use())
            {
                EnsureField<MediaArea>(MediaMapper, "MediaMappingButton");

                EnsureMediaFolderFieldGroup();
            }

            void EnsureField<T>(string fieldId, string fieldType, bool editable = true)
                where T : IArea
            {
                var field = definitionService.Get<T>(fieldId);
                if (field == null)
                {
                    Log.Information("MediaMapper: Creating missing {Area} field definition {FieldId}", typeof(T).Name, fieldId);
                    field = new FieldDefinition(fieldId, fieldType, typeof(T))
                    {
                        Editable = editable,
                    };
                    LocalizeField(field, fieldId);
                    definitionService.Create(field);
                }
                else
                {
                    Log.Information("MediaMapper: Verifying {Area} field definition {FieldId}", typeof(T).Name, fieldId);
                    field = field.MakeWritableClone();
                    field.Editable = editable;
                    LocalizeField(field, fieldId);
                    definitionService.Update(field);
                }
            }

            void EnsureMediaFolderFieldGroup()
            {
                foreach (var template in templateService.GetAll().OfType<FolderFieldTemplate>())
                {
                    Log.Information("MediaMapper: Verifying {Type} template {TemplateId}", typeof(FolderFieldTemplate).Name, template.Id);

                    var writableTemplate = template.MakeWritableClone();

                    var group = writableTemplate.FieldGroups.FirstOrDefault(g => g.Fields.Contains(MediaMapper));
                    if (group != null)
                    {
                        continue;
                    }
                    group = writableTemplate.FieldGroups.FirstOrDefault();

                    if (group != null)
                    {
                        group.Fields.Add(MediaMapper);
                        templateService.Update(writableTemplate);
                    }
                }
            }

            void LocalizeField(FieldDefinition field, string fieldId)
            {
                foreach (var l in languageService.GetAll())
                {
                    field.Localizations[l.CultureInfo.Name].Name = fieldId;
                }
            }
        }
    }
}
