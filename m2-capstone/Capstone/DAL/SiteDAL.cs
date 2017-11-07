using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAL
{
    public class SiteDAL
    {
        private const string getSites = @"SELECT DISTINCT TOP 5 * FROM campground JOIN site ON campground.campground_id = site.campground_id 
                                        WHERE campground.campground_id = @campgroundID AND site.site_id NOT IN ( SELECT site.site_id FROM campground 
                                        JOIN site ON campground.campground_id = site.campground_id LEFT JOIN reservation ON site.site_id = 
                                        reservation.site_id WHERE campground.campground_id = @campgroundID AND NOT (reservation.to_date <= @arriveDate OR 
                                        reservation.from_date >= @departDate OR reservation.from_date IS NULL));";
        private const string getAllReservations = @"SELECT * FROM campground
                                                    JOIN site ON campground.campground_id = site.campground_id
                                                    JOIN reservation ON site.site_id = reservation.site_id
                                                    WHERE from_date > @arriveDate AND to_date < DATEADD(day, 30, @arriveDate) AND park_id = @park;";
        private string connectionString;

        public SiteDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Site> GetAvailableSites(int cgID, DateTime arriveDate, DateTime departDate)
        {
            List<Site> availableSites = new List<Site>();

            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(getSites, connection);
                    cmd.Parameters.AddWithValue("@campgroundID", cgID);
                    cmd.Parameters.AddWithValue("@arriveDate", arriveDate);
                    cmd.Parameters.AddWithValue("@departDate", departDate);
                    SqlDataReader results = cmd.ExecuteReader();

                    while (results.Read())
                    {
                            Site availableSite = new Site(Convert.ToInt32(results["site_number"]), Convert.ToInt32(results["max_occupancy"]), Convert.ToInt32(results["accessible"]), Convert.ToInt32(results["max_rv_length"]), Convert.ToInt32(results["utilities"]));
                            availableSites.Add(availableSite); 
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw;
            }
            return availableSites;
        }
        
    }
    }
