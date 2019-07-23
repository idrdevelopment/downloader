namespace ISC.IDRDownloader.Domain
{
    public class Arguments
    {
        public Arguments()
        {
            DownloadType = "all";
            SavePath = "C:\\Temp";
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DownloadType { get; set; }

        public string PartnerId { get; set; }

        public string ChangesSince { get; set; }

        public string ChangesSincePartners { get; set; }

        public string ChangesSinceRelationships { get; set; }

        public string ChangesSinceReviews { get; set; }

        public string SavePath { get; set; }

        public string EntityUrl { get; set; }

        public string RelationshipUrl { get; set; }

        public string ReviewUrl { get; set; }
    }
}
