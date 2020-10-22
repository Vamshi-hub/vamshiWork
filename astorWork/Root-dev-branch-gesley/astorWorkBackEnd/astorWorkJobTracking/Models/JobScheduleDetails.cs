using System;

namespace astorWorkJobTracking.Models
{
    public class JobScheduleDetails
    {
        public int ID { get; set; }
        public int MaterialID { get; set; }
        public string MarkingNo { get; set; }
        public string Zone { get; set; }
        public string Level { get; set; }
        public string Block { get; set; }
        public string MaterialType { get; set; }
        public int TradeID { get; set; }
        public string TradeName { get; set; }

        public int SubConID { get; set; }
        public string SubConName { get; set; }
        public DateTimeOffset? Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public DateTimeOffset? ActualStartDate { get; set; }
        public DateTimeOffset? ActualEndDate { get; set; }
        public DateTimeOffset? PlannedStartDate { get; set; }
        //public DateTimeOffset? plannedEndDate { get; set; }

        //public DateTimeOffset? QCStartDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public bool IsUpdated { get; set; }

        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public bool RouteToRTO { get; set; }
        public int RtoInspectionStatusCode { get; set; }
        public string RtoInspectionStatus { get; set; }
        public int Type { get; set; }

    }
}
