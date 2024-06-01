// See https://aka.ms/new-console-template for more information

using System;

class mrrscli {
    public static void Main(string[] args) {

        MRRS mrrs = new MRRS("C:\\Users\\atoss\\source\\repos\\atossell91\\MRRS-Tracker\\testroot");
        mrrs.CreateDb();

        MRRSTester tester = new MRRSTester();
        tester.AddTestActivity(mrrs);
        tester.AddTestInspector(mrrs);

        InspectorActivity activity = new InspectorActivity() {
            InspectorID = 1,
            ActivityID = 1,
            Hours = 1.75,
            PeriodStart = new DateTime(2024, 09, 10),
            PeriodEnd = new DateTime(2024, 09, 22)
        };

        mrrs.AddTime(activity);
    }
}

