namespace ISC.IDRDownloader.Domain
{
    using Newtonsoft.Json;

    public class MyJson
    {
        [JsonProperty("changesSince")]
        public string ChangesSince { get; set; }
    }
}
