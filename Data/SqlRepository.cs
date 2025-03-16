using System;
using Microsoft.Data.SqlClient;

namespace IMEAutomationDBOperations.Data
{
    public class SqlRepository : IRepository
    {
        public string ConnectionString { get; }

        public SqlRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"UserID: {reader["UserID"]}, UserName: {reader["UserName"]}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata oluştu: " + ex.Message);
                }
            }
        }

        public void GetUsersData()
        {
            string query = "SELECT * FROM Users";
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"UserID: {reader["UserID"]}, UserName: {reader["UserName"]}");
                    }
                }
            }
        }

        public void GetStudentsData()
        {
            string query = "SELECT * FROM Students";
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"UserID: {reader["UserID"]}, FirstName: {reader["FirstName"]} " +
                        $"LastName: {reader["LastName"]} AcademicYear: {reader["AcademicYear"]} NationalID: {reader["NationalID"]} " +
                        $"BirthDate: {reader["BirthDate"]} SchoolNumber: {reader["SchoolNumber"]} Department: {reader["Department"]} " +
                        $"PhoneNumber: {reader["PhoneNumber"]} Email: {reader["Email"]} Address: {reader["Address"]}");
                    }
                }
            }
        }
    }
}
