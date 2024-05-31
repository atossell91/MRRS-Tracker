CREATE TABLE Inspector (
    ID INTEGER PRIMARY KEY,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL
);

CREATE TABLE RecordOfInspectorChanges (
    ID INTEGER PRIMARY KEY,
    Logtime TEXT NOT NULL,
    Description TEXT NOT NULL
);

CREATE TRIGGER OnInspectorINSERT
AFTER INSERT ON Inspector
BEGIN
    INSERT INTO RecordOfInspectorChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "INSERT");
END;

CREATE TRIGGER OnInspectorUPDATE
AFTER UPDATE ON Inspector
BEGIN
    INSERT INTO RecordOfInspectorChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "UPDATE");
END;

CREATE TRIGGER OnInspectorDELETE
AFTER DELETE ON Inspector
BEGIN
    INSERT INTO RecordOfInspectorChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "DELETE");
END;
