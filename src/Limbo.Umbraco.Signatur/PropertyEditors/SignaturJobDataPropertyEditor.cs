using Limbo.Umbraco.Signatur.Factories;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, ValueType = EditorValueType)]
public class SignaturJobDataPropertyEditor : DataEditor {

    private readonly SignaturJobDataPropertyIndexValueFactory _indexValueFactory;

    #region Constants

    public const string EditorAlias = "Limbo.Umbraco.Signatur.JobData";

    public const string EditorUiAlias = "Limbo.Umbraco.Signatur.PropertyEditorUi.JobData";

    public const string EditorName = "Limbo Signatur Job Data";

    public const string EditorIcon = "icon-limbo-signatur";

    public const string EditorValueType = ValueTypes.Json;

    #endregion

    #region Constructors

    public SignaturJobDataPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, SignaturJobDataPropertyIndexValueFactory indexValueFactory) : base(dataValueEditorFactory) {
        _indexValueFactory = indexValueFactory;
    }

    #endregion

    #region Member methods

    public override IPropertyIndexValueFactory PropertyIndexValueFactory => _indexValueFactory;

    #endregion

}