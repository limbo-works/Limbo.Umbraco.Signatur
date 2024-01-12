using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.String, Group = "Limbo", Icon = EditorIcon)]
public class SignaturTimestampEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.Timestamp";

    internal const string EditorName = "Limbo Signatur Timestamp";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/Timestamp.html";

    internal const string EditorIcon = "icon-limbo-time color-limbo";

    #endregion

    #region Constructors

    public SignaturTimestampEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}