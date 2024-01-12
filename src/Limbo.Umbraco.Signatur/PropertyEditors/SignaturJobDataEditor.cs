using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
public class SignaturJobDataEditor : DataEditor {

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.JobData";

    internal const string EditorName = "Limbo Signatur Job Data";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/JobData.html";

    internal const string EditorIcon = "icon-limbo-signatur color-limbo";

    #endregion

    #region Constructors

    public SignaturJobDataEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}