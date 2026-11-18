using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, ValueType = EditorValueType)]
public class SignaturJobIdPropertyEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Signatur.JobId";

    public const string EditorUiAlias = "Limbo.Umbraco.Signatur.PropertyEditorUi.JobId";

    public const string EditorName = "Limbo Signatur Job ID";

    public const string EditorIcon = "icon-limbo-signatur";

    public const string EditorGroup = "Limbo";

    public const string EditorValueType = ValueTypes.Integer;

    #endregion

    #region Constructors

    public SignaturJobIdPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}