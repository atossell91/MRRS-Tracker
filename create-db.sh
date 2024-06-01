sqlite3 MRRS.db <<EOF
.read ./SQL/create-inspector-table.sql
.read ./SQL/create-activity-table.sql
.read ./SQL/create-inspector-activity-table.sql
INSERT INTO Inspector (FirstName, LastName) VALUES ('Rubber', 'Duck');
DELETE FROM Inspector;
SELECT * FROM RecordOfInspectorChanges;
DELETE FROM RecordOfInspectorChanges;
EOF
