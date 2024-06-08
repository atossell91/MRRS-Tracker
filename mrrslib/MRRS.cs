using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using System.Reflection;
using Dapper;
using System.Data.SqlClient;

namespace mrrslib
{
    public class MRRS {
        public event EventHandler DatabaseDataChanged;
        public readonly string SQLDateTimeFormatStr = "yyyy-MM-dd HH:mm";
        public readonly string SQLDateFormatStr = "yyyy-MM-dd";
        public readonly string SQLTimeFormatStr = "HH:mm";
        public readonly string SQLDir;
        public const int WriteTimeout = 10000;
        public MRRS(string dbPath)
        {
            FileInfo fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            SQLDir = Path.Combine(fileInfo.DirectoryName, "SQL");
            DbPath = dbPath;
            
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

                Console.WriteLine(ConnectionString);
                SQLiteConnection.CreateFile(DbPath);
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

                modifyData(sql, new {
                    FirstName = inspector.FirstName,
                    LastName = inspector.LastName
                });
        }

        public void AddActivity(Activity activity) {
            string sql = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "add-activity.sql"));

                string sqlCmd = String.Format(sql,
                activity.Name);

                modifyData(sqlCmd, new {ActivityName=activity.Name});
        }

        public void AddTime(InspectorActivity inspectorActivity) {
            string sql = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "insert-time.sql"));
        
            modifyData(sql, new {
                InspectorID = inspectorActivity.InspectorID,
                ActivityID = inspectorActivity.ActivityID,
                Hours = inspectorActivity.Hours,
                PeriodStart = inspectorActivity.PeriodStart.ToString(SQLDateFormatStr),
                PeriodStartTime = inspectorActivity.PeriodStart.ToString(SQLTimeFormatStr),
                //inspectorActivity.PeriodEnd.ToString(SQLDateTimeFormatStr)
                Comment = inspectorActivity.Comment
            });
        }

        public void DeleteInspector(int inspectorID) {
            string sqlStr = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "delete-inspector.sql"));
            modifyData(sqlStr, new { ID = inspectorID });
        }

        public void DeleteActivity(int activityID) {
            string sqlStr = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "delete-activity.sql"));
            modifyData(sqlStr, new { ID = activityID});
        }

        public void DeleteInspectorActivity(int inspectorActivityID) {
            string sqlStr = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "delete-inspector-activity.sql"));
            modifyData(sqlStr, new {ID=inspectorActivityID});
        }

        public ObservableCollection<Inspector> GetInspectors() {
            string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-inspectors.sql"));
            return readData<Inspector>(sqlStr);
        }

        public ObservableCollection<Activity> GetActivities() {
            string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-activities.sql"));
            return readData<Activity>(sqlStr);
        }


        public ObservableCollection<InspectorActivity> GetActivityList() {
            string sqlStr = Utilities.LoadTextFile(Path.Combine(SQLDir, "get-all-inspector-activities.sql"));
            return readData<InspectorActivity>(sqlStr);
        }

        public ObservableCollection<InspectorActivity> GetFilteredActivityList(InspectorActivityFilter filter) {
            string templateStr = Utilities.LoadTextFile(
                Path.Combine(SQLDir, "get-activities-filtered.sql"));
            
            var builder = new SqlBuilder();

            if (filter.ActivityID >= 0) {
                builder.Where("ActivityID=@ActivityID", new {ActivityID=filter.ActivityID});
            }

            if (filter.InspectorID >= 0) {
                builder.Where("InspectorID=@InspectorID", new {InspectorID=filter.InspectorID});
            }

            if (filter.Dates != null) {
                builder.Where("PeriodStart >= @StartDate",
                new {StartDate=filter.Dates.Start.ToString(SQLDateFormatStr)});
                builder.Where("PeriodStart <= @EndDate",
                new {EndDate=filter.Dates.End.ToString(SQLDateFormatStr)});
            }

            var qry = builder.AddTemplate(templateStr);
            
            return readData<InspectorActivity>(qry.RawSql, qry.Parameters);
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
                output = con.ExecuteScalar<string>(SqlString);
            }

            return output;
        }

        private ObservableCollection<T> readData<T>(string sqlStr, object parameters = null) {
            var collection = new ObservableCollection<T>();
            using ( var con = new SQLiteConnection(ConnectionString)) {
                collection = new ObservableCollection<T>(con.Query<T>(sqlStr, parameters));
            }
            return collection;
        }

        private void modifyData(string sqlStr, object parameters) {
            int rowsChanged = 0;
            using (
                var con = new SQLiteConnection(ConnectionString)) {
                    rowsChanged = con.Execute(sqlStr, parameters);
            }
        }
    }
}