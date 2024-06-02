using System;

namespace mrrslib
{
    public class InspectorActivity
    {
        public int ID { get; set; }
        public string InspectorName { get; set; }
        public int InspectorID { get; set; }
        public string ActivityName { get; set; }
        public int ActivityID { get; set; }
        public double Hours { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
