namespace trafficFineManager.Models
{
    public class DashboardViewModel
    {

        public int ApprovedCount { get; set; }
        public decimal ApprovedTotalAmount { get; set; }

        public int PendingCount { get; set; }
        public decimal PendingTotalAmount { get; set; }

        public int RejectedCount { get; set; }

        public int TotalCount { get; set; }

        public decimal DailyTotal { get; set; }
        public decimal WeeklyTotal { get; set; }
        public decimal MonthlyTotal { get; set; }
        public decimal YearlyTotal { get; set; }
        
        public int DailyCount { get; set; }
        public int WeeklyCount { get; set; }
        public int MonthlyCount { get; set; }
        public int YearlyCount { get; set; }

        public decimal DailyAverage => DailyCount > 0 ? DailyTotal / DailyCount : 0;
        public decimal WeeklyAverage => WeeklyCount > 0 ? WeeklyTotal / WeeklyCount : 0;
        public decimal MonthlyAverage => MonthlyCount > 0 ? MonthlyTotal / MonthlyCount : 0;
        public decimal YearlyAverage => YearlyCount > 0 ? YearlyTotal / YearlyCount : 0;

        public string MostFinedPersonName { get; set; } = "-";
        public string MostFinedPersonTC { get; set; } = "-";
        public int MostFinedPersonCount { get; set; }
        public decimal MostFinedPersonTotalAmount { get; set; }

        public string UserRole { get; set; } = "";
    }
}

