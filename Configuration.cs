namespace ISC.IDRDownloader
{
    using System.Configuration;

    public struct Configuration
    {
        public static string ApiUrl => ConfigurationManager.AppSettings["APIUrl"];
    }
}
