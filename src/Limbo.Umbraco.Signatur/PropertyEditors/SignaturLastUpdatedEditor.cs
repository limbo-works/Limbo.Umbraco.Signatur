using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.String)]
public class SignaturLastUpdatedEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.LastUpdated";

    internal const string EditorName = "Limbo Signatur Last Updated";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/Timestamp.html";

    internal const string EditorIcon = "icon-limbo-signatur";

    #endregion

    #region Constructors

    public SignaturLastUpdatedEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}