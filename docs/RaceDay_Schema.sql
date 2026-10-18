
-- RaceDay Database Schema and DEMO Data

-- Create the database
CREATE DATABASE RaceDayDB;


USE RaceDayDB;


-- 1. CREATE TABLES

-- Users table (base for both roles)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);


-- Organisers table (one to one with Users)
CREATE TABLE Organisers (
    OrganiserID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    CompanyName NVARCHAR(100),
    ContactNumber NVARCHAR(20)
);


-- Participants table (one to one with Users)
CREATE TABLE Participants (
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
    DateOfBirth DATE,
    EmergencyContact NVARCHAR(100)
);


-- Events table (created by an Organiser)
CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL FOREIGN KEY REFERENCES Organisers(OrganiserID) ON DELETE CASCADE,
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    MaxParticipants INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);


-- Categories table (each event has multiple categories)
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL FOREIGN KEY REFERENCES Events(EventID) ON DELETE CASCADE,
    CategoryName NVARCHAR(50) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,  -- in kilometres
    AgeGroup NVARCHAR(20)
);


CREATE TABLE Enrollments (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL FOREIGN KEY REFERENCES Participants(ParticipantID),  -- NO CASCADE
    CategoryID INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryID) ON DELETE CASCADE,
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Registered' CHECK (Status IN ('Registered', 'Cancelled', 'Completed'))
);


-- Results table (one result per enrollment)
CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Enrollments(EnrollmentID) ON DELETE CASCADE,
    FinishTime TIME NOT NULL,      -- e.g, '01:30:00'
    [Position] INT NOT NULL
);



-- Insert Users (passwords are hashed placeholders; in production you'd use proper hashing)
INSERT INTO Users (Email, PasswordHash, FullName, Role) VALUES
('alice.organiser@email.com', 'hashed_password_1', 'Alice Organiser', 'Organiser'),
('bob.organiser@email.com', 'hashed_password_2', 'Bob Smith', 'Organiser'),
('charlie.runner@email.com', 'hashed_password_3', 'Charlie Runner', 'Participant'),
('diana.sprinter@email.com', 'hashed_password_4', 'Diana Sprinter', 'Participant'),
('eve.fast@email.com', 'hashed_password_5', 'Eve Fast', 'Participant');


-- Insert Organisers
INSERT INTO Organisers (UserID, CompanyName, ContactNumber) VALUES
(1, 'RunFast Events', '011-123-4567'),
(2, 'Endurance Sports', '021-987-6543');


-- Insert Participants
INSERT INTO Participants (UserID, DateOfBirth, EmergencyContact) VALUES
(3, '1995-03-15', 'John Runner - 082-555-1234'),
(4, '1988-11-20', 'Mike Sprinter - 083-555-5678'),
(5, '2000-07-01', 'Sarah Fast - 084-555-9012');


-- Insert Events
INSERT INTO Events (OrganiserID, EventName, EventDate, Location, Description, MaxParticipants) VALUES
(1, 'Cape Town Marathon', '2026-10-15 07:00:00', 'Cape Town', 'A scenic marathon along the coast.', 5000),
(1, 'Joburg 10km Run', '2026-11-05 08:00:00', 'Johannesburg', 'A fast 10km race in the city.', 2000),
(2, 'Durban Trail Challenge', '2026-12-01 06:30:00', 'Durban', 'An off-road trail race with beautiful views.', 300);


-- Insert Categories for each event
INSERT INTO Categories (EventID, CategoryName, Distance, AgeGroup) VALUES
(1, 'Full Marathon', 42.20, '18-34'),
(1, 'Full Marathon', 42.20, '35-49'),
(1, 'Half Marathon', 21.10, '18-34'),
(2, '10km Run', 10.00, 'Open'),
(2, '5km Fun Run', 5.00, 'All Ages'),
(3, '15km Trail', 15.00, 'Open'),
(3, '25km Trail', 25.00, '18-45');


-- Insert Enrollments
INSERT INTO Enrollments (ParticipantID, CategoryID, Status) VALUES
(1, 1, 'Registered'),    -- Charlie in Full Marathon 18-34
(1, 4, 'Completed'),     -- Charlie in 10km Run (finished)
(2, 6, 'Registered'),    -- Diana in 15km Trail
(3, 2, 'Registered');    -- Eve in Full Marathon 35-49


-- Insert Results (for completed enrollments)
INSERT INTO Results (EnrollmentID, FinishTime, [Position]) VALUES
(2, '00:48:32', 15);    -- Charlie's result for the 10km



-- 3. VERIFICATION QUERIES TO RUN TEST 


SELECT * FROM Users;
SELECT * FROM Organisers;
SELECT * FROM Participants;
SELECT * FROM Events;
SELECT * FROM Categories;
SELECT * FROM Enrollments;
SELECT * FROM Results;
