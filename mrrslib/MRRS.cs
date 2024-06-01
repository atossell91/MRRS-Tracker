using System.Data.SQLite;

namespace mrrsns;

public class MRRS {
    public MRRS(string dbPath)
    {
        DbPath = dbPath;
    }

    public readonly string DbPath;

    public void AddTime(Inspector inspector, Activity activity) {
        string sql = Utilities.LoadTextFile("SQL/insert-time.sql");
        using (
            var con = new SQLiteConnection(String.Format("Data Source={0}",
            DbPath))) {
            con.Open();
        }
    }
}