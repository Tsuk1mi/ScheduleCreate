-- Создание базы данных
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ScheduleCreate')
BEGIN
    CREATE DATABASE ScheduleCreate;
END
GO

USE ScheduleCreate;
GO

-- Создание таблиц
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Teachers')
CREATE TABLE Teachers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Department NVARCHAR(100),
    LoadHours FLOAT,
    ContactInfo NVARCHAR(200)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Disciplines')
CREATE TABLE Disciplines (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(20) NOT NULL,
    TotalHours FLOAT,
    LectureHours FLOAT,
    PracticeHours FLOAT,
    LabHours FLOAT
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TeacherDisciplines')
CREATE TABLE TeacherDisciplines (
    TeacherId INT,
    DisciplineId INT,
    CONSTRAINT PK_TeacherDisciplines PRIMARY KEY (TeacherId, DisciplineId),
    CONSTRAINT FK_TeacherDisciplines_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
    CONSTRAINT FK_TeacherDisciplines_Disciplines FOREIGN KEY (DisciplineId) REFERENCES Disciplines(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Groups')
CREATE TABLE Groups (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    StudentCount INT,
    Course INT,
    Faculty NVARCHAR(100)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Auditoriums')
CREATE TABLE Auditoriums (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Number NVARCHAR(20) NOT NULL,
    Capacity INT,
    Type INT
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScheduleEntries')
CREATE TABLE ScheduleEntries (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Date DATE NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    TeacherId INT,
    DisciplineId INT,
    AuditoriumId INT,
    Type INT,
    IsStream BIT,
    Notes NVARCHAR(MAX),
    CONSTRAINT FK_ScheduleEntries_Teachers FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
    CONSTRAINT FK_ScheduleEntries_Disciplines FOREIGN KEY (DisciplineId) REFERENCES Disciplines(Id),
    CONSTRAINT FK_ScheduleEntries_Auditoriums FOREIGN KEY (AuditoriumId) REFERENCES Auditoriums(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScheduleEntryGroups')
CREATE TABLE ScheduleEntryGroups (
    ScheduleEntryId INT,
    GroupId INT,
    CONSTRAINT PK_ScheduleEntryGroups PRIMARY KEY (ScheduleEntryId, GroupId),
    CONSTRAINT FK_ScheduleEntryGroups_ScheduleEntries FOREIGN KEY (ScheduleEntryId) REFERENCES ScheduleEntries(Id),
    CONSTRAINT FK_ScheduleEntryGroups_Groups FOREIGN KEY (GroupId) REFERENCES Groups(Id)
);
GO

-- Добавление тестовых данных
IF NOT EXISTS (SELECT * FROM Teachers)
BEGIN
    INSERT INTO Teachers (FullName, Department, LoadHours) VALUES
    ('Иванов И.И.', 'Кафедра информатики', 800),
    ('Петров П.П.', 'Кафедра математики', 600),
    ('Сидорова С.С.', 'Кафедра физики', 700);
END

IF NOT EXISTS (SELECT * FROM Disciplines)
BEGIN
    INSERT INTO Disciplines (Name, Code, TotalHours, LectureHours, PracticeHours, LabHours) VALUES
    ('Программирование', 'CS101', 144, 48, 48, 48),
    ('Математический анализ', 'MA201', 180, 90, 90, 0),
    ('Физика', 'PH101', 160, 64, 48, 48);
END
GO
