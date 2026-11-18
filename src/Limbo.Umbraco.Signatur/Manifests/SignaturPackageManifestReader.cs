using System.Collections.Generic;
using System.Threading.Tasks;
using Limbo.Umbraco.Signatur.PropertyEditors;
using Skybrud.Essentials.Umbraco.Manifests.Extensions;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.Icons;
using Skybrud.Essentials.Umbraco.Manifests.Extensions.PropertyEditors;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

using static Limbo.Umbraco.Signatur.SignaturPackage;

namespace Limbo.Umbraco.Signatur.Manifests;

public class SignaturPackageManifestReader : IPackageManifestReader {

    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        IEnumerable<PackageManifest> manifests = [
            new() {
                Id = Alias,
                Name = Name,
                AllowTelemetry = true,
                Version = InformationalVersion,
                Extensions = [..GetExtensions()]
            }
        ];

        return Task.FromResult(manifests);

    }

    private static IEnumerable<IExtension> GetExtensions() {

        yield return new PropertyEditorUiExtension {
            Alias = SignaturJobDataPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Job Data Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/PropertyEditors/JobData.js",
            ElementName = "limbo-umbraco-signatur-job-data-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = SignaturJobDataPropertyEditor.EditorName,
                Icon = SignaturJobDataPropertyEditor.EditorIcon,
                Group = SignaturJobDataPropertyEditor.EditorGroup,
                PropertyEditorSchemaAlias = SignaturJobDataPropertyEditor.EditorAlias
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = SignaturJobIdPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Job ID Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/PropertyEditors/JobId.js",
            ElementName = "limbo-umbraco-signatur-job-id-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = SignaturJobIdPropertyEditor.EditorName,
                Icon = SignaturJobIdPropertyEditor.EditorIcon,
                Group = SignaturJobIdPropertyEditor.EditorGroup,
                PropertyEditorSchemaAlias = SignaturJobIdPropertyEditor.EditorAlias
            }
        };

        yield return new PropertyEditorUiExtension {
            Alias = SignaturLastUpdatedPropertyEditor.EditorUiAlias,
            Name = $"{Name}: Last Updated Property Editor UI",
            Element = $"/App_Plugins/{Alias}/Elements/PropertyEditors/LastUpdated.js",
            ElementName = "limbo-umbraco-signatur-last-updated-property-editor",
            Meta = new PropertyEditorUiMeta {
                Label = SignaturLastUpdatedPropertyEditor.EditorName,
                Icon = SignaturLastUpdatedPropertyEditor.EditorIcon,
                Group = SignaturLastUpdatedPropertyEditor.EditorGroup,
                PropertyEditorSchemaAlias = SignaturLastUpdatedPropertyEditor.EditorAlias
            }
        };

        yield return new IconsExtension {
            Alias = $"{Alias}.Icons",
            Name = $"{Name}: Icons",
            Js = $"/App_Plugins/{Alias}/Icons.js"
        };

    }

}