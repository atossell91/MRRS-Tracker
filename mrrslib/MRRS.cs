using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Text;

public class MRRS {
    public event EventHandler DatabaseDataChanged;
    public readonly string SQLDateTimeFormatStr = "yyyy-MM-dd HH:mm";
    public readonly string SQLDateFormatStr = "yyyy-MM-dd";
    public readonly string RootDir;
    public readonly string SQLDir;
    public const int WriteTimeout = 10000;
    public MRRS(string rootPath)
    {
        RootDir = rootPath;
        SQLDir = Path.Combine(RootDir, "SQL");
        DbPath = Path.Combine(rootPath, "MRRS.db");
        
        ConnectionString = String.Format("Data Source = {0};", DbPath);
    }

    public readonly string DbPath;
    public readonly string ConnectionString;

    public void CreateDb() {

        if (!File.Exists(DbPath)) {

            List<string> initFilesList = new List<string> {
                Path.Combine(SQLDir, "create-activity-table.sql"),
                Path.Combine(SQLDir, "create-inspector-table.sql"),
                Path.Combine(SQLDir, "create-inspector-activity-table.sql"),
            };

            List<string> sqlCommands = new List<string>();
            foreach (string filename in initFilesList) {
                sqlCommands.Add(Utilities.LoadTextFile(filename));
            }

            using (var con = new SQLiteConnection(ConnectionString)) {
                con.Open();

                var cmd = con.CreateCommand();
                foreach (string cmdStr in sqlCommands) {
                    cmd.CommandText = cmdStr;
                    cmd.ExecuteNonQuery();
                }

            }
        }
    }

    public void AddInspector(Inspector inspector) {
        string sql = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "add-inspector.sql"));

            string sqlCmd = String.Format(sql,
            inspector.FirstName, inspector.LastName);

            modifyData(sqlCmd);
    }

    public void AddActivity(Activity activity) {
        string sql = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "add-activity.sql"));

            string sqlCmd = String.Format(sql,
            activity.Name);

            modifyData(sqlCmd);
    }

    public void AddTime(InspectorActivity inspectorActivity) {
        string sql = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "insert-time.sql"));

        string sqlCmd =  String.Format(sql,
            inspectorActivity.InspectorID,
            inspectorActivity.ActivityID,
            inspectorActivity.Hours,
            inspectorActivity.PeriodStart.ToString(SQLDateTimeFormatStr),
            inspectorActivity.PeriodEnd.ToString(SQLDateTimeFormatStr)
            );
        
        modifyData(sqlCmd);
    }

    public void DeleteInspector(int inspectorID) {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "delete-inspector.sql"));
        modifyData(String.Format(sqlStr, inspectorID));
    }

    public void DeleteActivity(int activityID) {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "delete-inspector.sql"));
        modifyData(String.Format(sqlStr, activityID));
    }

    public void DeleteInspectorActivity(int inspectorActivityID) {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "delete-inspector-activity.sql"));
        modifyData(String.Format(sqlStr, inspectorActivityID));
    }

    public ObservableCollection<Inspector> GetInspectors() {
        string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-inspectors.sql"));
        return readData<Inspector>(sqlStr, new InspectorDbParser());
    }

    public ObservableCollection<Activity> GetActivities() {
        string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-activities.sql"));
        return readData<Activity>(sqlStr, new ActivityDbParser());
    }


    public ObservableCollection<InspectorActivity> GetActivityList() {
        string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-inspector-activities.sql"));
        return readData<InspectorActivity>(sqlStr, new InspectorActivityDbParser());
    }

    public ObservableCollection<InspectorActivity> GetFilteredActivityList(InspectorActivity inspectorActivity) {
        string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-activities-filtered.sql"));

        List<string> conditions = new List<string>();
        if (inspectorActivity.InspectorID >= 0) {
            conditions.Add(String.Format("Inspector.ID = {0}", inspectorActivity.InspectorID));
        }

        if (inspectorActivity.ActivityID >= 0) {
            conditions.Add(String.Format("Activity.ID = {0}", inspectorActivity.ActivityID));
        }

        if (inspectorActivity.PeriodStart != null && inspectorActivity.PeriodEnd == null) {
            conditions.Add(String.Format("InspectorActivity.PeriodStart = {0}",
                inspectorActivity.PeriodStart.ToString(SQLDateFormatStr)));
        }

        if (inspectorActivity.PeriodStart != null && inspectorActivity.PeriodEnd != null) {
            conditions.Add(String.Format(
                "InspectorActivity.PeriodStart <= {0} AND InspectorActivity.PeriodEnd >= {1}",
                inspectorActivity.PeriodEnd.ToString(SQLDateFormatStr),
                inspectorActivity.PeriodStart.ToString(SQLDateFormatStr)));
        }

        StringBuilder builder = new StringBuilder();

        if (conditions.Count > 0) {
            builder.Append(" WHERE ");
        }

        for (int i = 0; i < conditions.Count; ++i) {
            builder.Append(String.Format("{0} ", conditions[i]));
            if (i < conditions.Count - 1) {
                builder.Append("AND ");
            }
        }

        string whereConditions = builder.ToString();
        string completeSql = String.Format(sqlStr, whereConditions);
        Console.WriteLine(completeSql);

        return readData<InspectorActivity>(completeSql, new InspectorActivityDbParser());
    }

    public DateTime GetLastDbUpdateTime() {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "check-update-logs.sql"));
        string dateStr = readSingleValue(sqlStr);
        DateTime dateTime = DateTime.Parse(dateStr);
        return dateTime;
    }

    public DateTime GetLastInspectorUpdateTime() {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "check-inspector-update-logs.sql"));
        string dateStr = readSingleValue(sqlStr);
        DateTime dateTime = DateTime.Parse(dateStr);
        return dateTime;
    }

    public DateTime GetLastActivityUpdateTime() {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "check-activity-update-logs.sql"));
        string dateStr = readSingleValue(sqlStr);
        DateTime dateTime = DateTime.Parse(dateStr);
        return dateTime;
    }

    public DateTime GetLastInspectorActivityUpdateTime() {
        string sqlStr = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "check-inspector-activity-update-logs.sql"));
        string dateStr = readSingleValue(sqlStr);
        DateTime dateTime = DateTime.Parse(dateStr);
        return dateTime;
    }

    private string readSingleValue(string SqlString) {
        string output = string.Empty;
        using (var con = new SQLiteConnection(ConnectionString)) {
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = SqlString;
            output = cmd.ExecuteScalar().ToString();
        }

        return output;
    }

    private ObservableCollection<T> readData<T>(string sqlStr, IDbParser<T> parser) {
        var collection = new ObservableCollection<T>();
        using ( var con = new SQLiteConnection(ConnectionString)) {
            con.BusyTimeout = WriteTimeout;
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = sqlStr;
            
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    collection.Add(parser.Parse(reader));
                }
            }
        }
        return collection;
    }

    private void modifyData(string sqlStr) {
        int rowsChanged = 0;
        using (
            var con = new SQLiteConnection(ConnectionString)) {
            con.BusyTimeout = WriteTimeout;
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = sqlStr;
            
            rowsChanged = cmd.ExecuteNonQuery();
        }

        if (rowsChanged > 0) {
            DatabaseDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}