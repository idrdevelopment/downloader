namespace ISC.IDRDownloader.Domain
{
    using Newtonsoft.Json;

    public class MyJson
    {
        [JsonProperty("changesSincePartners")]
        public string ChangesSincePartners { get; set; }

        [JsonProperty("changesSinceRelationships")]
        public string ChangesSinceRelationships { get; set; }

        [JsonProperty("changesSinceReviews")]
        public string ChangesSinceReviews { get; set; }
    }
}
