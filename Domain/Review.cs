namespace ISC.IDRDownloader.Domain
{
    using System;

    public class Review : Entity
    {
        public string CurrentStatus { get; set; }

        public string CurrentReviewBy { get; set; }

        public DateTime? CurrentReviewDate { get; set; }

        public string CurrentDeficientReason { get; set; }

        public string CurrentDeficientReasonNotes { get; set; }

        public DateTime? LastApprovedDate { get; set; }

        public string LastApprovedBy { get; set; }

        public string RiskLevel { get; set; }

        public int? RiskRatingLevelId { get; set; }

        public override string ToString()
        {
            return $"{EntityId},{Parse(EntityName)},{Parse(CurrentStatus)},{Parse(CurrentReviewBy)},{ParseDate(CurrentReviewDate)},{Parse(CurrentDeficientReason)},{Parse(CurrentDeficientReasonNotes)},{ParseDate(LastApprovedDate)},{Parse(LastApprovedBy)},{Parse(RiskLevel)},{ParseInt(RiskRatingLevelId)}";
        }

        public static string Header => "Entity ID,Entity Name,Current Status,Current Review By,Current Review Date,Current Deficient Reason,Current Deficient Reason Notes,Last Approved Date,Last Approved By,Risk Level,Risk Rating Level ID";
    }
}
