SELECT InspectorActivity.ID, Inspector.FirstName, Inspector.ID, Activity.Name, Activity.ID, Hours, PeriodStart, PeriodEnd
FROM InspectorActivity
INNER JOIN Inspector ON Inspector.ID = InspectorID
INNER JOIN Activity ON Activity.ID = ActivityID;