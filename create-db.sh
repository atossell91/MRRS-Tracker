sqlite3 MRRS.db <<EOF
.read ./SQL/create-inspector-table.sql
INSERT INTO Inspector (FirstName, LastName) VALUES ('Rubber', 'Duck');
DELETE FROM Inspector;
SELECT * FROM RecordOfInspectorChanges;
DELETE FROM RecordOfInspectorChanges;
EOF
