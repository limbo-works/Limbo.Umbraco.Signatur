using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, ValueType = EditorValueType)]
public class SignaturLastUpdatedPropertyEditor : DataEditor {

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Signatur.LastUpdated";

    public const string EditorUiAlias = "Limbo.Umbraco.Signatur.PropertyEditorUi.LastUpdated";

    public const string EditorName = "Limbo Signatur Last Updated";

    public const string EditorIcon = "icon-limbo-signatur";

    public const string EditorGroup = "Limbo";

    public const string EditorValueType = ValueTypes.String;

    #endregion

    #region Constructors

    public SignaturLastUpdatedPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

    #endregion

}