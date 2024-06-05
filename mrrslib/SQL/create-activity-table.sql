CREATE TABLE Activity (
    ID INTEGER PRIMARY KEY,
    Name TEXT NOT NULL
);

CREATE TABLE RecordOfActivityChanges (
    ID INTEGER PRIMARY KEY,
    Logtime TEXT NOT NULL,
    Description TEXT NOT NULL
);

CREATE TRIGGER OnActivityINSERT
AFTER INSERT ON Activity
BEGIN
    INSERT INTO RecordOfActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "INSERT");
END;

CREATE TRIGGER OnActivityUPDATE
AFTER UPDATE ON Activity
BEGIN
    INSERT INTO RecordOfActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "UPDATE");
END;

CREATE TRIGGER OnActivityDELETE
AFTER DELETE ON Activity
BEGIN
    INSERT INTO RecordOfActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "DELETE");
END;
