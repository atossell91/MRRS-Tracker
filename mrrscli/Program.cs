// See https://aka.ms/new-console-template for more information

using System;
using System.Runtime.InteropServices;

class MRRSCli {
    public static void Main(string[] args) {
        MRRS mrrs = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            mrrs = new MRRS("C:\\Users\\atoss\\source\\repos\\atossell91\\MRRS-Tracker\\testroot");
        }
        else {
            mrrs = new MRRS("/home/ant/Programming/MRRS-Tracker/testroot");
        }

        MRRSTester tester = new MRRSTester();
        var col = mrrs.GetInspectors();
        foreach (var row in col) {
            Console.WriteLine($"{row.FirstName} {row.LastName}");
        }
        //tester.AddTestInspector(mrrs);
        //mrrs.OpenClose();
        /*
        mrrs.CreateDb();

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
        */
    }
}

