namespace ISC.IDRDownloader.Services.Contracts
{
    using System.Text;
    using ISC.IDRDownloader.Domain;

    public interface IDownloadService
    {
        StringBuilder ValidateArguments(string[] args);

        Arguments Download(string[] args);
    }
}
