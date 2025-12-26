-- Script to create database tables for To-To-Do Application

-- Create Tasks table
CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Date" TEXT NOT NULL,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT,
    "Completed" BOOLEAN NOT NULL DEFAULT FALSE,
    "Description" TEXT
);

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" TEXT NOT NULL
);
