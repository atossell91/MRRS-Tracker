using mrrslib;

public class MRRSTester {
    public MRRSTester()
    {}

    public void AddTestActivity(MRRS mrrs) {
        var activity = new Activity() {
            ID = 0,
            Name = "Admin",
        };

        mrrs.AddActivity(activity);
    }

    public void AddTestInspector(MRRS mrrs) {
        var inspector = new Inspector() {
            ID = 0,
            FirstName = "Harry",
            LastName = "Potter"
        };

        mrrs.AddInspector(inspector);
    }
}