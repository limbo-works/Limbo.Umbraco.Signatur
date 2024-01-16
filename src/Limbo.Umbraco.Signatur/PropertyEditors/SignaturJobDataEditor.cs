using Limbo.Umbraco.Signatur.Factories;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
public class SignaturJobDataEditor : DataEditor {

    private readonly SignaturJobDataPropertyIndexValueFactory _indexValueFactory;

    #region Constants

    internal const string EditorAlias = "Limbo.Umbraco.Signatur.JobData";

    internal const string EditorName = "Limbo Signatur Job Data";

    internal const string EditorView = "/App_Plugins/Limbo.Umbraco.Signatur/Views/JobData.html";

    internal const string EditorIcon = "icon-limbo-signatur color-limbo";

    #endregion

    #region Constructors

    public SignaturJobDataEditor(IDataValueEditorFactory dataValueEditorFactory, SignaturJobDataPropertyIndexValueFactory indexValueFactory) : base(dataValueEditorFactory) {
        _indexValueFactory = indexValueFactory;
    }

    #endregion

    #region Member methods

    public override IPropertyIndexValueFactory PropertyIndexValueFactory => _indexValueFactory;

    #endregion

}