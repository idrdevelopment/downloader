namespace ISC.IDRDownloader.Domain
{
    public class Partner : Entity
    {
        public int PartnerId { get; set; }

        public string PartnerName { get; set; }

        public int EntityTypeId { get; set; }

        public string EntityTypeName { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return $"{EntityId},{Parse(EntityName)},{PartnerId},{Parse(PartnerName)},{EntityTypeId},{Parse(EntityTypeName)},{Parse(Status)}";
        }

        public static string Header => "Entity ID,Entity Name,Partner ID,Partner Name,Entity Type ID,Entity Type Name,Status";
    }
}
