using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Collections.ObjectModel;

public class MRRS {
    public readonly string SQLDateFormatStr = "yyyy-MM-dd HH:mm";
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

            addData(sqlCmd);
    }

    public void AddActivity(Activity activity) {
        string sql = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "add-activity.sql"));

            string sqlCmd = String.Format(sql,
            activity.Name);

            addData(sqlCmd);
    }

    public void AddTime(InspectorActivity inspectorActivity) {
        string sql = Utilities.LoadTextFile(
            Path.Combine(SQLDir, "insert-time.sql"));

        string sqlCmd =  String.Format(sql,
            inspectorActivity.InspectorID,
            inspectorActivity.ActivityID,
            inspectorActivity.Hours,
            inspectorActivity.PeriodStart.ToString(SQLDateFormatStr),
            inspectorActivity.PeriodEnd.ToString(SQLDateFormatStr)
            );
        
        addData(sqlCmd);
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

    private void addData(string sqlStr) {
        using (
            var con = new SQLiteConnection(ConnectionString)) {
            con.BusyTimeout = WriteTimeout;
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = sqlStr;
            
            cmd.ExecuteNonQuery();
        }
    }
}