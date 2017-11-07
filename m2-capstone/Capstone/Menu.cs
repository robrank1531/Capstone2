using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Capstone.Models;
using Capstone.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Interfaces;

namespace Capstone
{
    public class Menu : IMenu
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CapstoneDatabase"].ConnectionString;

        public void Runner()
        {
            bool running = true;
            while (running)
            {
                running = ParkDisplay();
            }
        }

        public bool ParkDisplay()
        {
            Console.Clear();
            ParkDAL parkDAL = new ParkDAL(connectionString);
            List<Park> parks = parkDAL.GetParks();

            Console.WriteLine("Select a Park for Further Details");
            foreach (Park p in parks)
            {
                Console.WriteLine($"{p.AlphaID} ) {p.Name}");
            }
            Console.WriteLine("Q ) quit");

            string input = Console.ReadLine();

            if (input.ToLower() == "q")
            {
                return false;
            }


            else
            {
                foreach (Park p in parks)
                {
                    if (input == p.AlphaID.ToString())
                    {
                        bool running = true;
                        while (running)
                        {
                            running = ParkInfoScreen(p);
                        }
                        return true;
                    }
                }

                Console.WriteLine("Please enter a valid option. Press any key to continue.");
                Console.ReadKey();
            }
            return true;
        }

        public bool ParkInfoScreen(Park p)
        {
            Console.Clear();
            Console.WriteLine(p.Name + " Park");
            Console.WriteLine("Location:".PadRight(20) + p.Location);
            Console.WriteLine("Established:".PadRight(20) + p.EstDate.ToShortDateString());
            Console.WriteLine("Area:".PadRight(20) + String.Format("{0:n0}", p.Area) + " sq km");
            Console.WriteLine("Annual Visitors:".PadRight(20) + String.Format("{0:n0}", p.NumVisitors));
            Console.WriteLine();
            Console.WriteLine(p.Description);
            Console.WriteLine();
            Console.WriteLine("Select A Command");
            Console.WriteLine("1) View Campgrounds");
            Console.WriteLine("2) Search for Reservation");
            Console.WriteLine("3) See All Reservations for Next 30 Days");
            Console.WriteLine("4) Return to Previous Screen");

            string input = Console.ReadLine();

            if (input == "1")
            {
                bool running = true;
                while (running)
                {
                    running = PrintCampgrounds(p);
                }
                return true;
            }
            else if (input == "2")
            {
                bool running = true;
                while (running)
                {
                    running = ParkCampgrounds(p);
                }
                return true;
            }
            else if (input == "4")
            {
                return false;
            }
            else if (input == "3")
            {
                ReservationSiteDAL allReservations = new ReservationSiteDAL(connectionString);
                bool running = true;
                while (running)
                {
                    DateTime arriveDate;
                    Console.WriteLine("What is the arrival date? mm/dd/yyyy");
                    try { arriveDate = Convert.ToDateTime(Console.ReadLine()); }//validate user entry on both dates, that arrival is before departure, and for at least one night
                    catch (Exception e) { Console.WriteLine("Please enter a valid option. Press any key to return."); Console.ReadKey(); return false; }
                    if (arriveDate > DateTime.Today)
                    {
                        int padding = 15;
                        Console.WriteLine("Campground ID".PadRight(15) + "Site Number".PadRight(15) + "Name".PadRight(25) + "Daily Fee".PadRight(padding) + "From Date".PadRight(padding) + "To Date");
                        Console.WriteLine("-".PadRight(75, '-'));
                        List<ReservationSite> listOfAvailableSites = allReservations.GetAllReservations(p.Id, arriveDate);
                        foreach(ReservationSite r in listOfAvailableSites)
                        {
                            Console.WriteLine(r.CampgrounID.ToString().PadRight(padding) + r.SiteNumber.ToString().PadRight(padding) + r.Name.PadRight(25) + "$"+ r.DailyFee.ToString("F2").PadRight(padding) + /*r.MaxOccupancy.ToString().PadRight(padding) + r.Accessible.PadRight(padding) + r.MaxRvLength.ToString().PadRight(padding) + r.Utilities.PadRight(padding) +*/ r.FromDate.ToShortDateString().PadRight(padding) + r.ToDate.ToShortDateString());
                        }
                        if(listOfAvailableSites.Count == 0) { Console.WriteLine("There are no reservations for your selected dates."); }
                        Console.WriteLine("Press any key to return."); Console.ReadKey(); return true;
                    }
                    else { Console.WriteLine("Please enter a valid option. Press any key to continue."); Console.ReadKey(); running = false; }
                    
                }

            }
            else
            {
                Console.WriteLine("Please enter a valid option. Press any key to continue.");
                Console.ReadKey();
                return true;
            }
            return true;
        }

        public bool PrintCampgrounds(Park p)
        {
            int pad = 20;

            Console.Clear();
            Console.WriteLine(p.Name + " Park Campgrounds");
            Console.WriteLine();
            Console.WriteLine("".PadRight(5) + "Name".PadRight(35) + "Open".PadRight(pad) + "Close".PadRight(pad) + "Daily Fee");
            Console.WriteLine("-".PadRight(95, '-'));
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampgrounds(p.Id);

            foreach (Campground cg in campgrounds)
            {
                Console.WriteLine("#" + cg.AlphaID.ToString().PadRight(4) + cg.Name.PadRight(35) + GetMonthString(cg.OpenFrom).PadRight(pad) + GetMonthString(cg.OpenTo).PadRight(pad) + "$" + cg.DailyFee.ToString("F2"));
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the previous menu");
            Console.ReadKey();
            return false;
        }

        public bool ParkCampgrounds(Park p)
        {
            int pad = 20;

            Console.Clear();
            Console.WriteLine(p.Name + " Park Campgrounds");
            Console.WriteLine();
            Console.WriteLine("".PadRight(5) + "Name".PadRight(35) + "Open".PadRight(pad) + "Close".PadRight(pad) + "Daily Fee");
            Console.WriteLine("-".PadRight(95, '-'));
            CampgroundDAL campgroundDAL = new CampgroundDAL(connectionString);
            List<Campground> campgrounds = campgroundDAL.GetCampgrounds(p.Id);

            foreach (Campground cg in campgrounds)
            {
                Console.WriteLine("#" + cg.AlphaID.ToString().PadRight(4) + cg.Name.PadRight(35) + GetMonthString(cg.OpenFrom).PadRight(pad) + GetMonthString(cg.OpenTo).PadRight(pad) + "$" + cg.DailyFee.ToString("F2"));
            }
            int alphaID = 0;
            Console.WriteLine("Which campground (enter 0 to cancel)?");
            try { alphaID = Convert.ToInt32(Console.ReadLine()); }
            catch(Exception e) { Console.WriteLine("Please enter valid option. Press any key to return."); Console.ReadKey(); return true; }
            bool campgroundIsInPark = false;
            foreach(Campground cg in campgrounds)
            {
                if(alphaID == cg.AlphaID)
                {
                    campgroundIsInPark = true;
                }
            }
            if (alphaID == 0) { return false; }
            else if( campgroundIsInPark == true)
            {
                int cgID = 0;
                decimal dailyFee = 0;

                foreach (Campground cg in campgrounds)
                {
                    if (cg.AlphaID == alphaID)
                    {
                        cgID = cg.Id;
                        dailyFee = cg.DailyFee;
                    }
                }

                bool running = true;
                bool reservationRunning = true;
                while (running)
                {
                    DateTime arriveDate;
                    DateTime departDate;
                    Console.WriteLine("What is the arrival date? mm/dd/yyyy");
                    try { arriveDate = Convert.ToDateTime(Console.ReadLine()); }//validate user entry on both dates, that arrival is before departure, and for at least one night
                    catch(Exception e) { Console.WriteLine("Please enter a valid option. Press any key to return.");Console.ReadKey(); return true; }
                    Console.WriteLine("What is the departure date? mm/dd/yyyy");
                    try { departDate = Convert.ToDateTime(Console.ReadLine()); }
                    catch (Exception e) { Console.WriteLine("Please enter a valid option. Press any key to return."); Console.ReadKey(); return true; }
                    if (departDate > arriveDate && (departDate - arriveDate).TotalDays > 0 && arriveDate > DateTime.Today)
                    {                     
                        
                            running = AvailableForReservation(dailyFee, cgID, arriveDate, departDate);
                            //Console.Clear();                       
                        
                    }
                    else { Console.WriteLine("Please enter a valid option. Press any key to continue."); Console.ReadKey(); running = false; }
                }
                
                return true;
            }
            else { Console.WriteLine("Please enter a valid option. Press any key to continue."); Console.ReadKey(); return true; }
        }

        public bool AvailableForReservation (decimal dailyFee, int cgID, DateTime arriveDate, DateTime departDate)
        {
            SiteDAL siteDAL = new SiteDAL(connectionString);

            List<Site> availableSites = siteDAL.GetAvailableSites(cgID, arriveDate, departDate);
            int pad = 20;
            Console.WriteLine("Results Matching Your Search Criteria");
            Console.WriteLine();
            Console.WriteLine("Site No.".PadRight(pad) + "Max Occup.".PadRight(pad) + "Accessible?".PadRight(pad) + "Max RV Length".PadRight(pad) + "Utility".PadRight(pad) + "Cost");
            Console.WriteLine("-".PadRight(120, '-'));
            if (availableSites.Count > 0)
            {
                foreach (Site a in availableSites)
                {
                    decimal cost = dailyFee * (decimal)((departDate - arriveDate).TotalDays);
                    Console.WriteLine(Convert.ToString(a.SiteNum).PadRight(pad) + Convert.ToString(a.MaxOccupancy).PadRight(pad) + a.Accessible.PadRight(pad) + a.MaxRVLength.PadRight(pad) + a.Utilities.PadRight(pad) + "$" + cost.ToString("F2"));
                }
            }

            else
            {                
                Console.WriteLine("There are no campsites available for the dates you have selected. Would you like to try different dates? Y/N");
                string input = Console.ReadLine();
                if (input.ToLower() == "y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            int siteNumSelection = 0;
            Console.WriteLine();
            Console.WriteLine("Which site should be reserved (enter 0 to cancel)? __");
            try { siteNumSelection = Convert.ToInt32(Console.ReadLine()); }
            catch(Exception e) { Console.WriteLine("Please enter valid option. Press any key to return."); Console.ReadKey(); return true; }
            bool siteExists = false;
            foreach (Site site in availableSites)
            {
                if (siteNumSelection == site.SiteNum)
                {
                    siteExists = true;
                }
            }

            if (siteNumSelection == 0) { return false; }                    
            else if(siteExists == true)
            {
                Console.WriteLine("What name should the reservation be made under? __");
                string reservationName = Console.ReadLine();
                MakeReservation(siteNumSelection, reservationName, arriveDate, departDate);
                Console.WriteLine("Press any key to continue.");
                Console.WriteLine();
                Console.ReadKey();
                return false;
            }
            else { Console.WriteLine("Please enter a valid option. Press any key to return."); Console.ReadKey(); return true; }
            
        }

        public void MakeReservation(int siteNumSelection, string reservationName, DateTime arriveDate, DateTime departDate)
        {
            ReservationDAL reservationDAL = new ReservationDAL(connectionString);
            int reservationID = reservationDAL.MakeReservation(siteNumSelection, reservationName, arriveDate, departDate);
            Console.WriteLine($"The reservation has been made and the confirmation id is {reservationID}");            
        }
        //public bool GetFollowingMonthOfReservations()

        public string GetMonthString(int month)
        {
            Dictionary<int, string> monthName = new Dictionary<int, string>() {
                {1, "January"},
                {2, "February" },
                {3, "March" },
                {4, "April" },
                {5, "May" },
                {6, "June" },
                {7, "July" },
                {8, "August" },
                {9, "September" },
                {10, "October" },
                {11, "November" },
                {12, "December" }
            };
            return monthName[month];
        }
    }

}

