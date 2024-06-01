using System.Data.SQLite;

public interface IDbParser<T> {
    T Parse(SQLiteDataReader reader);
}

public class InspectorDbParser : IDbParser<Inspector> {

    public Inspector Parse(SQLiteDataReader reader)
    {
        Inspector inspector = new Inspector() {
            ID = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
        };
        return inspector;
    }
}

public class ActivityDbParser : IDbParser<Activity> {
    public Activity Parse(SQLiteDataReader reader) {
        Activity activity = new Activity() {
            ID = reader.GetInt32(0),
            Name = reader.GetString(1),
        };
        return activity;
    }
}

public class InspectorActivityDbParser : IDbParser<InspectorActivity> {

    public InspectorActivity Parse(SQLiteDataReader reader)
    {
        InspectorActivity inspectorActivity = new InspectorActivity() {
            ID = reader.GetInt32(0),
            InspectorName = reader.GetString(1),
            InspectorID = reader.GetInt32(2),
            ActivityName = reader.GetString(3),
            ActivityID = reader.GetInt32(4),
            Hours = reader.GetDouble(5),
            PeriodStart = reader.GetDateTime(6),
            PeriodEnd = reader.GetDateTime(7),
        };

        return inspectorActivity;
    }
}
