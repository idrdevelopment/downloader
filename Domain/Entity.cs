namespace ISC.IDRDownloader.Domain
{
    using System;

    public abstract class Entity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        protected string Parse(string item)
        {
            return item ?? string.Empty;
        }

        protected string ParseDate(DateTime? item)
        {
            return item.HasValue ? item.Value.ToShortDateString() : string.Empty;
        }

        protected string ParseInt(int? item)
        {
            return item.HasValue ? item.Value.ToString() : string.Empty;
        }
    }
}
