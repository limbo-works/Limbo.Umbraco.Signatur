using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Integer, Group = "Limbo", Icon = EditorIcon)]
public class SignaturJobIdEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.JobId";

    internal const string EditorName = "Limbo Signatur Job ID";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/JobId.html";

    internal const string EditorIcon = "icon-limbo-signatur color-limbo";

    #endregion

    #region Constructors

    public SignaturJobIdEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}