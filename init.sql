CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" TEXT PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT,
    "Completed" BOOLEAN NOT NULL DEFAULT FALSE,
    "Description" TEXT
);
