using LockedIn.Data;
using LockedIn.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LockedIn.Managers
{
    public class JobManager
    {
        static string constr = DatabaseConfig.GetConStr();
        static SqlConnection conn;
        static SqlCommand cmd;

        //Method to create a Job object directly from DB
        public static Job LoadJob(int jobID)
        {
            Job job = null;

            string query = @"SELECT JobID, EmployerID, JobName, JobDescription, JobLocation, 
                StartDate, EndDate, PrisonersNeeded, Completed, DurationPerDay  
         FROM JOB 
         WHERE JobID = @JobID";

            try
            {
                using (conn = new SqlConnection(constr))
                {
                    using (cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@JobID", jobID);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                job = new Job
                                {
                                    JobID = reader.GetInt32(0),
                                    EmployerID = reader.GetInt32(1),
                                    JobName = reader.GetString(2),
                                    JobDescription = reader.GetString(3),
                                    JobLocation = reader.GetString(4),
                                    StartDate = reader.GetDateTime(5),
                                    EndDate = reader.GetDateTime(6),
                                    PrisonersNeeded = reader.GetInt32(7),
                                    Completed = reader.GetBoolean(8),
                                    DurationPerDay = reader.GetInt32(9)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving job with ID {jobID}: {ex.Message}");
                return null;
            }

            return job;
        }


        public static int AddJob(Job job)
        {
            try
            {
                using (conn = new SqlConnection(constr))
                {
                    conn.Open();
                    using (cmd = new SqlCommand("dbo.AddJob", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@employerID", job.EmployerID);
                        cmd.Parameters.AddWithValue("@jobName", job.JobName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@jobDescription", job.JobDescription ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@jobLocation", job.JobLocation ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@startDate", job.StartDate);
                        cmd.Parameters.AddWithValue("@endDate", job.EndDate);
                        cmd.Parameters.AddWithValue("@prisonersNeeded", job.PrisonersNeeded);
                        cmd.Parameters.AddWithValue("@completed", job.Completed);
                        cmd.Parameters.AddWithValue("@durationPerDay", job.DurationPerDay);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Rows affected by stored procedure: {rowsAffected}");

                        if (rowsAffected > 0)
                        {
                            // Get the newly inserted JobID using a separate query
                            using (SqlCommand selectCmd = new SqlCommand("SELECT MAX(JobID) FROM JOB WHERE EmployerID = @employerID", conn))
                            {
                                selectCmd.Parameters.AddWithValue("@employerID", job.EmployerID);
                                object result = selectCmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                {
                                    int jobId = Convert.ToInt32(result);
                                    Console.WriteLine($"Successfully added job with ID: {jobId}");
                                    return jobId;
                                }
                            }
                        }

                        Console.WriteLine($"No rows affected. RowsAffected: {rowsAffected}");
                        return -1;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}\nError Number: {ex.Number}");
                Console.WriteLine($"SQL Exception: {ex.Message}, Number: {ex.Number}, Severity: {ex.Class}");
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding job: {ex.Message}");
                Console.WriteLine($"General Exception: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return -1;
            }
        }

        public static bool UpdateJob(int jobId, int employerID, string jobName, string jobDescription, string jobLocation, DateTime startDate, DateTime endDate, decimal durationPerDay, bool completed, int prisonersNeeded)
        {
            try
            {
                using (conn = new SqlConnection(constr))
                {
                    conn.Open();
                    using (cmd = new SqlCommand("dbo.UpdateJob", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@jobID", jobId);
                        cmd.Parameters.AddWithValue("@employerID", employerID);
                        cmd.Parameters.AddWithValue("@jobName", jobName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@jobDescription", jobDescription ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@jobLocation", jobLocation ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);
                        cmd.Parameters.AddWithValue("@prisonersNeeded", prisonersNeeded);
                        cmd.Parameters.AddWithValue("@completed", completed);
                        cmd.Parameters.AddWithValue("@durationPerDay", (int)durationPerDay); // Cast decimal to int

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0; // True if a row was updated
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error updating job with ID {jobId}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating job with ID {jobId}: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteJob(int jobId)
        {
            try
            {
                using (conn = new SqlConnection(constr))
                {
                    conn.Open();
                    using (cmd = new SqlCommand("dbo.DeleteJob", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@jobID", jobId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0; // True if a row was deleted
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error deleting job with ID {jobId}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting job with ID {jobId}: {ex.Message}");
                return false;
            }
        }

        public static int CountPrisonersOnJob(Job job)
        {
            int count = 0;

            string query = "SELECT COUNT(*) FROM PRISONER_ON_JOB WHERE JobID = @JobID";

            using (SqlConnection conn = new SqlConnection(constr))
            using (cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@JobID", job.JobID);

                conn.Open();
                count = (int)cmd.ExecuteScalar();
            }

            return count;
        }


    }
}