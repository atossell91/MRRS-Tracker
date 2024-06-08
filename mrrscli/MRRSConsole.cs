using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using mrrslib;
using System.Reflection;
using System.IO;

public class MRRSConsole {
    public void Run() {
        
        string currentDir = Assembly.GetCallingAssembly().Location;
        FileInfo fileInfo = new FileInfo(currentDir);
        Configs configs = Configs.LoadConfigs(fileInfo.DirectoryName + "/configs.txt");
        Console.WriteLine($"DB Path: {configs.DatabasePath}");

        var mrrs = new MRRS(configs.DatabasePath);

        string userInput = String.Empty;
        while (userInput != "exit") {
            Console.Write("Enter a command: ");
            userInput = Console.ReadLine();
            switch (userInput) {
                case "help":
                    DisplayHelp();
                    break;
                case "list inspectors":
                    var inspectors = mrrs.GetInspectors();
                    DisplayInspectors(inspectors);
                    break;
                case "list activities":
                    var activities = mrrs.GetActivities();
                    DisplayActivities(activities);
                    break;
                case "list inspector activities":
                    var inspectorActivities = mrrs.GetActivityList();
                    DisplayInspectorActivity(inspectorActivities);
                    break;
                case "filter inspector activities":
                    var inspectorActivityFilter = buildInspectorActivityFilter();
                    var fInspectorActivities = mrrs.GetFilteredActivityList(inspectorActivityFilter);
                    DisplayInspectorActivity(fInspectorActivities);
                    break;
                case "add inspector activity":
                    var ia = BuildInspectorActivity();
                    mrrs.AddTime(ia);
                    break;
                case "add inspector":
                    var inspector = BuildInspector();
                    mrrs.AddInspector(inspector);
                    break;
                case "add activity":
                    var act = BuildActivity();
                    mrrs.AddActivity(act);
                    break;
                case "delete inspector":
                    int id = promptForInteger("Enter an Inspector ID: ");
                    mrrs.DeleteInspector(id);
                    break;
                case "delete inspector activity":
                    int recordId = promptForInteger("Enter a record ID: ");
                    mrrs.DeleteInspectorActivity(recordId);
                    break;
                case "delete activity":
                    int activityId = promptForInteger("Enter an Activity ID: ");
                    mrrs.DeleteActivity(activityId);
                    break;
                case "create database":
                    mrrs.CreateDb();
                    break;
                case "exit":
                    break;
            }
        }
    }

    public static InspectorActivityFilter buildInspectorActivityFilter() {
        Console.Write("Enter an inspector ID or -1 to skip: ");
        int inspectorId = -1;
        while (!int.TryParse(Console.ReadLine(), out inspectorId)) {
            Console.Write("Invalid input. Please enter a number: ");
        }

        Console.Write("Enter an activity ID or -1 to skip: ");
        int activityId = -1;
        while (!int.TryParse(Console.ReadLine(), out activityId)) {
            Console.Write("Invalid input. Please enter a number: ");
        }

        DateTime startDate = DateTime.MinValue;
        do {
            
            Console.Write("Enter a range start date in the format 'YYYY-MM-DD', or 's' to skip: ");
            string input = Console.ReadLine();
            if (input == "s") {
                break;
            }
            else if (DateTime.TryParse(input, out startDate)) {
                break;
            }

        } while (startDate == DateTime.MinValue);
        

        InspectorActivityFilter filter = new InspectorActivityFilter() {
            InspectorID = inspectorId,
            ActivityID = activityId
        };

        DateTime endDate = DateTime.MinValue;
        if (startDate != DateTime.MinValue) {
            do {
                Console.Write("Enter a range end date in the format 'YYYY-MM-DD': ");
                string input = Console.ReadLine();
                if (DateTime.TryParse(input, out endDate)) {
                    break;
                }

            } while (endDate == DateTime.MinValue);

            filter.Dates = new DateRange {
                Start = startDate,
                End = endDate
            };
        }

        return filter;
    }

    public static InspectorActivity BuildInspectorActivity() {
        Console.Write("Enter an inspector ID: ");
        int inspectorId = -1;
        while (!int.TryParse(Console.ReadLine(), out inspectorId)) {
            Console.Write("Invalid input. Please enter a number: ");
        }

        Console.Write("Enter an activity ID: ");
        int activityId = -1;
        while (!int.TryParse(Console.ReadLine(), out activityId)) {
            Console.Write("Invalid input. Please enter a number: ");
        }

        Console.Write("Enter time (in hours): ");
        double hours = 0.0;
        while (!double.TryParse(Console.ReadLine(), out hours)) {
            Console.Write("Invalid input. Please enter a number: ");
        }

        DateTime date = DateTime.MinValue;

        do {
            
            Console.Write("Enter a date in the format 'YYYY-MM-DD', or 't' or 'y' for today or yesterday's date: ");
            string input = Console.ReadLine();
            if (input == "t") {
                date = DateTime.Now;
                break;
            }
            if (input == "y") {
                date = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
            else if (DateTime.TryParse(input, out date)) {
                break;
            }

        } while (date == DateTime.MinValue);

        Console.Write("Enter a comment (or press enter to skip): ");
        string comment = Console.ReadLine();

        InspectorActivity inspectorActivity = new InspectorActivity();
        inspectorActivity.InspectorID = inspectorId;
        inspectorActivity.ActivityID = activityId;
        inspectorActivity.Hours = hours;
        inspectorActivity.PeriodStart = date;
        //inspectorActivity.PeriodEnd = date;
        inspectorActivity.Comment = comment;

        return inspectorActivity;
    }

    public static Inspector BuildInspector() {

        Console.Write("Enter first name: ");
        string firstName = Console.ReadLine();

        Console.Write("Enter last name: ");
        string lastName = Console.ReadLine();
        
        Inspector inspector = new Inspector();
        inspector.FirstName = firstName;
        inspector.LastName = lastName;

        return inspector;
    }

    public static Activity BuildActivity() {

        Console.Write("Enter name: ");
        string name = Console.ReadLine();
        
        Activity activity = new Activity();
        activity.Name = name;

        return activity;
    }

    private static int promptForInteger(string message) {
        Console.Write(message);
        int number = -1;
        while (!int.TryParse(Console.ReadLine(), out number)) {
            Console.Write("Please enter a number: ");
        }
        return number;
    }

    public static void DisplayInspectors(IEnumerable<Inspector> inspectors) {
        DisplayCLI inspectorDisplay = new DisplayCLI();

        int idWidth = 4;
        int nameWidth = 20;

        inspectorDisplay.AddColumn(new DisplayCLI.Column() {
            Width = idWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
        });
        inspectorDisplay.AddColumn(new DisplayCLI.Column() {
            Width = nameWidth,
        });
        inspectorDisplay.AddColumn(new DisplayCLI.Column() {
            Width = nameWidth,
        });
    
        inspectorDisplay.PrintDivider("Inspectors");
        inspectorDisplay.PrintRow("ID", "FIRST NAME", "LAST NAME");
        inspectorDisplay.PrintDivider("");
        foreach (var inspector in inspectors) {
            inspectorDisplay.PrintRow(inspector.ID.ToString(), inspector.FirstName, inspector.LastName);
        }
        inspectorDisplay.PrintDivider("");
    }

    public static void DisplayActivities(IEnumerable<Activity> activities) {
        DisplayCLI activityDisplay = new DisplayCLI();

        int idWidth = 4;
        int nameWidth = 20;

        activityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = idWidth
        });
        activityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = nameWidth
        });
    
        activityDisplay.PrintDivider("Activities");
        activityDisplay.PrintRow("ID", "NAME");
        activityDisplay.PrintDivider("");
        foreach (var activity in activities) {
            activityDisplay.PrintRow(activity.ID.ToString(), activity.Name);
        }
        activityDisplay.PrintDivider("");
    }

    public static void DisplayInspectorActivity(IEnumerable<InspectorActivity> inspectorActivities) {
        int idWidth = 4;
        int nameWidth = 20;
        int numWidth = 8;
        int dateWidth = 15;
        int commentWidth = 40;
        
        DisplayCLI inspectorActivityDisplay = new DisplayCLI();
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = idWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
            RightBorder = "|| "
        });
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = nameWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
        });
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = nameWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
        });
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = numWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
        });
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = dateWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Centre,
        });
        inspectorActivityDisplay.AddColumn(new DisplayCLI.Column() {
            Width = commentWidth,
            Justification = DisplayCLI.Column.ColumnJustification.Left
        });

        inspectorActivityDisplay.PrintDivider("Inspector Activities");
        inspectorActivityDisplay.PrintRow("ID", "Inspector", "Activity", "Hours", "Date", "Comment   ");
        inspectorActivityDisplay.PrintDivider("");
        foreach (var inspectorActivity in inspectorActivities) {
            inspectorActivityDisplay.PrintRow(
                inspectorActivity.ID.ToString(),
                inspectorActivity.InspectorFirstName,
                inspectorActivity.ActivityName,
                inspectorActivity.Hours.ToString(),
                inspectorActivity.PeriodStart.ToString("yyyy-MM-dd"),
                inspectorActivity.Comment
            );
        }
        inspectorActivityDisplay.PrintDivider("");
    }

    public static void DisplayHelp() {
        int colWidth = 30;
        int itemIndent = 0;

        DisplayCLI display = new DisplayCLI();
        
        display.AddColumn(new DisplayCLI.Column() {
            Width = colWidth,
            LeftBorder = "|",
            RightBorder = "|"
        });
        
        display.AddColumn(new DisplayCLI.Column());

        display.PrintDivider("Input Options");
        display.PrintRow("Command", "Description");
        display.PrintDivider("");
        display.PrintRow(itemIndent, "help:", "Show input options.");
        display.PrintRow(itemIndent, "exit:", "Exit the program.");
        display.PrintRow(itemIndent, "list inspector activities:", "List all inspector activities.");
        display.PrintRow(itemIndent, "list inspector:", "List all activities.");
        display.PrintRow(itemIndent, "list activities:", "List all inspectors.");
        display.PrintRow(itemIndent, "add inspector activity:", "Add a new inspector activity.");
        display.PrintRow(itemIndent, "add inspector:", "Add a new inspector.");
        display.PrintRow(itemIndent, "add activity:", "Add a new activity.");
        display.PrintDivider("");
        Console.WriteLine();
    }
}