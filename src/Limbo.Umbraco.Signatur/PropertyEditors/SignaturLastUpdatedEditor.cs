using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.String, Group = "Limbo", Icon = EditorIcon)]
public class SignaturLastUpdatedEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.LastUpdated";

    internal const string EditorName = "Limbo Signatur Last Updated";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/Timestamp.html";

    internal const string EditorIcon = "icon-limbo-signatur color-limbo";

    #endregion

    #region Constructors

    public SignaturLastUpdatedEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}