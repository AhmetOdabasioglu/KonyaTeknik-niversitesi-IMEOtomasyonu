using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IMEAutomationDBOperations.Services
{
    public class DatabaseService
    {
        private readonly IRepository _repository;

        public DatabaseService(IRepository repository)
        {
            _repository = repository;
        }

        public void CreateDatabase()
        {
            string createDbQuery = @"
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InternshipDB1') 
                BEGIN
                    CREATE DATABASE InternshipDB1;
                END";
            _repository.ExecuteQuery(createDbQuery);
        }

        public void CreateTables()
        {
            string createTablesQuery = @"
            -- Roles Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
            BEGIN
                CREATE TABLE Roles (
                    RoleID INT IDENTITY(1,1) PRIMARY KEY,
                    RoleName NVARCHAR(50) UNIQUE NOT NULL
                );
            END

            -- Rollerin eklenmesi
            IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName IN ('Admin', 'Student', 'Academician', 'InternshipSupervisor', 'Guest'))
            BEGIN
                INSERT INTO Roles (RoleName) VALUES 
                ('Admin'),
                ('Student'),
                ('Academician'),
                ('InternshipSupervisor'),
                ('Guest');
            END

            -- Users Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    UserID INT IDENTITY(1,1) PRIMARY KEY,
                    UserName NVARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(255) NOT NULL,  
                    RoleID INT NOT NULL,
                    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE CASCADE
                );
            END

            -- Company Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Company')
            BEGIN
                CREATE TABLE Company (
                    CompanyID INT IDENTITY(1,1) PRIMARY KEY,
                    CompanyName NVARCHAR(255) NOT NULL,
                    TaxNumber NVARCHAR(20) UNIQUE NOT NULL,
                    EmployeeCount INT,
                    Departments NVARCHAR(255),
                    Address NVARCHAR(255),
                    PhoneNumber NVARCHAR(15),
                    Website NVARCHAR(255),
                    Industry NVARCHAR(100),
                    Email NVARCHAR(100),
                    ManagerFirstName NVARCHAR(50),
                    ManagerLastName NVARCHAR(50),
                    ManagerPhone NVARCHAR(15),
                    ManagerEmail NVARCHAR(100),
                    BankName NVARCHAR(100),
                    BankBranch NVARCHAR(100),
                    BankIbanNo NVARCHAR(30)
                );
            END

            -- Students Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
            BEGIN
                CREATE TABLE Students (
                    StudentID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    AcademicYear INT NOT NULL,
                    NationalID CHAR(11) UNIQUE NOT NULL,
                    BirthDate DATE NOT NULL,
                    SchoolNumber NVARCHAR(20) UNIQUE NOT NULL,
                    Department NVARCHAR(100) NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    Email NVARCHAR(100),
                    Address NVARCHAR(255),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Academicians Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Academicians')
            BEGIN
                CREATE TABLE Academicians (
                    AcademicianID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    Department NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- InternshipSupervisors Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipSupervisors')
            BEGIN
                CREATE TABLE InternshipSupervisors (
                    SupervisorID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID) ON DELETE CASCADE,
					StudentID INT FOREIGN KEY REFERENCES Students(StudentID)  ON DELETE NO ACTION,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    ContactPhone NVARCHAR(15),
                    Expertise NVARCHAR(100),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Admins Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Admins')
            BEGIN
                CREATE TABLE Admins (
                    AdminID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    FullName NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    PhoneNumber NVARCHAR(15),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- Guests Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
            BEGIN
                CREATE TABLE Guests (
                    GuestID INT IDENTITY(1,1) PRIMARY KEY,
                    UserID INT UNIQUE NOT NULL,
                    Name NVARCHAR(100) NOT NULL,
                    VisitReason NVARCHAR(255),
                    ContactEmail NVARCHAR(100),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
                );
            END

            -- InternshipDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InternshipDetails')
            BEGIN
                CREATE TABLE InternshipDetails (
                    InternshipID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT FOREIGN KEY REFERENCES Students(StudentID)  ON DELETE NO ACTION,
                    CompanyID INT FOREIGN KEY REFERENCES Company(CompanyID)  ON DELETE NO ACTION,
                    SupervisorID INT FOREIGN KEY REFERENCES InternshipSupervisors(SupervisorID) ON DELETE SET NULL,
                    InternshipTitle NVARCHAR(100) NOT NULL,
                    StartDate DATE NOT NULL,
                    EndDate DATE NOT NULL,
                    TotalTrainingDays INT NOT NULL CHECK (TotalTrainingDays > 0),
                    LeaveDays INT DEFAULT 0 CHECK (LeaveDays >= 0),
                    WorkDays NVARCHAR(100),
                    PaidAmount DECIMAL(10,2) DEFAULT 0 CHECK (PaidAmount >= 0),
                    CreatedAt DATETIME DEFAULT GETDATE(),
                    UpdatedAt DATETIME DEFAULT GETDATE()
                );
            END

            -- LeaveDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LeaveDetails')
            BEGIN
                CREATE TABLE LeaveDetails (
                    LeaveID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    LeaveStart DATE NOT NULL,
                    LeaveEnd DATE NOT NULL,
                    LeaveReason NVARCHAR(100) NOT NULL,
                    ReasonDetail NVARCHAR(MAX) NULL,
                    AddressDuringLeave NVARCHAR(255) NOT NULL,
                    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE
                );
            END

            -- GradeDetails Tablosu
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GradeDetails')
            BEGIN
                CREATE TABLE GradeDetails (
                    GradeID INT IDENTITY(1,1) PRIMARY KEY,
                    StudentID INT NOT NULL,
                    SupervisorID INT NOT NULL,
                    InternshipSupervisorEvaluation DECIMAL(5,2) NOT NULL,
                    InternshipInstructorEvaluation DECIMAL(5,2) NOT NULL,
                    WeeklyVideoPresentationScore DECIMAL(5,2) NOT NULL,
                    DepartmentInternshipCommissionScore DECIMAL(5,2) NOT NULL,
                    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
                    FOREIGN KEY (SupervisorID) REFERENCES InternshipSupervisors(SupervisorID) ON DELETE NO ACTION
                );
            END
            ";

            _repository.ExecuteQuery(createTablesQuery);
        }

        public List<User> GetUsersData()
        {
            string query = "SELECT UserID, UserName, PasswordHash, RoleID FROM Users";
            var users = new List<User>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                UserID = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                PasswordHash = reader.GetString(2),
                                RoleID = reader.GetInt32(3)
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public List<Student> GetStudentsData()
        {
            string query = "SELECT UserID, FirstName, LastName, AcademicYear, NationalID, BirthDate, SchoolNumber, Department, PhoneNumber, Email, Address FROM Students";
            var students = new List<Student>();

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var student = new Student
                            {
                                UserID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                AcademicYear = reader.GetInt32(3),
                                NationalID = reader.GetString(4),
                                BirthDate = reader.GetDateTime(5),
                                SchoolNumber = reader.GetString(6),
                                Department = reader.GetString(7),
                                PhoneNumber = reader.GetString(8),
                                Email = reader.GetString(9),
                                Address = reader.GetString(10),
                                Password = "123456"
                            };
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }

        public (InternshipSupervisor?, Company?, InternshipDetails?) GetSupervisorCompanyAndInternshipDetailsByStudentEmail(string studentEmail)
        {
            string query = @"
            SELECT 
                s.SupervisorID, s.FirstName, s.LastName, ISNULL(s.ContactPhone, '') AS ContactPhone, ISNULL(s.Expertise, '') AS Expertise,
                c.CompanyID, c.CompanyName, c.TaxNumber, c.Address, c.PhoneNumber, c.Email,
                ISNULL(c.Departments, '') AS Departments, ISNULL(c.Website, '') AS Website, ISNULL(c.Industry, '') AS Industry,
                ISNULL(c.ManagerFirstName, '') AS ManagerFirstName, ISNULL(c.ManagerLastName, '') AS ManagerLastName,
                ISNULL(c.ManagerPhone, '') AS ManagerPhone, ISNULL(c.ManagerEmail, '') AS ManagerEmail,
                ISNULL(c.BankName, '') AS BankName, ISNULL(c.BankBranch, '') AS BankBranch, ISNULL(c.BankIbanNo, '') AS BankIbanNo,
                i.InternshipID, i.StudentID, i.CompanyID, i.SupervisorID, i.InternshipTitle, 
                i.StartDate, i.EndDate, i.TotalTrainingDays, i.LeaveDays, i.WorkDays, 
                i.PaidAmount, i.CreatedAt, i.UpdatedAt
            FROM Students st
            INNER JOIN InternshipSupervisors s ON st.StudentID = s.StudentID
            INNER JOIN Company c ON s.CompanyID = c.CompanyID
            INNER JOIN InternshipDetails i ON st.StudentID = i.StudentID
            WHERE st.Email = @StudentEmail";

            using (var connection = new SqlConnection(_repository.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentEmail", studentEmail);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var supervisor = new InternshipSupervisor
                            {
                                SupervisorID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                ContactPhone = reader.GetString(3),
                                Expertise = reader.GetString(4)
                            };

                            var company = new Company
                            {
                                CompanyID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                CompanyName = reader.GetString(6),
                                TaxNumber = reader.GetString(7),
                                Address = reader.GetString(8),
                                PhoneNumber = reader.GetString(9),
                                Email = reader.GetString(10),
                                Departments = reader.GetString(11),
                                Website = reader.GetString(12),
                                Industry = reader.GetString(13),
                                ManagerFirstName = reader.GetString(14),
                                ManagerLastName = reader.GetString(15),
                                ManagerPhone = reader.GetString(16),
                                ManagerEmail = reader.GetString(17),
                                BankName = reader.GetString(18),
                                BankBranch = reader.GetString(19),
                                BankIbanNo = reader.GetString(20)
                            };


                            var internshipDetails = new InternshipDetails
                            {
                                InternshipID = reader.GetInt32(21),
                                StudentID = reader.GetInt32(22),
                                CompanyID = reader.GetInt32(23),
                                SupervisorID = reader.IsDBNull(24) ? (int?)null : reader.GetInt32(24),
                                InternshipTitle = reader.GetString(25),
                                StartDate = reader.GetDateTime(26),
                                EndDate = reader.GetDateTime(27),
                                TotalTrainingDays = reader.GetInt32(28),
                                LeaveDays = reader.GetInt32(29),
                                WorkDays = reader.IsDBNull(30) ? string.Empty : reader.GetString(30),
                                PaidAmount = reader.GetDecimal(31),
                                CreatedAt = reader.GetDateTime(32),
                                UpdatedAt = reader.GetDateTime(33)
                            };

                            return (supervisor, company, internshipDetails);
                        }
                    }
                }
            }

            return (null, null, null);

        }
    }
}
