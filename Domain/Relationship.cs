namespace ISC.IDRDownloader.Domain
{
    public class Relationship : Entity
    {
        public int RelatedEntityId { get; set; }

        public string RelatedEntityName { get; set; }

        public string EntityRelationshipTypeName { get; set; }

        public override string ToString()
        {
            return $"{EntityId},{Parse(EntityName)},{RelatedEntityId},{Parse(RelatedEntityName)},{Parse(EntityRelationshipTypeName)}";
        }

        public static string Header => "Entity ID,Entity Name,Related Entity ID, Related Entity Name, Entity Relationship Type Name";
    }
}
