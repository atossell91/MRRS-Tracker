using System.Data.SQLite;

public class InspectorDbParser {
    public static Inspector ParseInspector(SQLiteDataReader reader) {
        Inspector inspector = new Inspector() {
            ID = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
        };
        return inspector;
    }
}

public class ActivityDbParser {
    public static Activity ParseActivity(SQLiteDataReader reader) {
        Activity activity = new Activity() {
            ID = reader.GetInt32(0),
            Name = reader.GetString(1),
        };
        return activity;
    }
}

public class InspectorActivityDbParser {
    public static InspectorActivity ParseInspectorActivity (SQLiteDataReader reader) {
        InspectorActivity inspectorActivity = new InspectorActivity() {
            ID = reader.GetInt32(0),
            InspectorID = reader.GetInt32(1),
            ActivityID = reader.GetInt32(2),
            Hours = reader.GetDouble(3),
            PeriodStart = reader.GetDateTime(4),
            PeriodEnd = reader.GetDateTime(5),
        };

        return inspectorActivity;
    }
}
