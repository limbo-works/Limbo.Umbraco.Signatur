namespace Limbo.Umbraco.Signatur.Models.Import;

public class SignaturImportOptions {

    public bool Write { get; set; } = true;

    public SignaturImportOptions() { }

    public SignaturImportOptions(bool write) {
        Write = write;
    }

}