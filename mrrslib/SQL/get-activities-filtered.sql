SELECT InspectorActivity.ID, Inspector.FirstName AS InspectorFirstName, Inspector.ID AS InspectorID, Activity.Name AS ActivityName, Activity.ID AS ActivityID, Hours, PeriodStart
FROM InspectorActivity
INNER JOIN Inspector ON Inspector.ID = InspectorID
INNER JOIN Activity ON Activity.ID = ActivityID
/**where**/
/**orderby**/;