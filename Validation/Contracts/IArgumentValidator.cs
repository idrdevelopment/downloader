namespace ISC.IDRDownloader.Validation.Contracts
{
    using System.Text;

    public interface IArgumentValidator
    {
        StringBuilder Validate(string[] args);
    }
}
