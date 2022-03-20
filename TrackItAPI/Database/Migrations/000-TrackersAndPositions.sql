CREATE TABLE IF NOT EXISTS Trackers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL,
    Name TEXT NOT NULL,
    Notes TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Positions (
    TrackerId INTEGER NOT NULL,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    CollectDate TEXT NOT NULL,
    PRIMARY KEY (TrackerId, CollectDate)
    FOREIGN KEY (TrackerID) REFERENCES Trackers(Id)
);