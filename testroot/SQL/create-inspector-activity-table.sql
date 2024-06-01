CREATE TABLE InspectorActivity (
    ID INTEGER PRIMARY KEY,
    InspectorID INTEGER NOT NULL,
    ActivityID INTEGER NOT NULL,
    PeriodStart TEXT NOT NULL,
    PeriodEnd TEXT NOT NULL,
    Hours REAL NOT NULL,
    FOREIGN KEY (InspectorID) REFERENCES Inspector(ID),
    FOREIGN KEY (ActivityID) REFERENCES Activity(ID)
);

CREATE TABLE RecordOfInspectorActivityChanges (
    ID INTEGER PRIMARY KEY,
    Logtime TEXT NOT NULL,
    Description TEXT NOT NULL
);

CREATE TRIGGER OnInspectorActivityINSERT
AFTER INSERT ON InspectorActivity
BEGIN
    INSERT INTO RecordOfInspectorActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "INSERT");
END;

CREATE TRIGGER OnInspectorActivityUPDATE
AFTER UPDATE ON InspectorActivity
BEGIN
    INSERT INTO RecordOfInspectorActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "UPDATE");
END;

CREATE TRIGGER OnInspectorActivityDELETE
AFTER DELETE ON InspectorActivity
BEGIN
    INSERT INTO RecordOfInspectorActivityChanges (Logtime, Description)
    VALUES (datetime('now', 'localtime'), "DELETE");
END;
