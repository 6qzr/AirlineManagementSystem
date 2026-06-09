using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace AirlineManagementSystem
{
    // Define all enums (based on attributes with multible fixed values)
    enum AircraftStatus { Active, UnderMaintenance, Retired }
    enum FlightStatus { Scheduled, Boarding, Departed, Arrived, Delayed, Cancelled }
    enum LoyaltyTier { Bronze, Silver, Gold, Platinum }
    enum CrewMemberRole { Pilot, CoPilot, CabinCrew, GroundStaff }
    enum TicketSeatClass { Business, Economy }
    enum TicketStatus { Confirmed, Cancelled, CheckedIn, Boarded }
    enum BaggageType { Cabin, Hold, Oversized }
    enum BaggageStatus { CheckedIn, Loaded, Lost, Delivered }
    enum PromotionApplicableClass { Economy, Business, Both }

    // Define all structs (based on the entities in the ERD)
    struct Flight
    {
        public string FlightNumber;
        public string OriginAirportCode;
        public string DestinationAirportCode;
        public string AirlineICAO;
        public string AircraftRegNumber;
        public DateTime ScheduledDeparture;
        public DateTime ScheduledArrival;
        public DateTime? ActualDeparture; // Empty by default
        public DateTime? ActualArrival; // Empty by default
        public FlightStatus Status;
        public int AvailableBusinessSeats;
        public int AvailableEconomySeats;
        public decimal BasePrice;
    }
    struct Airport
    {
        public string IATACode;
        public string FullName;
        public string City;
        public string Country;
        public float TimeZoneOffset;
    }

    struct Airline
    {
        public string ICAOCode;
        public string Name;
        public string RegistrationCountry;
        public string ContactInfo;
    }

    struct Aircraft
    {
        public string RegistrationNumber;
        public string AirlineICAO;
        public string Model;
        public string Manufacturer;
        public int TotalSeats;
        public int BusinessSeats;
        public int EconomySeats;
        public int ManufacturingYear;
        public AircraftStatus Status;
    }

    struct Passenger
    {
        public string PassengerID;
        public string FullName;
        public DateTime DateOfBirth;
        public string Nationality;
        public string PassportNumber;
        public string Email;
        public string Phone;
        public DateTime RegistrationDate;
        public int LoyaltyPoints;
        public LoyaltyTier TierStatus;
        public string Password;
        public int FailedLoginAttempts;
        public DateTime? LockoutUntil;
        public DateTime? LastLoginDate;
    }

    struct CrewMember
    {
        public string EmployeeID;
        public string FullName;
        public CrewMemberRole Role;
        public string Nationality;
        public string LicenseNumber;
        public string AirlineICAO;
        public int YearsOfExperience;
        public bool IsAvailable;
    }

    struct FlightCrew
    {
        public string FlightNumber;
        public string EmployeeID;
        public DateTime AssignedDate;
    }

    struct Ticket
    {
        public string TicketID;
        public string PassengerID;
        public string FlightNumber;
        public TicketSeatClass SeatClass;
        public string SeatNumber;
        public DateTime BookingDate;
        public TicketStatus Status;
        public decimal FinalPrice;
        public int LoyaltyPointsEarned;
        public string PromoCode;
    }

    struct Baggage
    {
        public string BaggageID;
        public string TicketID;
        public decimal WeightKg;
        public BaggageType Type;
        public BaggageStatus Status;
    }

    struct Promotion
    {
        public string PromoCode;
        public decimal DiscountPercentage;
        public DateTime StartDate;
        public DateTime EndDate;
        public int MaxUses;
        public int CurrentUseCount;
        public PromotionApplicableClass ApplicableClass;
        public bool IsActive;
    }

    struct Admin
    {
        public string AdminID;
        public string FullName;
        public string Email;
        public string Password;
        public int FailedLoginAttempts;
        public DateTime? LockoutUntil;
        public DateTime? LastLoginDate;
    }

    struct LoyaltyLog
    {
        public string LogID;
        public string PassengerID;
        public string TicketID;
        public int PointsChanged;
        public string Reason;
        public DateTime TransactionDate;
    }

    struct SystemLog
    {
        public string LogID;
        public DateTime Timestamp;
        public string UserID;
        public string UserRole;
        public string ActionType;
        public string EntityAffected;
        public string Details;
    }

    struct ErrorLog
    {
        public string LogID;
        public DateTime Timestamp;
        public string ErrorMessage;
        public string StackTrace;
    }

    // File Paths
    static class Constants
    {
        private static string BaseDir = Path.GetFullPath(Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data"));

        public static string AirportsFile = Path.Combine(BaseDir, "airports.csv");
        public static string AirlinesFile = Path.Combine(BaseDir, "airlines.csv");
        public static string AircraftsFile = Path.Combine(BaseDir, "aircrafts.csv");
        public static string FlightsFile = Path.Combine(BaseDir, "flights.csv");
        public static string PassengersFile = Path.Combine(BaseDir, "passengers.csv");
        public static string CrewMembersFile = Path.Combine(BaseDir, "crew_members.csv");
        public static string FlightCrewFile = Path.Combine(BaseDir, "flight_crew.csv");
        public static string TicketsFile = Path.Combine(BaseDir, "tickets.csv");
        public static string BaggagesFile = Path.Combine(BaseDir, "baggages.csv");
        public static string PromotionsFile = Path.Combine(BaseDir, "promotions.csv");
        public static string AdminsFile = Path.Combine(BaseDir, "admins.csv");
        public static string LoyaltyLogFile = Path.Combine(BaseDir, "loyalty_log.csv");
        public static string SystemLogFile = Path.Combine(BaseDir, "system_log.csv");
        public static string ErrorLogFile = Path.Combine(BaseDir, "error_log.csv");

        public static string ReportsFolder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports"));

        public const int MaxFailedLoginAttempts = 3;
        public const int LockoutMinutes = 15;
        public const int MinPasswordLength = 8;

        public const int SilverThreshold = 1000;
        public const int GoldThreshold = 5000;
        public const int PlatinumThreshold = 10000;

        public const decimal MaxCabinWeight = 7;
        public const decimal MaxHoldWeight = 23;
        public const decimal MaxOversizedWeight = 32;

        public const int CheckInWindowOpen = 3;   // hours before departure
        public const int CheckInWindowClose = 45; // minutes before departure

        public const decimal PerKmRate = 0.10m;
        public const decimal BusinessMultiplier = 2.0m;
        public const decimal PeakSeasonSurcharge = 0.15m;
        public const int AdvanceBookingDays = 30;
        public const decimal AdvanceBookingDiscount = 0.10m;
        public const decimal TaxRate = 0.05m;
        public static readonly int[] PeakMonths = { 6, 7, 8, 12 };

        public const int PointsPerDollar = 1; // 1 point per $1 spent

        public const int PageSize = 10;
    }

    static class DataStore
    {
        // Dictionaries: entities with primary keys
        public static Dictionary<string, Airport> Airports = new Dictionary<string, Airport>();
        public static Dictionary<string, Airline> Airlines = new Dictionary<string, Airline>();
        public static Dictionary<string, Aircraft> Aircrafts = new Dictionary<string, Aircraft>();
        public static Dictionary<string, Flight> Flights = new Dictionary<string, Flight>();
        public static Dictionary<string, Passenger> Passengers = new Dictionary<string, Passenger>();
        public static Dictionary<string, CrewMember> CrewMembers = new Dictionary<string, CrewMember>();
        public static Dictionary<string, Ticket> Tickets = new Dictionary<string, Ticket>();
        public static Dictionary<string, Promotion> Promotions = new Dictionary<string, Promotion>();
        public static Dictionary<string, Admin> Admins = new Dictionary<string, Admin>();

        // Lists
        public static List<FlightCrew> FlightCrew = new List<FlightCrew>();
        public static List<Baggage> Baggages = new List<Baggage>();
        public static List<LoyaltyLog> LoyaltyLogs = new List<LoyaltyLog>();
        public static List<SystemLog> SystemLogs = new List<SystemLog>();
        public static List<ErrorLog> ErrorLogs = new List<ErrorLog>();
    }

    /* 
     * ============== Read & Save CSVs ==============
     */
    static class CsvHelper
    {
        /* 
        * ============== Read CSVs ==============
        */
        //Reads airports.csv
        public static void LoadAirports()
        {
            using (StreamReader sr = new StreamReader(Constants.AirportsFile))
            {
                string line;
                sr.ReadLine(); // Skip Header
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Airport newAirport = new Airport();
                    newAirport.IATACode = record[0];
                    newAirport.FullName = record[1];
                    newAirport.City = record[2];
                    newAirport.Country = record[3];
                    newAirport.TimeZoneOffset = float.Parse(record[4]);

                    //Store Flight
                    DataStore.Airports[newAirport.IATACode] = newAirport;
                }
            }
        }

        //Reads airlines.csv
        public static void LoadAirlines()
        {
            using (StreamReader sr = new StreamReader(Constants.AirlinesFile))
            {
                string line;
                sr.ReadLine(); // Skip Header
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Airline newAirline = new Airline();
                    newAirline.ICAOCode = record[0];
                    newAirline.Name = record[1];
                    newAirline.RegistrationCountry = record[2];
                    newAirline.ContactInfo = record[3];

                    //Store Flight
                    DataStore.Airlines[newAirline.ICAOCode] = newAirline;
                }
            }
        }

        //Reads aircrafts.csv
        public static void LoadAircrafts()
        {
            using (StreamReader sr = new StreamReader(Constants.AircraftsFile))
            {
                string line;
                sr.ReadLine(); // Skip Header
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Aircraft newAircraft = new Aircraft();
                    newAircraft.RegistrationNumber = record[0];
                    newAircraft.AirlineICAO = record[1];
                    newAircraft.Model = record[2];
                    newAircraft.Manufacturer = record[3];
                    newAircraft.TotalSeats = Convert.ToInt32(record[4]);
                    newAircraft.BusinessSeats = Convert.ToInt32(record[5]);
                    newAircraft.EconomySeats = Convert.ToInt32(record[6]);
                    newAircraft.ManufacturingYear = Convert.ToInt32(record[7]);
                    newAircraft.Status = Enum.Parse<AircraftStatus>(record[8]);

                    //Store Flight
                    DataStore.Aircrafts[newAircraft.RegistrationNumber] = newAircraft;
                }
            }
        }

        //Reads flights.csv
        public static void LoadFlights()
        {
            using (StreamReader sr = new StreamReader(Constants.FlightsFile))
            {
                string line;
                sr.ReadLine(); // Skip Header
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Flight newFlight = new Flight();
                    newFlight.FlightNumber = record[0];
                    newFlight.OriginAirportCode = record[1];
                    newFlight.DestinationAirportCode = record[2];
                    newFlight.AirlineICAO = record[3];
                    newFlight.AircraftRegNumber = record[4];
                    newFlight.ScheduledDeparture = Convert.ToDateTime(record[5]);
                    newFlight.ScheduledArrival = Convert.ToDateTime(record[6]);
                    newFlight.ActualDeparture = string.IsNullOrEmpty(record[7]) ? null : Convert.ToDateTime(record[7]);
                    newFlight.ActualArrival = string.IsNullOrEmpty(record[8]) ? null : Convert.ToDateTime(record[8]);
                    newFlight.Status = Enum.Parse<FlightStatus>(record[9]);
                    newFlight.AvailableBusinessSeats = Convert.ToInt32(record[10]);
                    newFlight.AvailableEconomySeats = Convert.ToInt32(record[11]);
                    newFlight.BasePrice = decimal.Parse((record[12]));

                    //Store Flight
                    DataStore.Flights[newFlight.FlightNumber] = newFlight;
                }
            }
        }

        //Reads passengers.csv
        public static void LoadPassengers()
        {
            using (StreamReader sr = new StreamReader(Constants.PassengersFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Passenger newPassenger = new Passenger();
                    newPassenger.PassengerID = record[0];
                    newPassenger.FullName = record[1];
                    newPassenger.DateOfBirth = Convert.ToDateTime(record[2]);
                    newPassenger.Nationality = record[3];
                    newPassenger.PassportNumber = record[4];
                    newPassenger.Email = record[5];
                    newPassenger.Phone = record[6];
                    newPassenger.RegistrationDate = Convert.ToDateTime(record[7]);
                    newPassenger.LoyaltyPoints = Convert.ToInt32(record[8]);
                    newPassenger.TierStatus = Enum.Parse<LoyaltyTier>(record[9]);
                    newPassenger.Password = record[10];
                    newPassenger.FailedLoginAttempts = Convert.ToInt32(record[11]);
                    newPassenger.LockoutUntil = string.IsNullOrEmpty(record[12]) ? null : Convert.ToDateTime(record[12]);
                    newPassenger.LastLoginDate = string.IsNullOrEmpty(record[13]) ? null : Convert.ToDateTime(record[13]);

                    DataStore.Passengers[newPassenger.PassengerID] = newPassenger;
                }
            }
        }

        //Reads crew_members.csv
        public static void LoadCrewMembers()
        {
            using (StreamReader sr = new StreamReader(Constants.CrewMembersFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    CrewMember newCrewMember = new CrewMember();
                    newCrewMember.EmployeeID = record[0];
                    newCrewMember.FullName = record[1];
                    newCrewMember.Role = Enum.Parse<CrewMemberRole>(record[2]);
                    newCrewMember.Nationality = record[3];
                    newCrewMember.LicenseNumber = record[4];
                    newCrewMember.AirlineICAO = record[5];
                    newCrewMember.YearsOfExperience = Convert.ToInt32(record[6]);
                    newCrewMember.IsAvailable = record[7] == "Available";

                    DataStore.CrewMembers[newCrewMember.EmployeeID] = newCrewMember;
                }
            }
        }

        //Reads flight_crew.csv
        public static void LoadFlightCrew()
        {
            using (StreamReader sr = new StreamReader(Constants.FlightCrewFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    FlightCrew newFlightCrew = new FlightCrew();
                    newFlightCrew.FlightNumber = record[0];
                    newFlightCrew.EmployeeID = record[1];
                    newFlightCrew.AssignedDate = Convert.ToDateTime(record[2]);

                    DataStore.FlightCrew.Add(newFlightCrew);
                }
            }
        }

        //Reads tickets.csv
        public static void LoadTickets()
        {
            using (StreamReader sr = new StreamReader(Constants.TicketsFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Ticket newTicket = new Ticket();
                    newTicket.TicketID = record[0];
                    newTicket.PassengerID = record[1];
                    newTicket.FlightNumber = record[2];
                    newTicket.SeatClass = Enum.Parse<TicketSeatClass>(record[3]);
                    newTicket.SeatNumber = record[4];
                    newTicket.BookingDate = Convert.ToDateTime(record[5]);
                    newTicket.Status = Enum.Parse<TicketStatus>(record[6]);
                    newTicket.FinalPrice = decimal.Parse(record[7]);
                    newTicket.LoyaltyPointsEarned = Convert.ToInt32(record[8]);
                    newTicket.PromoCode = record[9];

                    DataStore.Tickets[newTicket.TicketID] = newTicket;
                }
            }
        }

        //Reads baggages.csv
        public static void LoadBaggages()
        {
            using (StreamReader sr = new StreamReader(Constants.BaggagesFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Baggage newBaggage = new Baggage();
                    newBaggage.BaggageID = record[0];
                    newBaggage.TicketID = record[1];
                    newBaggage.WeightKg = decimal.Parse(record[2]);
                    newBaggage.Type = Enum.Parse<BaggageType>(record[3]);
                    newBaggage.Status = Enum.Parse<BaggageStatus>(record[4]);

                    DataStore.Baggages.Add(newBaggage);
                }
            }
        }

        //Reads promotions.csv
        public static void LoadPromotions()
        {
            using (StreamReader sr = new StreamReader(Constants.PromotionsFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Promotion newPromotion = new Promotion();
                    newPromotion.PromoCode = record[0];
                    newPromotion.DiscountPercentage = decimal.Parse(record[1]);
                    newPromotion.StartDate = Convert.ToDateTime(record[2]);
                    newPromotion.EndDate = Convert.ToDateTime(record[3]);
                    newPromotion.MaxUses = Convert.ToInt32(record[4]);
                    newPromotion.CurrentUseCount = Convert.ToInt32(record[5]);
                    newPromotion.ApplicableClass = Enum.Parse<PromotionApplicableClass>(record[6]);
                    newPromotion.IsActive = bool.Parse(record[7]);

                    DataStore.Promotions[newPromotion.PromoCode] = newPromotion;
                }
            }
        }

        //Reads admins.csv
        public static void LoadAdmins()
        {
            using (StreamReader sr = new StreamReader(Constants.AdminsFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    Admin newAdmin = new Admin();
                    newAdmin.AdminID = record[0];
                    newAdmin.FullName = record[1];
                    newAdmin.Email = record[2];
                    newAdmin.Password = record[3];
                    newAdmin.FailedLoginAttempts = Convert.ToInt32(record[4]);
                    newAdmin.LockoutUntil = string.IsNullOrEmpty(record[5]) ? null : Convert.ToDateTime(record[5]);
                    newAdmin.LastLoginDate = string.IsNullOrEmpty(record[6]) ? null : Convert.ToDateTime(record[6]);

                    DataStore.Admins[newAdmin.AdminID] = newAdmin;
                }
            }
        }

        //Reads loyalty_log.csv
        public static void LoadLoyaltyLogs()
        {
            using (StreamReader sr = new StreamReader(Constants.LoyaltyLogFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    LoyaltyLog newLog = new LoyaltyLog();
                    newLog.LogID = record[0];
                    newLog.PassengerID = record[1];
                    newLog.TicketID = record[2];
                    newLog.PointsChanged = Convert.ToInt32(record[3]);
                    newLog.Reason = record[4];
                    newLog.TransactionDate = Convert.ToDateTime(record[5]);

                    DataStore.LoyaltyLogs.Add(newLog);
                }
            }
        }

        //Reads system_log.csv
        public static void LoadSystemLogs()
        {
            using (StreamReader sr = new StreamReader(Constants.SystemLogFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    SystemLog newLog = new SystemLog();
                    newLog.LogID = record[0];
                    newLog.Timestamp = Convert.ToDateTime(record[1]);
                    newLog.UserID = record[2];
                    newLog.UserRole = record[3];
                    newLog.ActionType = record[4];
                    newLog.EntityAffected = record[5];
                    newLog.Details = record[6];

                    DataStore.SystemLogs.Add(newLog);
                }
            }
        }

        //Reads error_log.csv
        public static void LoadErrorLogs()
        {
            using (StreamReader sr = new StreamReader(Constants.ErrorLogFile))
            {
                sr.ReadLine(); // Skip Header
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    List<string> record = new List<string>(line.Split(','));
                    ErrorLog newLog = new ErrorLog();
                    newLog.LogID = record[0];
                    newLog.Timestamp = Convert.ToDateTime(record[1]);
                    newLog.ErrorMessage = record[2];
                    newLog.StackTrace = record[3];

                    DataStore.ErrorLogs.Add(newLog);
                }
            }
        }

        /* 
        * ============== Save CSVs ==============
        */
        // Save Airports
        public static void SaveAirports()
        {
            string tempFile = Constants.AirportsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                // Write Header
                sw.WriteLine("IATACode,FullName,City,Country,TimeZoneOffset");

                // Loop through DataStore and write each record
                foreach (var x in DataStore.Airports)
                {
                    Airport f = x.Value;
                    sw.WriteLine($"{f.IATACode}," +
                                 $"{f.FullName}," +
                                 $"{f.City}," +
                                 $"{f.Country}," +
                                 $"{f.TimeZoneOffset}");
                }
            }

            File.Replace(tempFile, Constants.AirportsFile, null);

        }

        // Save Airlines
        public static void SaveAirlines()
        {
            string tempFile = Constants.AirlinesFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("ICAOCode,Name,RegistrationCountry,ContactInfo");

                foreach (var x in DataStore.Airlines)
                {
                    Airline a = x.Value;
                    sw.WriteLine($"{a.ICAOCode}," +
                                 $"{a.Name}," +
                                 $"{a.RegistrationCountry}," +
                                 $"{a.ContactInfo}");
                }
            }

            File.Replace(tempFile, Constants.AirlinesFile, null);
        }

        // Save Aircrafts
        public static void SaveAircrafts()
        {
            string tempFile = Constants.AircraftsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("RegistrationNumber,AirlineICAO,Model,Manufacturer,TotalSeats,BusinessSeats,EconomySeats,ManufacturingYear,Status");

                foreach (var x in DataStore.Aircrafts)
                {
                    Aircraft a = x.Value;
                    sw.WriteLine($"{a.RegistrationNumber}," +
                                 $"{a.AirlineICAO}," +
                                 $"{a.Model}," +
                                 $"{a.Manufacturer}," +
                                 $"{a.TotalSeats}," +
                                 $"{a.BusinessSeats}," +
                                 $"{a.EconomySeats}," +
                                 $"{a.ManufacturingYear}," +
                                 $"{a.Status}");
                }
            }

            File.Replace(tempFile, Constants.AircraftsFile, null);
        }

        // Save Flights
        public static void SaveFlights()
        {
            string tempFile = Constants.FlightsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                // Write Header
                sw.WriteLine("FlightNumber,OriginAirportCode,DestinationAirportCode,AirlineICAO,AircraftRegNumber,ScheduledDeparture,ScheduledArrival,ActualDeparture,ActualArrival,Status,AvailableBusinessSeats,AvailableEconomySeats,BasePrice");

                // Loop through DataStore and write each record
                foreach (var x in DataStore.Flights)
                {
                    Flight f = x.Value;
                    sw.WriteLine($"{f.FlightNumber}," +
                                 $"{f.OriginAirportCode}," +
                                 $"{f.DestinationAirportCode}," +
                                 $"{f.AirlineICAO}," +
                                 $"{f.AircraftRegNumber}," +
                                 $"{f.ScheduledDeparture.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{f.ScheduledArrival.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{(f.ActualDeparture.HasValue ? f.ActualDeparture.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}," +
                                 $"{(f.ActualArrival.HasValue ? f.ActualArrival.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}," +
                                 $"{f.Status}," +
                                 $"{f.AvailableBusinessSeats}," +
                                 $"{f.AvailableEconomySeats}," +
                                 $"{f.BasePrice}");
                }
            }

            File.Replace(tempFile, Constants.FlightsFile, null);

        }

        // Save Passengers
        public static void SavePassengers()
        {
            string tempFile = Constants.PassengersFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("PassengerID,FullName,DateOfBirth,Nationality,PassportNumber,Email,Phone,RegistrationDate,LoyaltyPoints,TierStatus,Password,FailedLoginAttempts,LockoutUntil,LastLoginDate");

                foreach (var x in DataStore.Passengers)
                {
                    Passenger p = x.Value;
                    sw.WriteLine($"{p.PassengerID}," +
                                 $"{p.FullName}," +
                                 $"{p.DateOfBirth.ToString("yyyy-MM-dd")}," +
                                 $"{p.Nationality}," +
                                 $"{p.PassportNumber}," +
                                 $"{p.Email}," +
                                 $"{p.Phone}," +
                                 $"{p.RegistrationDate.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{p.LoyaltyPoints}," +
                                 $"{p.TierStatus}," +
                                 $"{p.Password}," +
                                 $"{p.FailedLoginAttempts}," +
                                 $"{(p.LockoutUntil.HasValue ? p.LockoutUntil.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}," +
                                 $"{(p.LastLoginDate.HasValue ? p.LastLoginDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}");
                }
            }

            File.Replace(tempFile, Constants.PassengersFile, null);
        }

        // Save Crew Members
        public static void SaveCrewMembers()
        {
            string tempFile = Constants.CrewMembersFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("EmployeeID,FullName,Role,Nationality,LicenseNumber,AirlineICAO,YearsOfExperience,IsAvailable");

                foreach (var x in DataStore.CrewMembers)
                {
                    CrewMember c = x.Value;
                    sw.WriteLine($"{c.EmployeeID}," +
                                 $"{c.FullName}," +
                                 $"{c.Role}," +
                                 $"{c.Nationality}," +
                                 $"{c.LicenseNumber}," +
                                 $"{c.AirlineICAO}," +
                                 $"{c.YearsOfExperience}," +
                                 $"{c.IsAvailable.ToString().ToLower()}");
                }
            }

            File.Replace(tempFile, Constants.CrewMembersFile, null);
        }

        // Save Flight Crew
        public static void SaveFlightCrew()
        {
            string tempFile = Constants.FlightCrewFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("FlightNumber,EmployeeID,AssignedDate");

                foreach (FlightCrew fc in DataStore.FlightCrew)
                {
                    sw.WriteLine($"{fc.FlightNumber}," +
                                 $"{fc.EmployeeID}," +
                                 $"{fc.AssignedDate.ToString("yyyy-MM-ddTHH:mm:ss")}");
                }
            }

            File.Replace(tempFile, Constants.FlightCrewFile, null);
        }

        // Save Tickets
        public static void SaveTickets()
        {
            string tempFile = Constants.TicketsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("TicketID,PassengerID,FlightNumber,SeatClass,SeatNumber,BookingDate,Status,FinalPrice,LoyaltyPointsEarned,PromoCode");

                foreach (var x in DataStore.Tickets)
                {
                    Ticket t = x.Value;
                    sw.WriteLine($"{t.TicketID}," +
                                 $"{t.PassengerID}," +
                                 $"{t.FlightNumber}," +
                                 $"{t.SeatClass}," +
                                 $"{t.SeatNumber}," +
                                 $"{t.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{t.Status}," +
                                 $"{t.FinalPrice}," +
                                 $"{t.LoyaltyPointsEarned}," +
                                 $"{t.PromoCode}");
                }
            }

            File.Replace(tempFile, Constants.TicketsFile, null);
        }

        // Save Baggages
        public static void SaveBaggages()
        {
            string tempFile = Constants.BaggagesFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("BaggageID,TicketID,WeightKg,BaggageType,Status");

                foreach (Baggage b in DataStore.Baggages)
                {
                    sw.WriteLine($"{b.BaggageID}," +
                                 $"{b.TicketID}," +
                                 $"{b.WeightKg}," +
                                 $"{b.Type}," +
                                 $"{b.Status}");
                }
            }

            File.Replace(tempFile, Constants.BaggagesFile, null);
        }

        // Save Promotions
        public static void SavePromotions()
        {
            string tempFile = Constants.PromotionsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("PromoCode,DiscountPercentage,StartDate,EndDate,MaxUses,CurrentUseCount,ApplicableClass,IsActive");

                foreach (var x in DataStore.Promotions)
                {
                    Promotion p = x.Value;
                    sw.WriteLine($"{p.PromoCode}," +
                                 $"{p.DiscountPercentage}," +
                                 $"{p.StartDate.ToString("yyyy-MM-dd")}," +
                                 $"{p.EndDate.ToString("yyyy-MM-dd")}," +
                                 $"{p.MaxUses}," +
                                 $"{p.CurrentUseCount}," +
                                 $"{p.ApplicableClass}," +
                                 $"{p.IsActive.ToString().ToLower()}");
                }
            }

            File.Replace(tempFile, Constants.PromotionsFile, null);
        }

        // Save Admins
        public static void SaveAdmins()
        {
            string tempFile = Constants.AdminsFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("AdminID,FullName,Email,Password,FailedLoginAttempts,LockoutUntil,LastLoginDate");

                foreach (var x in DataStore.Admins)
                {
                    Admin a = x.Value;
                    sw.WriteLine($"{a.AdminID}," +
                                 $"{a.FullName}," +
                                 $"{a.Email}," +
                                 $"{a.Password}," +
                                 $"{a.FailedLoginAttempts}," +
                                 $"{(a.LockoutUntil.HasValue ? a.LockoutUntil.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}," +
                                 $"{(a.LastLoginDate.HasValue ? a.LastLoginDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : "")}");
                }
            }

            File.Replace(tempFile, Constants.AdminsFile, null);
        }

        // Save Loyalty Logs
        public static void SaveLoyaltyLogs()
        {
            string tempFile = Constants.LoyaltyLogFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("LogID,PassengerID,TicketID,PointsChanged,Reason,TransactionDate");

                foreach (LoyaltyLog l in DataStore.LoyaltyLogs)
                {
                    sw.WriteLine($"{l.LogID}," +
                                 $"{l.PassengerID}," +
                                 $"{l.TicketID}," +
                                 $"{l.PointsChanged}," +
                                 $"{l.Reason}," +
                                 $"{l.TransactionDate.ToString("yyyy-MM-ddTHH:mm:ss")}");
                }
            }

            File.Replace(tempFile, Constants.LoyaltyLogFile, null);
        }

        // Save System Logs
        public static void SaveSystemLogs()
        {
            string tempFile = Constants.SystemLogFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("LogID,Timestamp,UserID,UserRole,ActionType,EntityAffected,Details");

                foreach (SystemLog l in DataStore.SystemLogs)
                {
                    sw.WriteLine($"{l.LogID}," +
                                 $"{l.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{l.UserID}," +
                                 $"{l.UserRole}," +
                                 $"{l.ActionType}," +
                                 $"{l.EntityAffected}," +
                                 $"{l.Details}");
                }
            }

            File.Replace(tempFile, Constants.SystemLogFile, null);
        }

        public static void WriteSystemLog(string userID, string userRole, string actionType, string entityAffected, string details)
        {
            string logID = $"SL{DataStore.SystemLogs.Count + 1:D4}";

            SystemLog log = new SystemLog
            {
                LogID = logID,
                Timestamp = DateTime.Now,
                UserID = userID,
                UserRole = userRole,
                ActionType = actionType,
                EntityAffected = entityAffected,
                Details = details
            };

            DataStore.SystemLogs.Add(log);
            SaveSystemLogs();
        }

        // Save Error Logs
        public static void SaveErrorLogs()
        {
            string tempFile = Constants.ErrorLogFile + ".tmp";

            using (StreamWriter sw = new StreamWriter(tempFile))
            {
                sw.WriteLine("LogID,Timestamp,ErrorMessage,StackTrace");

                foreach (ErrorLog l in DataStore.ErrorLogs)
                {
                    sw.WriteLine($"{l.LogID}," +
                                 $"{l.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss")}," +
                                 $"{l.ErrorMessage}," +
                                 $"{l.StackTrace}");
                }
            }

            File.Replace(tempFile, Constants.ErrorLogFile, null);
        }

    }

    static class Session
    {
        public static string CurrentUserID = "";
        public static string CurrentUserRole = "";  // "Admin" or "Passenger"
    }

    static class AuthService
    {
        /* 
         * ============== Login ==============
         */
        public static void Login()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║                Login Page                ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  [0] Back to Main Menu");
                Console.Write("\n  Enter your email: ");
                Console.ResetColor();
                string email = Console.ReadLine();

                if (email == "0") return;

                if (string.IsNullOrEmpty(email))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Email cannot be empty. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Admin? foundAdmin = DataStore.Admins.Values
                    .Cast<Admin?>()
                    .FirstOrDefault(a => a.Value.Email == email);

                Passenger? foundPassenger = DataStore.Passengers.Values
                    .Cast<Passenger?>()
                    .FirstOrDefault(p => p.Value.Email == email);

                if (!foundAdmin.HasValue && !foundPassenger.HasValue)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  No account found with that email. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                // Lockout check
                if (foundAdmin.HasValue && foundAdmin.Value.LockoutUntil.HasValue && foundAdmin.Value.LockoutUntil.Value > DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Account locked. Try again after {foundAdmin.Value.LockoutUntil.Value.ToString("HH:mm:ss")}");
                    Console.ResetColor();
                    Console.ReadLine();
                    return; // locked out — back to main menu
                }

                if (foundPassenger.HasValue && foundPassenger.Value.LockoutUntil.HasValue && foundPassenger.Value.LockoutUntil.Value > DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Account locked. Try again after {foundPassenger.Value.LockoutUntil.Value.ToString("HH:mm:ss")}");
                    Console.ResetColor();
                    Console.ReadLine();
                    return; // locked out — back to main menu
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter your password: ");
                Console.ResetColor();
                string password = Console.ReadLine();

                if (string.IsNullOrEmpty(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Password cannot be empty. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                if (foundAdmin.HasValue)
                {
                    Admin a = foundAdmin.Value;
                    if (a.Password == password)
                    {
                        a.FailedLoginAttempts = 0;
                        a.LockoutUntil = null;
                        a.LastLoginDate = DateTime.Now;
                        DataStore.Admins[a.AdminID] = a;
                        CsvHelper.SaveAdmins();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n  Welcome, {a.FullName}! Logged in as Admin.");
                        Console.ResetColor();
                        Console.ReadLine();

                        //Add Login System Log
                        SystemLog log = new SystemLog();
                        log.LogID = "SL" + (DataStore.SystemLogs.Count + 1).ToString("D5"); // Padding - minimum 5 digits
                        log.Timestamp = DateTime.Now;
                        log.UserID = a.AdminID;
                        log.UserRole = "Admin";
                        log.ActionType = "Login";
                        log.EntityAffected = "";
                        log.Details = $"Admin {a.FullName} logged in successfully.";
                        DataStore.SystemLogs.Add(log);
                        CsvHelper.SaveSystemLogs();


                        Session.CurrentUserID = a.AdminID;
                        Session.CurrentUserRole = "Admin";
                        AdminPortal.Show(a);


                        return; // success — exit login
                    }
                    else
                    {
                        a.FailedLoginAttempts++;
                        if (a.FailedLoginAttempts >= Constants.MaxFailedLoginAttempts)
                        {
                            a.LockoutUntil = DateTime.Now.AddMinutes(Constants.LockoutMinutes);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n  Too many failed attempts. Account locked for {Constants.LockoutMinutes} minutes.");
                            Console.ResetColor();
                            DataStore.Admins[a.AdminID] = a;
                            CsvHelper.SaveAdmins();
                            Console.ReadLine();
                            return; // locked — back to main menu
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n  Wrong password. {Constants.MaxFailedLoginAttempts - a.FailedLoginAttempts} attempts remaining.");
                            Console.ResetColor();
                            DataStore.Admins[a.AdminID] = a;
                            CsvHelper.SaveAdmins();
                            Console.ReadLine();
                            continue; // try again
                        }
                    }
                }
                else if (foundPassenger.HasValue)
                {
                    Passenger p = foundPassenger.Value;
                    if (p.Password == password)
                    {
                        p.FailedLoginAttempts = 0;
                        p.LockoutUntil = null;
                        p.LastLoginDate = DateTime.Now;
                        DataStore.Passengers[p.PassengerID] = p;
                        CsvHelper.SavePassengers();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n  Welcome, {p.FullName}! Logged in as Passenger.");
                        Console.ResetColor();
                        Console.ReadLine();

                        //Add Login System log
                        SystemLog log = new SystemLog();
                        log.LogID = "SL" + (DataStore.SystemLogs.Count + 1).ToString("D5");
                        log.Timestamp = DateTime.Now;
                        log.UserID = p.PassengerID;
                        log.UserRole = "Passenger";
                        log.ActionType = "Login";
                        log.EntityAffected = "";
                        log.Details = $"Passenger {p.FullName} logged in successfully.";
                        DataStore.SystemLogs.Add(log);
                        CsvHelper.SaveSystemLogs();


                        Session.CurrentUserID = p.PassengerID;
                        Session.CurrentUserRole = "Passenger";
                        PassengerPortal.Show(p);


                        return; // success — exit login
                    }
                    else
                    {
                        p.FailedLoginAttempts++;
                        if (p.FailedLoginAttempts >= Constants.MaxFailedLoginAttempts)
                        {
                            p.LockoutUntil = DateTime.Now.AddMinutes(Constants.LockoutMinutes);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n  Too many failed attempts. Account locked for {Constants.LockoutMinutes} minutes.");
                            Console.ResetColor();
                            DataStore.Passengers[p.PassengerID] = p;
                            CsvHelper.SavePassengers();
                            Console.ReadLine();
                            return; // locked — back to main menu
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n  Wrong password. {Constants.MaxFailedLoginAttempts - p.FailedLoginAttempts} attempts remaining.");
                            Console.ResetColor();
                            DataStore.Passengers[p.PassengerID] = p;
                            CsvHelper.SavePassengers();
                            Console.ReadLine();
                            continue; // try again
                        }
                    }
                }
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (password.Length < Constants.MinPasswordLength) return false;
            if (!Regex.IsMatch(password, @"\d")) return false; // at least one digit
            if (!Regex.IsMatch(password, @"[A-Z]")) return false; // at least one uppercase
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]]")) return false; // at least one special char
            return true;
        }

        public static void Register()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║            Registeration Page            ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  [0] Back to Main Menu");
                Console.Write("\n  Enter your name: ");
                string name = Console.ReadLine();

                if (name == "0") return;

                Passenger? foundPassenger;

                Console.Write("\n  Enter your date of birth (yyyy-MM-dd): ");
                string dobInput = Console.ReadLine();
                if (string.IsNullOrEmpty(dobInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Date of birth cannot be empty. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }
                // Parse safely
                if (!DateTime.TryParse(dobInput, out DateTime DOB))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid date format. Use yyyy-MM-dd. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Console.Write("\n  Enter your nationality: ");
                string nationality = Console.ReadLine();
                if (string.IsNullOrEmpty(nationality))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Nationality cannot be empty. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.Write("\n  Enter your passport number: ");
                string passport = Console.ReadLine();
                // Check passport number uniqeness 
                foundPassenger = DataStore.Passengers.Values
                    .Cast<Passenger?>()
                    .FirstOrDefault(p => p.Value.PassportNumber == passport);
                if (foundPassenger != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Passport already exists. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Console.Write("\n  Enter your email: ");
                string email = Console.ReadLine();
                // Check email uniqeness 
                foundPassenger = DataStore.Passengers.Values
                    .Cast<Passenger?>()
                    .FirstOrDefault(p => p.Value.Email == email);
                if (foundPassenger != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Email already exists. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Console.Write("\n  Enter your phone: ");
                string phone = Console.ReadLine();
                if (string.IsNullOrEmpty(phone))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Phone cannot be empty. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Console.Write("\n  Enter your password: ");
                string password = Console.ReadLine();

                if (!IsValidPassword(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Password must be with has digit, uppercase, special char, and length 8+ . Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue; // restart the loop
                }

                Passenger newPassenger = new Passenger();
                newPassenger.PassengerID = "P" + (DataStore.Passengers.Count + 1).ToString("D5");
                newPassenger.FullName = name;
                newPassenger.Nationality = nationality;
                newPassenger.DateOfBirth = DOB;
                newPassenger.PassportNumber = passport;
                newPassenger.Email = email;
                newPassenger.Phone = phone;
                newPassenger.RegistrationDate = DateTime.Now;
                newPassenger.LoyaltyPoints = 0; // default
                newPassenger.TierStatus = LoyaltyTier.Bronze; // default
                newPassenger.Password = password;
                newPassenger.FailedLoginAttempts = 0;
                newPassenger.LockoutUntil = null;
                newPassenger.LastLoginDate = null;

                // Add to DataStore
                DataStore.Passengers[newPassenger.PassengerID] = newPassenger;
                CsvHelper.SavePassengers();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Registration successful! Your ID is {newPassenger.PassengerID}. Press Enter to login.");
                Console.ResetColor();
                Console.ReadLine();

                // Add Login System Log
                SystemLog log = new SystemLog();
                log.LogID = "SL" + (DataStore.SystemLogs.Count + 1).ToString("D5");
                log.Timestamp = DateTime.Now;
                log.UserID = newPassenger.PassengerID;
                log.UserRole = "Passenger";
                log.ActionType = "Register";
                log.EntityAffected = $"Passenger {newPassenger.PassengerID}";
                log.Details = $"{newPassenger.FullName} registered successfully.";
                DataStore.SystemLogs.Add(log);
                CsvHelper.SaveSystemLogs();
                return;
            }
        }

        public static void Logout(string userID, string fullName, string role)
        {
            SystemLog log = new SystemLog();
            log.LogID = "SL" + (DataStore.SystemLogs.Count + 1).ToString("D5");
            log.Timestamp = DateTime.Now;
            log.UserID = userID;
            log.UserRole = role;
            log.ActionType = "Logout";
            log.EntityAffected = "";
            log.Details = $"{fullName} logged out.";
            DataStore.SystemLogs.Add(log);
            CsvHelper.SaveSystemLogs();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Logged out successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }
    }

    static class FlightService
    {
        private static void DisplayFlights(List<Flight> flights, TicketSeatClass seatClass)
        {
            Console.WriteLine($"{"Flight",-8} {"Airline",-8} {"Departure",-20} {"Arrival",-20} {"Duration",-10} {"Seats",-8} {"Price",-10}");
            Console.WriteLine(new string('-', 85));

            foreach (Flight f in flights)
            {
                TimeSpan duration = f.ScheduledArrival - f.ScheduledDeparture;
                string durationStr = $"{(int)duration.TotalHours}h {duration.Minutes}m";
                int availableSeats = seatClass == TicketSeatClass.Business
                    ? f.AvailableBusinessSeats
                    : f.AvailableEconomySeats;

                if (availableSeats == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{f.FlightNumber,-8} {f.AirlineICAO,-8} {f.ScheduledDeparture:yyyy-MM-dd HH:mm,-20} {f.ScheduledArrival:yyyy-MM-dd HH:mm,-20} {durationStr,-10} {"SOLD OUT",-8} {f.BasePrice,-10}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"{f.FlightNumber,-8} {f.AirlineICAO,-8} {f.ScheduledDeparture:yyyy-MM-dd HH:mm,-20} {f.ScheduledArrival:yyyy-MM-dd HH:mm,-20} {durationStr,-10} {availableSeats,-8} {f.BasePrice,-10}");
                }
            }
        }

        public static void Search(Passenger passenger)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║              Search Flights              ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  [0] Back");
                Console.Write("  One-way or round-trip? [1/2]: ");
                string trip = Console.ReadLine();
                if (trip == "0") return;
                if (trip != "1" && trip != "2")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid trip option. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }
                bool roundTrip = trip == "2";

                Console.Write("\n  Origin airport code: ");
                string oAirport = Console.ReadLine().ToUpper();
                if (!DataStore.Airports.ContainsKey(oAirport))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid origin airport code. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }
                Console.Write("\n  Destination airport code: ");
                string dAirport = Console.ReadLine().ToUpper();
                if (!DataStore.Airports.ContainsKey(dAirport))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid destination airport code. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }
                if (oAirport == dAirport)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Origin and destination cannot be the same.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }
                Console.Write("\n  Departure date (yyyy-MM-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime dDate))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid date format. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                DateTime rDate = DateTime.MinValue;
                if (roundTrip)
                {
                    Console.Write("\n  Return date (yyyy-MM-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out rDate))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid date format. Press Enter.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;
                    }
                }
                Console.Write("\n  Seat class [1] Economy [2] Business: ");
                TicketSeatClass seatClass = TicketSeatClass.Economy;
                switch (Console.ReadLine())
                {
                    case "1":
                        seatClass = TicketSeatClass.Economy;
                        break;

                    case "2":
                        seatClass = TicketSeatClass.Business;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Used economy seat class by default.");
                        Console.ResetColor();
                        break;
                }
                Console.Write("\n  Maximum price (optional — press Enter to skip): ");
                decimal.TryParse(Console.ReadLine(), out decimal maxPrice);
                Console.ResetColor();

                List<Flight> results = DataStore.Flights.Values
                    .Where(f => f.OriginAirportCode == oAirport &&
                                f.DestinationAirportCode == dAirport &&
                                f.ScheduledDeparture.Date == dDate.Date &&
                                f.Status != FlightStatus.Cancelled &&
                                (maxPrice == 0 || f.BasePrice <= maxPrice))
                    .ToList();

                List<Flight> returnResults = new();
                if (roundTrip)
                {
                    returnResults = DataStore.Flights.Values
                        .Where(f => f.OriginAirportCode == dAirport &&
                                    f.DestinationAirportCode == oAirport &&
                                    f.ScheduledDeparture.Date == rDate.Date &&
                                    f.Status != FlightStatus.Cancelled &&
                                    (maxPrice == 0 || f.BasePrice <= maxPrice))
                        .ToList();
                }

                if (results.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  No flights found. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Select sort type
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Price");
                Console.WriteLine("  [2] Duration");
                Console.WriteLine("  [3] Departure Time");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select sort option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        results = results.OrderBy(f => f.BasePrice).ToList();
                        returnResults = returnResults.OrderBy(f => f.BasePrice).ToList();
                        break;

                    case "2":
                        results = results.OrderBy(f => f.ScheduledArrival - f.ScheduledDeparture).ToList();
                        returnResults = returnResults.OrderBy(f => f.ScheduledArrival - f.ScheduledDeparture).ToList();
                        break;

                    case "3":
                        results = results.OrderBy(f => f.ScheduledDeparture).ToList();
                        returnResults = returnResults.OrderBy(f => f.ScheduledDeparture).ToList();
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  Outbound Flights:");
                Console.ResetColor();
                DisplayFlights(results, seatClass);

                if (roundTrip)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n  Return Flights:");
                    Console.ResetColor();
                    DisplayFlights(returnResults, seatClass);
                }

                /*
                 * Book a Ticket
                 */

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Book Ticket");
                Console.WriteLine("  [Enter] Continue Flight Search");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();
                string outboundFlightNumber = "";
                string returnFlightNumber = "";
                if (Console.ReadLine() == "1")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Enter outbound flight number: ");
                    outboundFlightNumber = Console.ReadLine();
                    Console.ResetColor();
                    bool exists = results.Any(f => f.FlightNumber == outboundFlightNumber);
                    if (!exists)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid outbound flight number. Press Enter");
                        Console.ReadLine();
                        Console.ResetColor();
                        continue;
                    }

                    if (roundTrip && returnResults.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("\n  Enter return flight number: ");
                        returnFlightNumber = Console.ReadLine();
                        Console.ResetColor();
                        exists = returnResults.Any(f => f.FlightNumber == returnFlightNumber);
                        if (!exists)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  Invalid outbound flight number. Press Enter");
                            Console.ReadLine();
                            Console.ResetColor();
                            continue;
                        }
                    }
                    else if (roundTrip && returnResults.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n  No return flights found for selected date.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;
                    }
                    TicketService.BookTicket(passenger, outboundFlightNumber, roundTrip, returnFlightNumber, seatClass);
                }
            }
        }

        public static string GetFlightNumber()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter flight Number (0 to cancel): ");
            Console.ResetColor();
            return Console.ReadLine().ToUpper();
        }

        public static void AddFlight()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();

                if (flightNumber == "0")
                    return;

                if (string.IsNullOrEmpty(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight number cannot be empty.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                if (DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight already exists. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // --- Airline ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Airline ICAO Code: ");
                Console.ResetColor();
                string icaoCode = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (!DataStore.Airlines.ContainsKey(icaoCode))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Airline '{icaoCode}' not found.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Aircraft ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Aircraft Registration Number: ");
                Console.ResetColor();
                string regNumber = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (!DataStore.Aircrafts.ContainsKey(regNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Aircraft '{regNumber}' not found.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                if (DataStore.Aircrafts[regNumber].Status != AircraftStatus.Active)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Aircraft is not active.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Origin Airport ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Origin IATA Code: ");
                Console.ResetColor();
                string origin = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (!DataStore.Airports.ContainsKey(origin))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Airport '{origin}' not found.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Destination Airport ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Destination IATA Code: ");
                Console.ResetColor();
                string destination = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (!DataStore.Airports.ContainsKey(destination))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Airport '{destination}' not found.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                if (destination == origin)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Origin and destination cannot be the same.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Scheduled Departure ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Scheduled Departure (yyyy-MM-dd HH:mm): ");
                Console.ResetColor();
                string depInput = Console.ReadLine()?.Trim() ?? "";

                if (!DateTime.TryParse(depInput, out DateTime scheduledDeparture))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid departure date format.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                if (scheduledDeparture <= DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Departure must be in the future.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Scheduled Arrival ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Scheduled Arrival (yyyy-MM-dd HH:mm): ");
                Console.ResetColor();
                string arrInput = Console.ReadLine()?.Trim() ?? "";

                if (!DateTime.TryParse(arrInput, out DateTime scheduledArrival))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid arrival date format.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                if (scheduledArrival <= scheduledDeparture)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Arrival must be after departure.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Seat Counts ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Total Economy Seats: ");
                Console.ResetColor();
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int totalEconomy) || totalEconomy < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid economy seat count.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Total Business Seats: ");
                Console.ResetColor();
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int totalBusiness) || totalBusiness < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid business seat count.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                if (totalEconomy + totalBusiness == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight must have at least one seat.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Base Price ---
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Base Price (Economy): ");
                Console.ResetColor();
                if (!decimal.TryParse(Console.ReadLine()?.Trim(), out decimal basePrice) || basePrice <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid base price.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Build and Save ---
                Flight newFlight = new Flight
                {
                    FlightNumber = flightNumber,
                    AirlineICAO = icaoCode,
                    AircraftRegNumber = regNumber,
                    OriginAirportCode = origin,
                    DestinationAirportCode = destination,
                    ScheduledDeparture = scheduledDeparture,
                    ScheduledArrival = scheduledArrival,
                    ActualDeparture = null,
                    ActualArrival = null,
                    Status = FlightStatus.Scheduled,
                    AvailableEconomySeats = totalEconomy,
                    AvailableBusinessSeats = totalBusiness,
                    BasePrice = basePrice
                };

                DataStore.Flights[flightNumber] = newFlight;

                bool crewAssigned = CrewService.AssignCrewToFlight(flightNumber);

                if (!crewAssigned)
                {
                    // Roll back — remove from DataStore without saving
                    DataStore.Flights.Remove(flightNumber);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight creation cancelled. No changes saved.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                // --- Save both together only after crew is confirmed ---
                CsvHelper.SaveFlights();
                CsvHelper.SaveFlightCrew();

                // AddFlight
                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "CREATE", "Flight", $"Flight {flightNumber} created.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Flight '{flightNumber}' created and crew assigned successfully.");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        public static void ViewAllFlights()
        {
            List<Flight> flights = DataStore.Flights.Values.ToList();

            if (flights.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  No flights found.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            int totalPages = (int)Math.Ceiling(flights.Count / (double)Constants.PageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n  ---------- VIEW ALL FLIGHTS ----------");

                List<Flight> pageItems = flights
                    .Skip((currentPage - 1) * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .ToList();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    $"\n  {"FlightNo",-10} {"Origin",-8} {"Destination",-12} {"Airline",-8} {"Aircraft",-12} " +
                    $"{"SchedDep",-20} {"SchedArr",-20} {"ActDep",-20} {"ActArr",-20} " +
                    $"{"Status",-10} {"Biz",-6} {"Eco",-6} {"Price",-8}"
                );
                Console.WriteLine(new string('-', 171));

                foreach (Flight flight in pageItems)
                {
                    string schedDep = flight.ScheduledDeparture.ToString("MM/dd HH:mm");
                    string schedArr = flight.ScheduledArrival.ToString("MM/dd HH:mm");
                    string actDep = flight.ActualDeparture?.ToString("MM/dd HH:mm") ?? "-";
                    string actArr = flight.ActualArrival?.ToString("MM/dd HH:mm") ?? "-";

                    Console.WriteLine(
                        $"   {flight.FlightNumber,-10}" +
                        $" {flight.OriginAirportCode,-8}" +
                        $" {flight.DestinationAirportCode,-12}" +
                        $" {flight.AirlineICAO,-8}" +
                        $" {flight.AircraftRegNumber,-10}" +
                        $" {schedDep,-20}" +
                        $" {schedArr,-20}" +
                        $" {actDep,-20}" +
                        $" {actArr,-20}" +
                        $" {flight.Status,-10}" +
                        $" {flight.AvailableBusinessSeats,-6}" +
                        $" {flight.AvailableEconomySeats,-6}" +
                        $" {flight.BasePrice,-8:0.00}"
                    );
                }

                // Footer
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  Page {currentPage} of {totalPages}  |  Total: {flights.Count} flights");
                Console.WriteLine("  [N] Next   [P] Previous   [0] Back");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Choice: ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (input == "0") return;
                else if (input == "N" && currentPage < totalPages) currentPage++;
                else if (input == "P" && currentPage > 1) currentPage--;
            }
        }

        public static void UpdateFlight()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();

                if (flightNumber == "0")
                    return;

                if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Flight flight = DataStore.Flights[flightNumber];

                while (true)
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  Update Flight - {flight.FlightNumber}");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  [1] Origin Airport ({flight.OriginAirportCode})");
                    Console.WriteLine($"  [2] Destination Airport ({flight.DestinationAirportCode})");
                    Console.WriteLine($"  [3] Scheduled Departure ({flight.ScheduledDeparture:yyyy-MM-dd HH:mm})");
                    Console.WriteLine($"  [4] Scheduled Arrival ({flight.ScheduledArrival:yyyy-MM-dd HH:mm})");
                    Console.WriteLine($"  [5] Status ({flight.Status})");
                    Console.WriteLine($"  [6] Business Seats ({flight.AvailableBusinessSeats})");
                    Console.WriteLine($"  [7] Economy Seats ({flight.AvailableEconomySeats})");
                    Console.WriteLine($"  [8] Base Price ({flight.BasePrice:0.00})");
                    Console.WriteLine("  [0] Back");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Select a field to update: ");
                    Console.ResetColor();

                    string choice = Console.ReadLine();
                    string updatedField = "";

                    switch (choice)
                    {
                        case "1":
                            Console.Write("  New Origin Airport: ");
                            string orgAirportCode = Console.ReadLine().Trim().ToUpper();
                            if (!DataStore.Airports.ContainsKey(orgAirportCode))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("  Airport does not exist. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }
                            flight.OriginAirportCode = orgAirportCode;
                            updatedField = $"OriginAirportCode -> {orgAirportCode}";
                            break;

                        case "2":
                            Console.Write("  New Destination Airport: ");
                            string destAirportCode = Console.ReadLine().Trim().ToUpper();
                            if (!DataStore.Airports.ContainsKey(destAirportCode))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("  Airport does not exist. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }
                            flight.DestinationAirportCode = destAirportCode;
                            updatedField = $"DestinationAirportCode -> {destAirportCode}";
                            break;

                        case "3":
                            Console.Write("  New Scheduled Departure (yyyy-MM-dd HH:mm): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime dep))
                            {
                                flight.ScheduledDeparture = dep;
                                updatedField = $"ScheduledDeparture -> {dep:yyyy-MM-dd HH:mm}";
                            }
                            break;

                        case "4":
                            Console.Write("  New Scheduled Arrival (yyyy-MM-dd HH:mm): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime arr))
                            {
                                flight.ScheduledArrival = arr;
                                updatedField = $"ScheduledArrival -> {arr:yyyy-MM-dd HH:mm}";
                            }
                            break;

                        case "5":
                            Console.Write("  New Status: ");
                            if (Enum.TryParse(Console.ReadLine(), true, out FlightStatus status))
                            {
                                flight.Status = status;
                                updatedField = $"Status -> {status}";
                            }
                            break;

                        case "6":
                            Console.Write("  New Business Seats: ");
                            if (int.TryParse(Console.ReadLine(), out int bizSeats))
                            {
                                flight.AvailableBusinessSeats = bizSeats;
                                updatedField = $"AvailableBusinessSeats -> {bizSeats}";
                            }
                            break;

                        case "7":
                            Console.Write("  New Economy Seats: ");
                            if (int.TryParse(Console.ReadLine(), out int ecoSeats))
                            {
                                flight.AvailableEconomySeats = ecoSeats;
                                updatedField = $"AvailableEconomySeats -> {ecoSeats}";
                            }
                            break;

                        case "8":
                            Console.Write("  New Base Price: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal price))
                            {
                                flight.BasePrice = price;
                                updatedField = $"BasePrice -> {price:0.00}";
                            }
                            break;

                        case "0":
                            CsvHelper.SaveFlights();
                            return;
                    }

                    if (!string.IsNullOrEmpty(updatedField))
                    {
                        DataStore.Flights[flightNumber] = flight;
                        // ++ system log
                        CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                            "UPDATE", "Flight", $"Flight {flightNumber} updated: {updatedField}.");
                    }
                }
            }
        }

        private static void CancelFlightAndTickets(string flightNumber)
        {
            List<Ticket> tickets = DataStore.Tickets.Values
                .Where(t => t.FlightNumber == flightNumber &&
                            t.Status != TicketStatus.Cancelled)
                .ToList();

            foreach (Ticket ticket in tickets)
            {
                Passenger p = DataStore.Passengers[ticket.PassengerID];
                p.LoyaltyPoints -= ticket.LoyaltyPointsEarned;
                p.TierStatus = TicketService.GetUpdatedTier(p.LoyaltyPoints);
                DataStore.Passengers[ticket.PassengerID] = p;

                Ticket updated = ticket;
                updated.Status = TicketStatus.Cancelled;
                DataStore.Tickets[updated.TicketID] = updated;

                for (int i = 0; i < DataStore.Baggages.Count; i++)
                {
                    if (DataStore.Baggages[i].TicketID == ticket.TicketID &&
                        DataStore.Baggages[i].Status != BaggageStatus.Delivered)
                    {
                        Baggage b = DataStore.Baggages[i];
                        b.Status = BaggageStatus.Delivered;
                        DataStore.Baggages[i] = b;
                    }
                }
            }

            DataStore.FlightCrew.RemoveAll(fc => fc.FlightNumber == flightNumber);

            Flight f = DataStore.Flights[flightNumber];
            f.Status = FlightStatus.Cancelled;
            f.AvailableBusinessSeats = 0;
            f.AvailableEconomySeats = 0;
            DataStore.Flights[flightNumber] = f;

            CsvHelper.SaveFlights();
            CsvHelper.SaveTickets();
            CsvHelper.SavePassengers();
            CsvHelper.SaveFlightCrew();

            // ++ system log
            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                "CANCEL", "Flight", $"Flight {flightNumber} cancelled with {tickets.Count} ticket(s) voided.");
        }

        public static void DeleteFlight()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();

                if (flightNumber == "0")
                    return;

                if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Flight flight = DataStore.Flights[flightNumber];

                bool hasTickets = DataStore.Tickets.Values
                    .Any(t => t.FlightNumber == flightNumber &&
                              t.Status != TicketStatus.Cancelled);

                if (hasTickets)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  This flight has confirmed tickets.");
                    Console.WriteLine("  [1] Cancel flight and all related tickets");
                    Console.WriteLine("  [Enter] Abort");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Select option: ");
                    Console.ResetColor();

                    if (Console.ReadLine()?.Trim() != "1")
                    {
                        // ++ system log — aborted
                        CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                            "DELETE", "Flight", $"Flight {flightNumber} deletion aborted by admin.");
                        return;
                    }
                }

                CancelFlightAndTickets(flightNumber);  // log is written inside here

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Flight and all related tickets cancelled. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        public static void SetActualTimes()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();
                if (flightNumber == "0") return;

                if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Flight flight = DataStore.Flights[flightNumber];

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter actual departure datetime (yyyy-MM-dd HH:mm): ");
                Console.ResetColor();
                string actDepDate = Console.ReadLine();

                if (!DateTime.TryParse(actDepDate, out DateTime depTime))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid date format. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter actual arrival datetime (yyyy-MM-dd HH:mm): ");
                Console.ResetColor();
                string actArrDate = Console.ReadLine();

                if (!DateTime.TryParse(actArrDate, out DateTime arrTime))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid date format. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                if (arrTime <= depTime)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Arrival time must be after departure time. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                flight.ActualDeparture = depTime;
                flight.ActualArrival = arrTime;

                DataStore.Flights[flightNumber] = flight;
                CsvHelper.SaveFlights();

                // ++ system log
                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                    "UPDATE", "Flight",
                    $"Flight {flightNumber} actual times set: Dep {depTime:yyyy-MM-dd HH:mm}, Arr {arrTime:yyyy-MM-dd HH:mm}.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Actual times updated successfully. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        /*
        * Allow updating flight status with appropriate transition rules (e.g., a Cancelled flight cannot be moved back to Scheduled)
        */
        private static bool IsValidTransition(FlightStatus current, FlightStatus next)
        {
            if (current == FlightStatus.Cancelled) return false;
            if (current == FlightStatus.Arrived) return false;
            if (current == FlightStatus.Departed && next != FlightStatus.Arrived) return false;
            return true;
        }

        public static void BulkUpdateStatus()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter Airline ICAO code OR Aircraft Registration Number (0 to cancel): ");
                Console.ResetColor();
                string input = Console.ReadLine().ToUpper();
                if (input == "0") return;

                if (!DataStore.Airlines.ContainsKey(input) && !DataStore.Aircrafts.ContainsKey(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid ICAO code/Registration Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  Select new status:");
                Console.WriteLine("  [1] Scheduled");
                Console.WriteLine("  [2] Boarding");
                Console.WriteLine("  [3] Departed");
                Console.WriteLine("  [4] Arrived");
                Console.WriteLine("  [5] Delayed");
                Console.WriteLine("  [6] Cancelled");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select: ");
                FlightStatus parsedStatus;
                switch (Console.ReadLine())
                {
                    case "1": parsedStatus = FlightStatus.Scheduled; break;
                    case "2": parsedStatus = FlightStatus.Boarding; break;
                    case "3": parsedStatus = FlightStatus.Departed; break;
                    case "4": parsedStatus = FlightStatus.Arrived; break;
                    case "5": parsedStatus = FlightStatus.Delayed; break;
                    case "6": parsedStatus = FlightStatus.Cancelled; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;
                }

                List<Flight> flights = DataStore.Airlines.ContainsKey(input)
                    ? DataStore.Flights.Values.Where(f => f.AirlineICAO == input).ToList()
                    : DataStore.Flights.Values.Where(f => f.AircraftRegNumber == input).ToList();

                if (flights.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  No flights found for this airline/aircraft. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                int updatedCount = 0;
                for (int i = 0; i < flights.Count; i++)
                {
                    Flight f = flights[i];
                    if (!IsValidTransition(f.Status, parsedStatus))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n  Cannot update {f.FlightNumber} from {f.Status} to {parsedStatus} — skipped.");
                        Console.ResetColor();
                        continue;
                    }
                    f.Status = parsedStatus;
                    DataStore.Flights[f.FlightNumber] = f;
                    updatedCount++;
                }

                CsvHelper.SaveFlights();

                // ++ system log
                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                    "UPDATE", "Flight",
                    $"Bulk status update to {parsedStatus} for {input}: {updatedCount}/{flights.Count} flight(s) updated.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  {updatedCount} flight(s) updated to {parsedStatus}. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        private static List<Ticket> GetFlightTickets(string flightNumber)
        {
            return DataStore.Tickets.Values
                .Where(t => t.FlightNumber == flightNumber)
                .OrderBy(t => t.SeatClass)
                .ThenBy(t => t.SeatNumber)
                .ToList();
        }

        public static void ViewManifest()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();
                if (flightNumber == "0") return;
                else if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                List<Ticket> tickets = GetFlightTickets(flightNumber);

                if (tickets.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  No passengers found for this flight. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Flight flight = DataStore.Flights[flightNumber];

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  Passenger Manifest — Flight {flightNumber}");
                Console.WriteLine($"  {flight.OriginAirportCode} => {flight.DestinationAirportCode} | {flight.ScheduledDeparture:yyyy-MM-dd HH:mm}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n{"Seat",-8} {"Class",-12} {"Passenger",-25} {"Nationality",-15} {"Passport",-15} {"Status"}");
                Console.WriteLine(new string('-', 90));

                foreach (Ticket t in tickets)
                {
                    Passenger p = DataStore.Passengers[t.PassengerID];
                    Console.WriteLine($"{t.SeatNumber,-8} {t.SeatClass,-12} {p.FullName,-25} {p.Nationality,-15} {p.PassportNumber,-15} {t.Status}");
                }

                Console.WriteLine(new string('-', 90));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Total Passengers: {tickets.Count}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  Press Enter to continue.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        private static List<CrewMember> GetFlightCrew(string flightNumber)
        {
            return DataStore.FlightCrew
                .Where(fc => fc.FlightNumber == flightNumber)
                .Select(fc => DataStore.CrewMembers[fc.EmployeeID])
                .ToList();
        }

        public static void ViewCrewAssignment()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();
                if (flightNumber == "0") return;
                else if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                List <CrewMember> crewMembers = GetFlightCrew(flightNumber);

                if (crewMembers.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  No crew found for this flight. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  Crew Assignment View — Flight {flightNumber}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n{"ID",-10} {"Name",-25} {"Role",-12} {"Nationality",-15} {"License",-15} {"Airline",-8} {"Exp",-5} {"Status"}");
                Console.WriteLine(new string('-', 110));

                foreach (CrewMember crew in crewMembers)
                {
                    Console.WriteLine(
                        $"{crew.EmployeeID,-10} " +
                        $"{crew.FullName,-25} " +
                        $"{crew.Role,-12} " +
                        $"{crew.Nationality,-15} " +
                        $"{(string.IsNullOrEmpty(crew.LicenseNumber) ? "N/A" : crew.LicenseNumber),-15} " +
                        $"{crew.AirlineICAO,-8} " +
                        $"{crew.YearsOfExperience,-5} " +
                        $"{(crew.IsAvailable ? "Available" : "Unavailable")}"
                    );
                }

                Console.WriteLine(new string('-', 110));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Total Crew Members: {crewMembers.Count}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  Press Enter to continue.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        public static void ExportFlightReport()
        {
            while (true)
            {
                string flightNumber = GetFlightNumber();
                if (flightNumber == "0") return;
                else if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid flight Number. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Flight flight = DataStore.Flights[flightNumber];

                var report = new StringBuilder();

                report.AppendLine("========================================");
                report.AppendLine("FLIGHT REPORT");
                report.AppendLine("========================================");
                report.AppendLine();
                report.AppendLine("FLIGHT DETAILS");
                report.AppendLine($"Flight Number: {flight.FlightNumber}");
                report.AppendLine($"Origin: {flight.OriginAirportCode}");
                report.AppendLine($"Destination: {flight.DestinationAirportCode}");
                report.AppendLine($"Departure: {flight.ScheduledDeparture}");
                report.AppendLine($"Arrival: {flight.ScheduledArrival}");
                report.AppendLine($"Status: {flight.Status}");
                report.AppendLine();
                // Passenger Section
                var tickets = GetFlightTickets(flightNumber);
                report.AppendLine("PASSENGER MANIFEST");
                report.AppendLine($"{"Seat",-8} {"Class",-12} {"Passenger",-25}");
                report.AppendLine("----------------------------------------");
                foreach (Ticket t in tickets)
                {
                    Passenger p = DataStore.Passengers[t.PassengerID];

                    report.AppendLine(
                        $"{t.SeatNumber,-8} " +
                        $"{t.SeatClass,-12} " +
                        $"{p.FullName,-25}"
                    );
                }
                report.AppendLine();
                report.AppendLine($"Total Passengers: {tickets.Count}");
                report.AppendLine();
                // Crew Section
                var crewMembers = GetFlightCrew(flightNumber);
                report.AppendLine("CREW LIST");
                report.AppendLine($"{"ID",-10} {"Name",-25} {"Role",-12}");
                report.AppendLine("----------------------------------------");
                foreach (CrewMember crew in crewMembers)
                {
                    report.AppendLine(
                        $"{crew.EmployeeID,-10} " +
                        $"{crew.FullName,-25} " +
                        $"{crew.Role,-12}"
                    );
                }
                report.AppendLine();
                report.AppendLine($"Total Crew: {crewMembers.Count}");

                // Save Report
                Directory.CreateDirectory(Constants.ReportsFolder);

                string filePath = Path.Combine(
                    Constants.ReportsFolder,
                    $"FlightReport_{flightNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                );
                File.WriteAllText(filePath, report.ToString());

                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "EXPORT", "Report", $"Flight report exported for {flightNumber}.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  The report has been saved in {filePath}. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║             Flight Management            ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Add New Flight");
                Console.WriteLine("  [2] View Flights");
                Console.WriteLine("  [3] Update Flight");
                Console.WriteLine("  [4] Delete Flight");
                Console.WriteLine("  [5] Set Actual Departure/Arrival");
                Console.WriteLine("  [6] Bulk-Update Flight Status");
                Console.WriteLine("  [7] Passenger Manifest");
                Console.WriteLine("  [8] Crew Assignment View");
                Console.WriteLine("  [9] Export Flight Report");
                Console.WriteLine("  [0] Back to Admin Dashboard");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        AddFlight();
                        break;

                    case "2":
                        ViewAllFlights();
                        break;

                    case "3":
                        UpdateFlight();
                        break;

                    case "4":
                        DeleteFlight();
                        break;

                    case "5":
                        SetActualTimes();
                        break;

                    case "6":
                        BulkUpdateStatus();
                        break;

                    case "7":
                        ViewManifest();
                        break;

                    case "8":
                        ViewCrewAssignment();
                        break;

                    case "9":
                        ExportFlightReport();
                        break;

                    case "0":
                        return;
                }
            }
        }
    }

    static class CrewService
    {
        public static bool AssignCrewToFlight(string flightNumber)
        {
            Console.WriteLine("\n  ----------- ASSIGN CREW -----------");

            // Show available crew
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{"ID",-12} {"Name",-25} {"Role",-15}");
            Console.WriteLine(new string('-', 52));
            Console.ResetColor();

            foreach (var crew in DataStore.CrewMembers.Values)
            {
                Console.WriteLine($"{crew.EmployeeID,-12} {crew.FullName,-25} {crew.Role,-15}");
            }

            Console.WriteLine();

            // Track entries added this session so we can roll back if cancelled
            List<FlightCrew> addedThisSession = new List<FlightCrew>();

            bool addingMore = true;
            while (addingMore)
            {
                bool hasPilot = DataStore.FlightCrew.Any(fc => fc.FlightNumber == flightNumber &&
                                   DataStore.CrewMembers[fc.EmployeeID].Role == CrewMemberRole.Pilot);
                bool hasCoPilot = DataStore.FlightCrew.Any(fc => fc.FlightNumber == flightNumber &&
                                   DataStore.CrewMembers[fc.EmployeeID].Role == CrewMemberRole.CoPilot);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Employee ID (Enter to finish / 0 to cancel): ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (input == "0")
                {
                    // Roll back FlightCrew entries added this session
                    foreach (var entry in addedThisSession)
                        DataStore.FlightCrew.Remove(entry);

                    return false;
                }

                if (string.IsNullOrEmpty(input))
                {
                    if (!hasPilot || !hasCoPilot)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  A flight requires at least one Pilot and one CoPilot.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;
                    }
                    addingMore = false;
                    continue;
                }

                if (!DataStore.CrewMembers.ContainsKey(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Employee '{input}' not found.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Duplicate check
                bool alreadyAssigned = DataStore.FlightCrew
                    .Any(fc => fc.FlightNumber == flightNumber && fc.EmployeeID == input);

                if (alreadyAssigned)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  This crew member is already assigned to this flight.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Scheduling conflict — crew on another flight at the same time
                Flight thisFlight = DataStore.Flights[flightNumber];
                bool hasConflict = DataStore.FlightCrew
                    .Where(fc => fc.EmployeeID == input && fc.FlightNumber != flightNumber)
                    .Any(fc =>
                    {
                        Flight other = DataStore.Flights[fc.FlightNumber];
                        return thisFlight.ScheduledDeparture < other.ScheduledArrival &&
                               thisFlight.ScheduledArrival > other.ScheduledDeparture;
                    });

                if (hasConflict)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  This crew member has a scheduling conflict with another flight.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                FlightCrew assignment = new FlightCrew
                {
                    FlightNumber = flightNumber,
                    EmployeeID = input
                };

                DataStore.FlightCrew.Add(assignment);
                addedThisSession.Add(assignment);   // track for potential rollback

                // AssignCrewToFlight
                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "ASSIGN", "FlightCrew", $"Crew {input} assigned to flight {flightNumber}.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Crew member '{input}' assigned successfully. Press Enter");
                Console.ResetColor();
                Console.ReadLine();
            }

            return true;
        }
    }

    static class TicketService
    {
        public static void ShowTickets(Passenger passenger, List<TicketStatus> statuses)
        {
            List<Ticket> tickets = DataStore.Tickets.Values
            .Where(t => t.PassengerID == passenger.PassengerID && statuses.Contains(t.Status))
            .ToList();

            if (tickets.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  No tickets found.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n{"Ticket ID",-12} {"Flight",-8} {"From",-6} {"To",-6} {"Departure",-20} {"Class",-10} {"Seat",-6} {"Price",-10} {"Points",-8} {"Promo",-10} {"Status"}");
            Console.WriteLine(new string('-', 110));

            foreach (Ticket t in tickets)
            {
                Flight f = DataStore.Flights[t.FlightNumber];
                Console.WriteLine(
                    $"{t.TicketID,-12} " +
                    $"{f.FlightNumber,-8} " +
                    $"{f.OriginAirportCode,-6} " +
                    $"{f.DestinationAirportCode,-6} " +
                    $"{f.ScheduledDeparture.ToString("yyyy-MM-dd HH:mm"),-20} " +
                    $"{t.SeatClass,-10} " +
                    $"{t.SeatNumber,-6} " +
                    $"{t.FinalPrice.ToString("C"),-10} " +
                    $"{t.LoyaltyPointsEarned,-8} " +
                    $"{(string.IsNullOrEmpty(t.PromoCode) ? "None" : t.PromoCode),-10} " +
                    $"{t.Status}"
                );
            }
            Console.WriteLine(new string('-', 110));
            Console.ResetColor();
        }

        public static void CancelTicket(Passenger passenger)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter ticket ID: ");
            string ticketID = Console.ReadLine();
            Console.ResetColor();

            Ticket? ticket = DataStore.Tickets.Values
                .FirstOrDefault(t => t.TicketID == ticketID &&
                    t.PassengerID == passenger.PassengerID &&
                    (t.Status == TicketStatus.Confirmed ||
                     t.Status == TicketStatus.CheckedIn));


            if (ticket == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid ticket ID or ticket is not upcoming. Press Enter to try again.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Ticket t = ticket.Value;
            Flight flight = DataStore.Flights[t.FlightNumber];

            if (!(flight.ScheduledDeparture > DateTime.Now))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Ticket cannot be cancelled. Flight already departed. Press Enter");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            t.Status = TicketStatus.Cancelled;
            DataStore.Tickets[t.TicketID] = t;

            // Restore seat
            if (t.SeatClass == TicketSeatClass.Business)
                flight.AvailableBusinessSeats++;
            else
                flight.AvailableEconomySeats++;

            DataStore.Flights[t.FlightNumber] = flight;

            CsvHelper.SaveTickets();
            CsvHelper.SaveTickets();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Ticket cancelled successfully! Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void AddBaggage(string ticketID)
        {
            // Select baggage type
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  Baggage Type:");
            Console.WriteLine("  [1] Cabin");
            Console.WriteLine("  [2] Hold");
            Console.WriteLine("  [3] Oversized");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select type: ");
            Console.ResetColor();

            BaggageType type;
            switch (Console.ReadLine())
            {
                case "1": type = BaggageType.Cabin; break;
                case "2": type = BaggageType.Hold; break;
                case "3": type = BaggageType.Oversized; break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid type. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
            }

            // Weight limit check
            decimal maxWeight = 0;

            if (type == BaggageType.Cabin)
                maxWeight = Constants.MaxCabinWeight;
            else if (type == BaggageType.Hold)
                maxWeight = Constants.MaxHoldWeight;
            else if (type == BaggageType.Oversized)
                maxWeight = Constants.MaxOversizedWeight;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"\n  Enter weight in kg (max {maxWeight}kg): ");
            Console.ResetColor();

            if (!decimal.TryParse(Console.ReadLine(), out decimal weight) || weight <= 0 || weight > maxWeight)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  Invalid weight. Must be between 0 and {maxWeight}kg. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Create baggage
            Baggage newBaggage = new Baggage();
            newBaggage.BaggageID = "BG" + (DataStore.Baggages.Count + 1).ToString("D5");
            newBaggage.TicketID = ticketID;
            newBaggage.WeightKg = weight;
            newBaggage.Type = type;
            newBaggage.Status = BaggageStatus.CheckedIn;

            DataStore.Baggages.Add(newBaggage);
            CsvHelper.SaveBaggages();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Baggage {newBaggage.BaggageID} added successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void UpdateBaggage(string ticketID)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter baggage ID to update: ");
            string baggageID = Console.ReadLine();
            Console.ResetColor();

            int index = DataStore.Baggages.FindIndex(b => b.BaggageID == baggageID && b.TicketID == ticketID);

            if (index == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Baggage not found. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"\n  Enter new weight in kg: ");
            Console.ResetColor();

            if (!decimal.TryParse(Console.ReadLine(), out decimal newWeight) || newWeight <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid weight. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Weight limit check
            decimal maxWeight = 0;
            BaggageType type = DataStore.Baggages[index].Type;
            if (type == BaggageType.Cabin)
                maxWeight = Constants.MaxCabinWeight;
            else if (type == BaggageType.Hold)
                maxWeight = Constants.MaxHoldWeight;
            else if (type == BaggageType.Oversized)
                maxWeight = Constants.MaxOversizedWeight;

            if (newWeight > maxWeight)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  Weight exceeds max limit of {maxWeight}kg. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Copy modify put back
            Baggage b = DataStore.Baggages[index];
            b.WeightKg = newWeight;
            DataStore.Baggages[index] = b;
            CsvHelper.SaveBaggages();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Baggage updated successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        public static void AddUpdateBaggage(Passenger passenger)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter ticket ID: ");
            string ticketID = Console.ReadLine();
            Console.ResetColor();

            // Validate ticket belongs to passenger and is upcoming
            Ticket? ticket = DataStore.Tickets.Values
                .FirstOrDefault(t => t.TicketID == ticketID &&
                                     t.PassengerID == passenger.PassengerID &&
                                     (t.Status == TicketStatus.Confirmed ||
                                      t.Status == TicketStatus.CheckedIn));

            if (ticket == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid ticket ID or ticket is not upcoming. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Show existing baggage for this ticket
            List<Baggage> existing = DataStore.Baggages
                .Where(b => b.TicketID == ticketID)
                .ToList();

            if (existing.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n{"Baggage ID",-12} {"Type",-12} {"Weight (kg)",-14} {"Status"}");
                Console.WriteLine(new string('-', 55));
                foreach (Baggage b in existing)
                    Console.WriteLine($"{b.BaggageID,-12} {b.Type,-12} {b.WeightKg,-14} {b.Status}");
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  No baggage found for this ticket.");
                Console.ResetColor();
            }

            // Menu
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  [1] Add Baggage");
            Console.WriteLine("  [2] Update Baggage");
            Console.WriteLine("  [0] Back");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select an option: ");
            Console.ResetColor();

            switch (Console.ReadLine())
            {
                case "1":
                    AddBaggage(ticketID);
                    break;
                case "2":
                    UpdateBaggage(ticketID);
                    break;
                case "0":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid option. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    break;
            }
        }

        public static void CheckIn(Passenger passenger)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter ticket ID: ");
            string ticketID = Console.ReadLine();
            Console.ResetColor();

            // Validate ticket belongs to passenger and is upcoming
            Ticket? ticket = DataStore.Tickets.Values
                .FirstOrDefault(t => t.TicketID == ticketID &&
                                     t.PassengerID == passenger.PassengerID &&
                                     t.Status == TicketStatus.Confirmed);

            if (ticket == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid ticket ID or ticket is not upcoming. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Allow to change ticket status to Checked-In between 3 hours to 45 minutes from departure
            Flight flight = DataStore.Flights[ticket.Value.FlightNumber];

            DateTime windowOpen = flight.ScheduledDeparture.AddHours(-Constants.CheckInWindowOpen);
            DateTime windowClose = flight.ScheduledDeparture.AddMinutes(-Constants.CheckInWindowClose);

            if (DateTime.Now >= windowOpen && DateTime.Now <= windowClose)
            {
                Ticket t = ticket.Value;
                t.Status = TicketStatus.CheckedIn;
                DataStore.Tickets[t.TicketID] = t;
                CsvHelper.SaveTickets();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Checked in successfully! Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  Check-in window is between {windowOpen:yyyy-MM-dd HH:mm} and {windowClose:yyyy-MM-dd HH:mm}. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        public static decimal TicketPriceCalculator(Flight flight, Airport originAirport, Airport DestinationAirport, TicketSeatClass seatClass, DateTime travelDate, Passenger passenger, string promoCode = null)
        {
            // Base Price
            decimal price = flight.BasePrice;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  {"Base Price",-35} +{price:C}");

            // Distance-Based Multiplier
            float tzDiff = Math.Abs(originAirport.TimeZoneOffset - DestinationAirport.TimeZoneOffset);
            decimal simulatedDistance = (decimal)(tzDiff * 800); // 800km per timezone hour
            decimal distanceCost = simulatedDistance * Constants.PerKmRate;
            price += distanceCost;
            Console.WriteLine($"  {"Distance Surcharge",-35} +{distanceCost:C}");

            // Seat Class Multiplier
            if (seatClass == TicketSeatClass.Business)
            {
                decimal businessCost = price * (Constants.BusinessMultiplier - 1);
                price += businessCost;
                string label = $"Business Class (x{Constants.BusinessMultiplier})";
                Console.WriteLine($"  {label,-35} +{businessCost:C}");
            }
            else
            {
                Console.WriteLine($"  {"Economy Class",-35} +{0:C}");
            }

            // Peak Season Surcharge
            if (Constants.PeakMonths.Contains(travelDate.Month))
            {
                decimal surcharge = price * Constants.PeakSeasonSurcharge;
                price += surcharge;
                string label = $"Peak Season Surcharge ({Constants.PeakSeasonSurcharge:P0})";
                Console.WriteLine($"  {label,-35} +{surcharge:C}");
            }
            else
            {
                Console.WriteLine($"  {"Peak Season Surcharge",-35} Not applicable");
            }

            // Advance Booking Discount
            int daysUntilDeparture = (travelDate.Date - DateTime.Now.Date).Days;
            if (daysUntilDeparture > Constants.AdvanceBookingDays)
            {
                decimal discount = price * Constants.AdvanceBookingDiscount;
                price -= discount;
                string label = $"Advance Booking ({Constants.AdvanceBookingDiscount:P0})";
                Console.WriteLine($"  {label,-35} -{discount:C}");
            }
            else
            {
                Console.WriteLine($"  {"Advance Booking Discount",-35} Not applicable");
            }

            // Loyalty Tier Discount
            decimal tierDiscount = passenger.TierStatus switch
            {
                LoyaltyTier.Silver => 0.05m,
                LoyaltyTier.Gold => 0.10m,
                LoyaltyTier.Platinum => 0.15m,
                _ => 0.00m
            };

            if (tierDiscount > 0)
            {
                decimal tierCost = price * tierDiscount;
                price -= tierCost;
                string label = $"Loyalty Discount ({passenger.TierStatus} {tierDiscount:P0})";
                Console.WriteLine($"  {label,-35} -{tierCost:C}");
            }
            else
            {
                Console.WriteLine($"  {"Loyalty Discount (Bronze)",-35} Not applicable");
            }

            // Promo Code
            if (!string.IsNullOrEmpty(promoCode) && DataStore.Promotions.ContainsKey(promoCode))
            {
                Promotion promo = DataStore.Promotions[promoCode];
                bool validDate = DateTime.Now >= promo.StartDate && DateTime.Now <= promo.EndDate;
                bool validUses = promo.CurrentUseCount < promo.MaxUses;
                bool validClass = promo.ApplicableClass == PromotionApplicableClass.Both ||
                                  (promo.ApplicableClass == PromotionApplicableClass.Economy && seatClass == TicketSeatClass.Economy) ||
                                  (promo.ApplicableClass == PromotionApplicableClass.Business && seatClass == TicketSeatClass.Business);

                if (promo.IsActive && validDate && validUses && validClass)
                {
                    decimal promoDiscount = price * (promo.DiscountPercentage / 100);
                    price -= promoDiscount;
                    string label = $"Promo Code ({promoCode} -{promo.DiscountPercentage}%)";
                    Console.WriteLine($"  {label,-35} -{promoDiscount:C}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  {"Promo Code",-35} Invalid or expired");
                    Console.ResetColor();
                }
            }
            else if (!string.IsNullOrEmpty(promoCode))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  {"Promo Code",-35} Not found");
                Console.ResetColor();
            }

            // Tax
            decimal tax = price * Constants.TaxRate;
            price += tax;
            string taxLabel = $"Tax ({Constants.TaxRate:P0})";
            Console.WriteLine($"  {taxLabel,-35} +{tax:C}");

            // Final Price
            Console.WriteLine(new string('-', 50));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {"FINAL PRICE",-35} {price:C}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 50));

            return price;
        }

        public static void ManageMyTickets(Passenger passenger)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║             Manage My Tickets            ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Upcoming Tickets");
                Console.WriteLine("  [2] Past Tickets");
                Console.WriteLine("  [0] Back to Portal");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.White;
                        // Get passenger's Upcoming tickets
                        Console.WriteLine("\n============= Upcoming Tickets ==============");
                        ShowTickets(passenger, [TicketStatus.Confirmed, TicketStatus.CheckedIn]);
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n  [1] Cancel a ticket");
                        Console.WriteLine("  [2] Add or update baggage");
                        Console.WriteLine("  [3] Check in to a flight");
                        Console.WriteLine("  [0] Back to Manage My Ticket");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("\n  Select an option: ");
                        Console.ResetColor();

                        switch (Console.ReadLine())
                        {
                            case "1":
                                CancelTicket(passenger);
                                break;

                            case "2":
                                AddUpdateBaggage(passenger);
                                break;

                            case "3":
                                CheckIn(passenger);
                                break;

                            case "0":
                                // Back to Manage My Tickets
                                break;

                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Invalid option. Press Enter to back to Manage My Tickets");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                        }

                        break;

                    case "2":
                        Console.ForegroundColor = ConsoleColor.White;
                        // Get passenger's Upcoming tickets
                        Console.WriteLine("\n============= Past Tickets ==============");
                        ShowTickets(passenger, [TicketStatus.Cancelled, TicketStatus.Boarded]);
                        Console.ResetColor();
                        Console.ReadLine();
                        break;

                    case "0":
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void BookSingleTicket(Passenger passenger, string flightNumber, TicketSeatClass seatClass, decimal finalPrice, string promoCode)
        {
            Flight flight = DataStore.Flights[flightNumber];

            // Assign seat
            string seatNumber = AssignSeat(flight, seatClass);
            if (string.IsNullOrEmpty(seatNumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  No seats available. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Calculate loyalty points earned
            int pointsEarned = (int)(finalPrice * Constants.PointsPerDollar);

            // Create ticket
            Ticket newTicket = new Ticket();
            newTicket.TicketID = "TK" + (DataStore.Tickets.Count + 1).ToString("D5");
            newTicket.PassengerID = passenger.PassengerID;
            newTicket.FlightNumber = flightNumber;
            newTicket.SeatClass = seatClass;
            newTicket.SeatNumber = seatNumber;
            newTicket.BookingDate = DateTime.Now;
            newTicket.Status = TicketStatus.Confirmed;
            newTicket.FinalPrice = finalPrice;
            newTicket.LoyaltyPointsEarned = pointsEarned;
            newTicket.PromoCode = promoCode;

            // Decrement available seats
            Flight updatedFlight = flight;
            if (seatClass == TicketSeatClass.Business)
                updatedFlight.AvailableBusinessSeats--;
            else
                updatedFlight.AvailableEconomySeats--;
            DataStore.Flights[flightNumber] = updatedFlight;

            // Update promo use count
            if (!string.IsNullOrEmpty(promoCode) && DataStore.Promotions.ContainsKey(promoCode))
            {
                Promotion promo = DataStore.Promotions[promoCode];
                promo.CurrentUseCount++;
                if (promo.CurrentUseCount >= promo.MaxUses)
                    promo.IsActive = false;
                DataStore.Promotions[promoCode] = promo;
                CsvHelper.SavePromotions();
            }

            // Award loyalty points and update tier
            Passenger updatedPassenger = passenger;
            updatedPassenger.LoyaltyPoints += pointsEarned;
            updatedPassenger.TierStatus = GetUpdatedTier(updatedPassenger.LoyaltyPoints);
            DataStore.Passengers[passenger.PassengerID] = updatedPassenger;

            // Save loyalty log
            LoyaltyLog log = new LoyaltyLog();
            log.LogID = "LL" + (DataStore.LoyaltyLogs.Count + 1).ToString("D5");
            log.PassengerID = passenger.PassengerID;
            log.TicketID = newTicket.TicketID;
            log.PointsChanged = pointsEarned;
            log.Reason = $"Booking {newTicket.TicketID}";
            log.TransactionDate = DateTime.Now;
            DataStore.LoyaltyLogs.Add(log);
            CsvHelper.SaveLoyaltyLogs();

            // Save system log
            SystemLog sysLog = new SystemLog();
            sysLog.LogID = "SL" + (DataStore.SystemLogs.Count + 1).ToString("D5");
            sysLog.Timestamp = DateTime.Now;
            sysLog.UserID = passenger.PassengerID;
            sysLog.UserRole = "Passenger";
            sysLog.ActionType = "Book";
            sysLog.EntityAffected = $"Ticket {newTicket.TicketID}";
            sysLog.Details = $"{passenger.FullName} booked {seatClass} seat on {flightNumber}.";
            DataStore.SystemLogs.Add(sysLog);
            CsvHelper.SaveSystemLogs();

            // Save everything
            DataStore.Tickets[newTicket.TicketID] = newTicket;
            CsvHelper.SaveTickets();
            CsvHelper.SaveFlights();
            CsvHelper.SavePassengers();

            // Booking summary
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║         Booking Confirmed!               ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  {"Ticket ID",-25} {newTicket.TicketID}");
            Console.WriteLine($"  {"Flight",-25} {flightNumber}");
            Console.WriteLine($"  {"From",-25} {flight.OriginAirportCode}");
            Console.WriteLine($"  {"To",-25} {flight.DestinationAirportCode}");
            Console.WriteLine($"  {"Departure",-25} {flight.ScheduledDeparture.ToString("yyyy-MM-dd HH:mm")}");
            Console.WriteLine($"  {"Seat Class",-25} {seatClass}");
            Console.WriteLine($"  {"Seat Number",-25} {seatNumber}");
            Console.WriteLine($"  {"Price Paid",-25} {finalPrice:C}");
            Console.WriteLine($"  {"Points Earned",-25} {pointsEarned}");
            Console.WriteLine($"  {"Total Points",-25} {updatedPassenger.LoyaltyPoints}");
            Console.WriteLine($"  {"Tier Status",-25} {updatedPassenger.TierStatus}");

            if (updatedPassenger.TierStatus != passenger.TierStatus)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  Congratulations! You've been upgraded to {updatedPassenger.TierStatus}!");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        public static void BookTicket(Passenger passenger, string outboundFlightNumber, bool roundTrip, string returnFlightNumber, TicketSeatClass seatClass)
        {
            Flight outbound = DataStore.Flights[outboundFlightNumber];

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║            Booking Summary               ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            // Outbound summary
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  OUTBOUND FLIGHT");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {"Flight",-20} {outbound.FlightNumber}");
            Console.WriteLine($"  {"From",-20} {outbound.OriginAirportCode}");
            Console.WriteLine($"  {"To",-20} {outbound.DestinationAirportCode}");
            Console.WriteLine($"  {"Departure",-20} {outbound.ScheduledDeparture:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"  {"Arrival",-20} {outbound.ScheduledArrival:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"  {"Seat Class",-20} {seatClass}");
            Console.ResetColor();

            // Return summary
            if (roundTrip && !string.IsNullOrEmpty(returnFlightNumber))
            {
                Flight ret = DataStore.Flights[returnFlightNumber];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  RETURN FLIGHT");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"  {"Flight",-20} {ret.FlightNumber}");
                Console.WriteLine($"  {"From",-20} {ret.OriginAirportCode}");
                Console.WriteLine($"  {"To",-20} {ret.DestinationAirportCode}");
                Console.WriteLine($"  {"Departure",-20} {ret.ScheduledDeparture:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"  {"Arrival",-20} {ret.ScheduledArrival:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"  {"Seat Class",-20} {seatClass}");
                Console.ResetColor();
            }

            // Promo code — ask once for both
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Promo code (optional — press Enter to skip): ");
            Console.ResetColor();
            string promoCode = Console.ReadLine();

            // Show price breakdown for outbound
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  OUTBOUND PRICE BREAKDOWN");
            Console.ResetColor();
            Console.WriteLine(new string('-', 50));
            decimal outboundPrice = TicketPriceCalculator(outbound, DataStore.Airports[outbound.OriginAirportCode], DataStore.Airports[outbound.DestinationAirportCode], seatClass, outbound.ScheduledDeparture, passenger, promoCode);

            // Show price breakdown for return
            decimal returnPrice = 0;
            if (roundTrip && !string.IsNullOrEmpty(returnFlightNumber))
            {
                Flight ret = DataStore.Flights[returnFlightNumber];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  RETURN PRICE BREAKDOWN");
                Console.ResetColor();
                Console.WriteLine(new string('-', 50));
                returnPrice = TicketPriceCalculator(ret, DataStore.Airports[ret.OriginAirportCode], DataStore.Airports[ret.DestinationAirportCode], seatClass, ret.ScheduledDeparture, passenger, promoCode);
            }

            // Total
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  {"TOTAL PRICE",-35} {(outboundPrice + returnPrice):C}");
            Console.ResetColor();

            // Single confirmation
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n  Confirm booking? [y/n]: ");
            Console.ResetColor();
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  Booking cancelled. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Book both tickets
            BookSingleTicket(passenger, outboundFlightNumber, seatClass, outboundPrice, promoCode);
            if (roundTrip && !string.IsNullOrEmpty(returnFlightNumber))
                BookSingleTicket(passenger, returnFlightNumber, seatClass, returnPrice, promoCode);
        }

        private static string AssignSeat(Flight flight, TicketSeatClass seatClass)
        {
            // Get already taken seats for this flight
            List<string> takenSeats = DataStore.Tickets.Values
                .Where(t => t.FlightNumber == flight.FlightNumber &&
                            t.SeatClass == seatClass &&
                            t.Status != TicketStatus.Cancelled)
                .Select(t => t.SeatNumber)
                .ToList();

            // Generate seat pool based on class
            int totalSeats = seatClass == TicketSeatClass.Business
                ? flight.AvailableBusinessSeats
                : flight.AvailableEconomySeats;

            string[] suffixes = { "A", "B", "C", "D", "E", "F" };

            for (int row = 1; row <= totalSeats; row++)
            {
                foreach (string suffix in suffixes)
                {
                    string seat = $"{row}{suffix}";
                    if (!takenSeats.Contains(seat))
                        return seat;
                }
            }

            return ""; // no seats available
        }

        public static LoyaltyTier GetUpdatedTier(int points)
        {
            if (points >= Constants.PlatinumThreshold) return LoyaltyTier.Platinum;
            if (points >= Constants.GoldThreshold) return LoyaltyTier.Gold;
            if (points >= Constants.SilverThreshold) return LoyaltyTier.Silver;
            return LoyaltyTier.Bronze;
        }
    }

    static class PassengerService
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║           Passenger Management           ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Add New Passenger");
                Console.WriteLine("  [2] View All Passengers");
                Console.WriteLine("  [3] Update Passenger");
                Console.WriteLine("  [4] Delete Passenger");
                Console.WriteLine("  [5] Search Passengers");
                Console.WriteLine("  [6] View Booking History");
                Console.WriteLine("  [7] Adjust Loyalty Points");
                Console.WriteLine("  [8] Loyalty Tier Report");
                Console.WriteLine("  [0] Back to Admin Dashboard");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        AddPassenger();
                        break;

                    case "2":
                        ViewAllPassengers();
                        break;

                    case "3":
                        UpdatePassenger();
                        break;

                    case "4":
                        DeletePassenger();
                        break;

                    case "5":
                        SearchPassengers();
                        break;

                    case "6":
                        ViewBookingHistory();
                        break;

                    case "7":
                        AdjustLoyaltyPoints();
                        break;

                    case "8":
                        LoyaltyTierReport();
                        break;

                    case "0":
                        return;
                }
            }
        }

        private static string GetPassengerID()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter Passenger ID (0 to cancel): ");
            Console.ResetColor();
            return Console.ReadLine();
        }

        public static void AddPassenger()
        {
            AuthService.Register();
        }

        public static void ViewAllPassengers()
        {
            List<Passenger> passengers = DataStore.Passengers.Values.ToList();

            if (passengers.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  No passengers found.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            int totalPages = (int)Math.Ceiling(passengers.Count / (double)Constants.PageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n  ---------- VIEW ALL PASSENGERS ----------");

                List<Passenger> pageItems = passengers
                    .Skip((currentPage - 1) * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .ToList();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    $"\n  {"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} " +
                    $"{"Nationality",-15} {"Tier",-10} {"Points",-8}"
                );
                Console.WriteLine(new string('-', 120));

                foreach (Passenger p in pageItems)
                {
                    Console.WriteLine(
                        $"   {p.PassengerID,-10}" +
                        $" {p.FullName,-25}" +
                        $" {p.Email,-30}" +
                        $" {p.PassportNumber,-15}" +
                        $" {p.Nationality,-15}" +
                        $" {p.TierStatus,-10}" +
                        $" {p.LoyaltyPoints,-8}"
                    );
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  Page {currentPage} of {totalPages}  |  Total: {passengers.Count} passengers");
                Console.WriteLine("  [N] Next   [P] Previous   [0] Back");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Choice: ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (input == "0") return;
                else if (input == "N" && currentPage < totalPages) currentPage++;
                else if (input == "P" && currentPage > 1) currentPage--;
            }
        }

        public static void UpdatePassenger()
        {
            while (true)
            {
                string PassengerID = GetPassengerID();

                if (PassengerID == "0")
                    return;

                if (!DataStore.Passengers.ContainsKey(PassengerID))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid Passenger ID. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Passenger passenger = DataStore.Passengers[PassengerID];

                while (true)
                {
                    Console.Clear();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  Update Passenger - {passenger.FullName}: {passenger.PassengerID}");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  [1] Full Name ({passenger.FullName})");
                    Console.WriteLine($"  [2] Email ({passenger.Email})");
                    Console.WriteLine($"  [3] Passport Number ({passenger.PassportNumber})");
                    Console.WriteLine($"  [4] Nationality ({passenger.Nationality})");
                    Console.WriteLine($"  [5] PhoneNumber ({passenger.Phone})");
                    Console.WriteLine($"  [6] Date of Birth ({passenger.DateOfBirth})");
                    Console.WriteLine("  [0] Back");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Select a field to update: ");
                    Console.ResetColor();

                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.Write("  New Full Name: ");
                            string fullName = Console.ReadLine().Trim().ToUpper();
                            if (string.IsNullOrEmpty(fullName))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Full Name cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.FullName = fullName;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Full Name' updated.");
                            break;

                        case "2":
                            Console.Write("  New Email: ");
                            string email = Console.ReadLine().Trim().ToUpper();

                            if (string.IsNullOrEmpty(email))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Email cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            if (DataStore.Passengers.Values.Any(p => p.Email == email))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("  Email is already used. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.Email = email;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Email' updated.");
                            break;

                        case "3":
                            Console.Write("  New Passport Number: ");
                            string passportNum = Console.ReadLine().Trim().ToUpper();

                            if (string.IsNullOrEmpty(passportNum))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Passport Number cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            if (DataStore.Passengers.Values.Any(p => p.PassportNumber == passportNum))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("  Passport Number is already used. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.PassportNumber = passportNum;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Passport Number' updated.");
                            break;

                        case "4":
                            Console.Write("  New Nationality: ");
                            string nationality = Console.ReadLine().Trim().ToUpper();
                            
                            if (string.IsNullOrEmpty(nationality))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Nationality cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.Nationality = nationality;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Nationality' updated.");
                            break;

                        case "5":
                            Console.Write("  New Phone Number: ");
                            string phoneNum = Console.ReadLine().Trim().ToUpper();

                            if (string.IsNullOrEmpty(phoneNum))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Phone Number cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            if (DataStore.Passengers.Values.Any(p => p.Phone == phoneNum))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("  Phone Number is already used. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.Phone = phoneNum;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Phone Number' updated.");
                            break;

                        case "6":
                            Console.Write("\n  New Date of Birth (yyyy-MM-dd): ");
                            string dobInput = Console.ReadLine();
                            if (string.IsNullOrEmpty(dobInput))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Date of birth cannot be empty. Press Enter.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }
                            // Parse safely
                            if (!DateTime.TryParse(dobInput, out DateTime DOB))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n  Invalid date format. Use yyyy-MM-dd. Press Enter to try again.");
                                Console.ResetColor();
                                Console.ReadLine();
                                break;
                            }

                            passenger.DateOfBirth = DOB;
                            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "UPDATE", "Passenger", $"Passenger {PassengerID} field 'Date of Birth' updated.");
                            break;

                        case "0":
                            CsvHelper.SavePassengers();
                            return;
                    }

                    DataStore.Passengers[passenger.PassengerID] = passenger;
                }
            }
        }

        public static void DeletePassengerAndTickets(string PassengerID)
        {
            // Cancel all tickets
            List<Ticket> tickets = DataStore.Tickets.Values
                .Where(t => t.PassengerID == PassengerID &&
                            t.Status != TicketStatus.Cancelled)
                .ToList();

            foreach (Ticket ticket in tickets)
            {
                // Cancel ticket — copy modify put back
                Ticket updated = ticket;
                updated.Status = TicketStatus.Cancelled;
                DataStore.Tickets[updated.TicketID] = updated;

                // Update Flight available seats
                Flight f = DataStore.Flights[ticket.FlightNumber];
                if (ticket.SeatClass == TicketSeatClass.Economy)
                    f.AvailableEconomySeats++;
                else
                    f.AvailableBusinessSeats++;
                DataStore.Flights[ticket.FlightNumber] = f;

                // Cancel related baggage
                for (int i = 0; i < DataStore.Baggages.Count; i++)
                {
                    if (DataStore.Baggages[i].TicketID == ticket.TicketID &&
                        DataStore.Baggages[i].Status != BaggageStatus.Delivered)
                    {
                        Baggage b = DataStore.Baggages[i];
                        b.Status = BaggageStatus.Delivered;
                        DataStore.Baggages[i] = b;
                    }
                }
            }

            // Delete Passenger
            DataStore.Passengers.Remove(PassengerID);

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "DELETE", "Passenger", $"Passenger {PassengerID} deleted with {tickets.Count} tickets cancelled.");

            // Save everything
            CsvHelper.SaveFlights();
            CsvHelper.SaveTickets();
            CsvHelper.SaveBaggages();
            CsvHelper.SavePassengers();
        } 

        public static void DeletePassenger()
        {
            while (true)
            {
                string PassengerID = GetPassengerID();

                if (PassengerID == "0")
                    return;

                if (!DataStore.Passengers.ContainsKey(PassengerID))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid Passenger ID. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                bool hasTickets = DataStore.Tickets.Values
                    .Any(t => t.PassengerID == PassengerID &&
                              t.Status != TicketStatus.Cancelled);

                if (hasTickets)
                {
                    // 3. Warn admin and offer cancel instead
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n  This passenger has confirmed tickets.");
                    Console.WriteLine("  [1] Delete passenger and all related tickets");
                    Console.WriteLine("  [Enter] Abort");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Select option: ");
                    Console.ResetColor();

                    if (Console.ReadLine()?.Trim() != "1")
                        continue;
                }

                DeletePassengerAndTickets(PassengerID);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Passenger and all related tickets deleted. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        public static void SearchPassengers()
        {
            List<Passenger> passengers;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                        Search Passenger                      ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  By:");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  [1] Full Name");
                Console.WriteLine("  [2] Email");
                Console.WriteLine("  [3] Passport Number");
                Console.WriteLine("  [4] Nationality");
                Console.WriteLine("  [0] Back");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Write("  Enter Full Name: ");
                        string fullName = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(fullName))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  Full Name cannot be empty. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        fullName = fullName.ToUpper();

                        passengers = DataStore.Passengers.Values
                            .Where(p => p.FullName.ToUpper().Contains(fullName))
                            .ToList();

                        if (passengers.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  No passenger found. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(
                            $"\n{"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} {"Nationality",-15} {"Tier",-10} {"Points",-8}"
                        );
                        Console.WriteLine(new string('-', 120));

                        foreach (Passenger p in passengers)
                        {
                            Console.WriteLine(
                                $"{p.PassengerID,-10} " +
                                $"{p.FullName,-25} " +
                                $"{p.Email,-30} " +
                                $"{p.PassportNumber,-15} " +
                                $"{p.Nationality,-15} " +
                                $"{p.TierStatus,-10} " +
                                $"{p.LoyaltyPoints,-8}"
                            );
                        }

                        Console.WriteLine("\n  Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.Write("  Enter Email: ");
                        string email = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(email))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  Email cannot be empty. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        email = email.ToUpper();

                        passengers = DataStore.Passengers.Values
                            .Where(p => p.Email.ToUpper() == email)
                            .ToList();

                        if (passengers.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  No passenger found. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(
                            $"\n{"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} {"Nationality",-15} {"Tier",-10} {"Points",-8}"
                        );
                        Console.WriteLine(new string('-', 120));

                        foreach (Passenger p in passengers)
                        {
                            Console.WriteLine(
                                $"{p.PassengerID,-10} " +
                                $"{p.FullName,-25} " +
                                $"{p.Email,-30} " +
                                $"{p.PassportNumber,-15} " +
                                $"{p.Nationality,-15} " +
                                $"{p.TierStatus,-10} " +
                                $"{p.LoyaltyPoints,-8}"
                            );
                        }

                        Console.WriteLine("\n  Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "3":
                        Console.Write("  Enter Passport Number: ");
                        string passNum = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(passNum))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  Passport Number cannot be empty. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        passengers = DataStore.Passengers.Values
                            .Where(p => p.PassportNumber == passNum)
                            .ToList();

                        if (passengers.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  No passenger found. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(
                            $"\n{"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} {"Nationality",-15} {"Tier",-10} {"Points",-8}"
                        );
                        Console.WriteLine(new string('-', 120));

                        foreach (Passenger p in passengers)
                        {
                            Console.WriteLine(
                                $"{p.PassengerID,-10} " +
                                $"{p.FullName,-25} " +
                                $"{p.Email,-30} " +
                                $"{p.PassportNumber,-15} " +
                                $"{p.Nationality,-15} " +
                                $"{p.TierStatus,-10} " +
                                $"{p.LoyaltyPoints,-8}"
                            );
                        }

                        Console.WriteLine("\n  Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "4":
                        Console.Write("  Enter Email: ");
                        string Nationality = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(Nationality))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  Nationality cannot be empty. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        Nationality = Nationality.ToUpper();

                        passengers = DataStore.Passengers.Values
                            .Where(p => p.Nationality.ToUpper() == Nationality)
                            .ToList();

                        if (passengers.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n  No passenger found. Press Enter.");
                            Console.ResetColor();
                            Console.ReadLine();
                            break;
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(
                            $"\n{"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} {"Nationality",-15} {"Tier",-10} {"Points",-8}"
                        );
                        Console.WriteLine(new string('-', 120));

                        foreach (Passenger p in passengers)
                        {
                            Console.WriteLine(
                                $"{p.PassengerID,-10} " +
                                $"{p.FullName,-25} " +
                                $"{p.Email,-30} " +
                                $"{p.PassportNumber,-15} " +
                                $"{p.Nationality,-15} " +
                                $"{p.TierStatus,-10} " +
                                $"{p.LoyaltyPoints,-8}"
                            );
                        }

                        Console.WriteLine("\n  Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "0":
                        return;
                }
            }
        }

        public static void ViewBookingHistory()
        {
            while (true)
            {
                string PassengerID = GetPassengerID();

                if (PassengerID == "0")
                    return;

                if (!DataStore.Passengers.ContainsKey(PassengerID))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid Passenger ID. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Passenger passenger = DataStore.Passengers[PassengerID];

                Console.ForegroundColor = ConsoleColor.White;
                // Get passenger's booking history
                Console.WriteLine("\n============= Booking History ==============");
                Console.WriteLine($"\n{"Ticket ID",-10} {"Flight",-8} {"From",-6} {"To",-6} {"Departure",-18} {"Class",-10} {"Status"}");
                Console.WriteLine(new string('-', 75));
                List<Ticket> bookingHistory = DataStore.Tickets.Values
                    .Where(t => t.PassengerID == passenger.PassengerID)
                    .ToList();
                foreach (Ticket t in bookingHistory)
                {
                    Flight f = DataStore.Flights[t.FlightNumber];
                    Console.WriteLine($"{t.TicketID,-10} {f.FlightNumber,-8} {f.OriginAirportCode,-6} {f.DestinationAirportCode,-6} {f.ScheduledDeparture.ToString("yyyy-MM-dd HH:mm"),-18} {t.SeatClass,-10} {t.Status}");
                }
            }
        }

        public static void AdjustLoyaltyPoints()
        {
            while (true)
            {
                string passengerID = GetPassengerID();

                if (passengerID == "0")
                    return;

                if (!DataStore.Passengers.ContainsKey(passengerID))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid Passenger ID. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Passenger passenger = DataStore.Passengers[passengerID];

                // Show current state
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n  Passenger : {passenger.FullName}");
                Console.WriteLine($"  Current Points : {passenger.LoyaltyPoints}");
                Console.WriteLine($"  Current Tier   : {passenger.TierStatus}");
                Console.ResetColor();

                // Adjustment amount (positive or negative)
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter adjustment amount (+/-): ");
                Console.ResetColor();
                if (!int.TryParse(Console.ReadLine(), out int adjustment))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid amount. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                int newPoints = passenger.LoyaltyPoints + adjustment;

                // Floor at 0
                if (newPoints < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n  Adjustment would result in negative points ({newPoints}). Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Reason note
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Reason: ");
                Console.ResetColor();
                string reason = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(reason))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Reason is required. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Apply
                int oldPoints = passenger.LoyaltyPoints;
                LoyaltyTier oldTier = passenger.TierStatus;

                passenger.LoyaltyPoints = newPoints;
                passenger.TierStatus = TicketService.GetUpdatedTier(newPoints);
                DataStore.Passengers[passengerID] = passenger;

                // Loyalty log entry
                LoyaltyLog log = new LoyaltyLog
                {
                    LogID = $"LL{DataStore.LoyaltyLogs.Count + 1:D4}",
                    PassengerID = passengerID,
                    PointsChanged = adjustment,
                    Reason = "By Admin: " + reason,
                    TransactionDate = DateTime.Now
                };
                DataStore.LoyaltyLogs.Add(log);

                CsvHelper.SavePassengers();
                CsvHelper.SaveLoyaltyLogs();

                // System log
                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                    "ADJUST", "Passenger",
                    $"Passenger {passengerID} points adjusted by {adjustment} ({oldPoints} -> {newPoints}). " +
                    $"Tier: {oldTier} -> {passenger.TierStatus}. Reason: {reason}.");

                // Confirm
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  Points updated: {oldPoints} => {newPoints}  |  Tier: {oldTier} => {passenger.TierStatus}");
                Console.WriteLine("  Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        public static void ExportLoyaltyTierReport(LoyaltyTier tier)
        {
            var report = new StringBuilder();

            List<Passenger> passengers = DataStore.Passengers.Values
                .Where(p => p.TierStatus == tier)
                .ToList();

            report.AppendLine("========================================");
            report.AppendLine("LOYALTY TIER REPORT - Passengers Details");
            report.AppendLine("========================================");

            report.AppendLine(
                $"\n{"ID",-10} {"Full Name",-25} {"Email",-30} {"Passport",-15} {"Nationality",-15} {"Tier",-10} {"Points",-8}"
            );
            Console.WriteLine(new string('-', 120));
            foreach (Passenger p in passengers)
            {
                
                report.AppendLine(
                    $"{p.PassengerID,-10} " +
                    $"{p.FullName,-25} " +
                    $"{p.Email,-30} " +
                    $"{p.PassportNumber,-15} " +
                    $"{p.Nationality,-15} " +
                    $"{p.TierStatus,-10} " +
                    $"{p.LoyaltyPoints,-8}"
                );
            }
            report.AppendLine();
            report.AppendLine($"Total Passengers: {passengers.Count}");

            // Save Report
            Directory.CreateDirectory(Constants.ReportsFolder);

            string filePath = Path.Combine(
                Constants.ReportsFolder,
                $"LoyaltyTierReport_{tier}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            );
            File.WriteAllText(filePath, report.ToString());

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole, "EXPORT", "Report", $"Loyalty Tier report exported for {tier}.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  The report has been saved in {filePath}. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        public static void LoyaltyTierReport()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter Loyalty Tier  (0 to cancel): ");
                Console.ResetColor();
                string tier = Console.ReadLine()?.Trim().ToUpper();

                switch (tier)
                {
                    case "BRONZE":
                        ExportLoyaltyTierReport(LoyaltyTier.Bronze);
                        break;
                    case "SILVER":
                        ExportLoyaltyTierReport(LoyaltyTier.Silver);
                        break;
                    case "GOLD":
                        ExportLoyaltyTierReport(LoyaltyTier.Gold);
                        break;
                    case "PLATINUM":
                        ExportLoyaltyTierReport(LoyaltyTier.Platinum);
                        break;
                    case "0":
                        return;
                    
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid Loyalty Tier. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;
                }
            }
        }
    }

    static class SystemLogService
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║          SYSTEM LOGS MANAGEMENT          ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] View All Logs");
                Console.WriteLine("  [2] Filter Logs by Date Range");
                Console.WriteLine("  [3] Filter Logs by User");
                Console.WriteLine("  [4] Filter Logs by Action Type");
                Console.WriteLine("  [5] Export Logs to File");
                Console.WriteLine("  [0] Back to Admin Menu");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        ViewAllLogs();
                        break;
                    case "2":
                        FilterByDateRange();
                        break;
                    case "3":
                        FilterByUser();
                        break;
                    case "4":
                        FilterByActionType();
                        break;
                    case "5":
                        ExportLogs();
                        break;
                    case "0":
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void ViewAllLogs()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║              ALL SYSTEM LOGS             ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            if (DataStore.SystemLogs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  No logs available.");
                Console.ResetColor();
            }
            else
            {
                DisplayLogsWithPagination(DataStore.SystemLogs);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void FilterByDateRange()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║         FILTER LOGS BY DATE RANGE        ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter start date (yyyy-MM-dd): ");
            Console.ResetColor();
            string startDateStr = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  Enter end date (yyyy-MM-dd): ");
            Console.ResetColor();
            string endDateStr = Console.ReadLine();

            if (!DateTime.TryParse(startDateStr, out DateTime startDate) || !DateTime.TryParse(endDateStr, out DateTime endDate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid date format. Press Enter to try again.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Set end date to end of day
            endDate = endDate.AddDays(1).AddSeconds(-1);

            List<SystemLog> filteredLogs = DataStore.SystemLogs
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .ToList();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"║    LOGS FROM {startDate:yyyy-MM-dd} TO {endDate:yyyy-MM-dd}     ║");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
            Console.ResetColor();

            if (filteredLogs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  No logs found for the specified date range.");
                Console.ResetColor();
            }
            else
            {
                DisplayLogsWithPagination(filteredLogs);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void FilterByUser()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║          FILTER LOGS BY USER             ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter User ID: ");
            Console.ResetColor();
            string userID = Console.ReadLine().Trim();

            List<SystemLog> filteredLogs = DataStore.SystemLogs
                .Where(l => l.UserID.Equals(userID, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"║       LOGS FOR USER {userID,-20}║");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
            Console.ResetColor();

            if (filteredLogs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"\n  No logs found for user {userID}.");
                Console.ResetColor();
            }
            else
            {
                DisplayLogsWithPagination(filteredLogs);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void FilterByActionType()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║        FILTER LOGS BY ACTION TYPE        ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            // Display unique action types
            var actionTypes = DataStore.SystemLogs
                .Select(l => l.ActionType)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            if (actionTypes.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n  No action types found.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Available Action Types:");
            Console.ResetColor();
            for (int i = 0; i < actionTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {actionTypes[i]}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select an action type (or enter custom): ");
            Console.ResetColor();
            string input = Console.ReadLine().Trim();

            string selectedActionType = "";
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= actionTypes.Count)
            {
                selectedActionType = actionTypes[choice - 1];
            }
            else
            {
                selectedActionType = input;
            }

            List<SystemLog> filteredLogs = DataStore.SystemLogs
                .Where(l => l.ActionType.Equals(selectedActionType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔══════════════════════════════════════════╗");
            Console.WriteLine($"║     LOGS FOR ACTION: {selectedActionType,-16}║");
            Console.WriteLine($"╚══════════════════════════════════════════╝");
            Console.ResetColor();

            if (filteredLogs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"\n  No logs found for action type '{selectedActionType}'.");
                Console.ResetColor();
            }
            else
            {
                DisplayLogsWithPagination(filteredLogs);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void ExportLogs()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║            EXPORT SYSTEM LOGS            ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  [1] Export All Logs");
            Console.WriteLine("  [2] Export Filtered by Date Range");
            Console.WriteLine("  [3] Export Filtered by User");
            Console.WriteLine("  [4] Export Filtered by Action Type");
            Console.WriteLine("  [0] Back to System Logs Menu");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select an option: ");
            Console.ResetColor();

            List<SystemLog> logsToExport = new List<SystemLog>();
            string exportName = "";

            switch (Console.ReadLine())
            {
                case "1":
                    logsToExport = DataStore.SystemLogs;
                    exportName = "SystemLogs_All";
                    break;
                case "2":
                    logsToExport = GetLogsFromDateRangeInput();
                    exportName = "SystemLogs_DateRange";
                    break;
                case "3":
                    logsToExport = GetLogsFromUserInput();
                    exportName = "SystemLogs_User";
                    break;
                case "4":
                    logsToExport = GetLogsFromActionTypeInput();
                    exportName = "SystemLogs_ActionType";
                    break;
                case "0":
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
            }

            if (logsToExport.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  No logs to export. Press Enter to continue.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Create export file
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{exportName}_{timestamp}.txt";

            Directory.CreateDirectory(Constants.ReportsFolder);
            string filePath = Path.Combine(Constants.ReportsFolder, fileName);

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════╗");
                    sw.WriteLine("║                           SYSTEM LOGS EXPORT REPORT                            ║");
                    sw.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════╝");
                    sw.WriteLine();
                    sw.WriteLine($"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sw.WriteLine($"Total Logs: {logsToExport.Count}");
                    sw.WriteLine();
                    sw.WriteLine(new string('─', 140));
                    sw.WriteLine($"{"LogID",-8} {"Timestamp",-25} {"UserID",-10} {"Role",-12} {"Action",-15} {"Entity",-20} {"Details",-50}");
                    sw.WriteLine(new string('─', 140));

                    foreach (SystemLog log in logsToExport)
                    {
                        sw.WriteLine($"{log.LogID,-8} {log.Timestamp:yyyy-MM-dd HH:mm:ss,-25} {log.UserID,-10} {log.UserRole,-12} {log.ActionType,-15} {log.EntityAffected,-20} {log.Details,-50}");
                    }

                    sw.WriteLine(new string('─', 140));
                    sw.WriteLine($"End of Report");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  ✓ Logs exported successfully to: {fileName}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✗ Error exporting logs: {ex.Message}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static List<SystemLog> GetLogsFromDateRangeInput()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter start date (yyyy-MM-dd): ");
            Console.ResetColor();
            string startDateStr = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  Enter end date (yyyy-MM-dd): ");
            Console.ResetColor();
            string endDateStr = Console.ReadLine();

            if (!DateTime.TryParse(startDateStr, out DateTime startDate) || !DateTime.TryParse(endDateStr, out DateTime endDate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid date format.");
                Console.ResetColor();
                return new List<SystemLog>();
            }

            endDate = endDate.AddDays(1).AddSeconds(-1);

            return DataStore.SystemLogs
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .ToList();
        }

        private static List<SystemLog> GetLogsFromUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter User ID: ");
            Console.ResetColor();
            string userID = Console.ReadLine().Trim();

            return DataStore.SystemLogs
                .Where(l => l.UserID.Equals(userID, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static List<SystemLog> GetLogsFromActionTypeInput()
        {
            var actionTypes = DataStore.SystemLogs
                .Select(l => l.ActionType)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            if (actionTypes.Count == 0)
                return new List<SystemLog>();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Available Action Types:");
            Console.ResetColor();
            for (int i = 0; i < actionTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {actionTypes[i]}");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select an action type (or enter custom): ");
            Console.ResetColor();
            string input = Console.ReadLine().Trim();

            string selectedActionType = "";
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= actionTypes.Count)
            {
                selectedActionType = actionTypes[choice - 1];
            }
            else
            {
                selectedActionType = input;
            }

            return DataStore.SystemLogs
                .Where(l => l.ActionType.Equals(selectedActionType, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private static void DisplayLogsWithPagination(List<SystemLog> logs)
        {
            int pageSize = 5;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)logs.Count / pageSize);

            while (currentPage < totalPages)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"╔══════════════════════════════════════════╗");
                Console.WriteLine($"║     SYSTEM LOGS (Page {currentPage + 1} of {totalPages})          ║");
                Console.WriteLine($"╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n{"LogID",-8} {"Timestamp",-20} {"UserID",-10} {"Action",-15} {"Entity",-20}");
                Console.WriteLine(new string('-', 85));
                Console.ResetColor();

                int startIdx = currentPage * pageSize;
                int endIdx = Math.Min(startIdx + pageSize, logs.Count);

                for (int i = startIdx; i < endIdx; i++)
                {
                    SystemLog log = logs[i];
                    Console.WriteLine($"{log.LogID,-8} {log.Timestamp:yyyy-MM-dd HH:mm,-20} {log.UserID,-10} {log.ActionType,-15} {log.EntityAffected,-20}");
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"\n({endIdx} of {logs.Count} entries shown)");

                if (totalPages > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  [P] Previous Page");
                    Console.WriteLine($"  [N] Next Page");
                    Console.WriteLine($"  [D] View Details");
                    Console.WriteLine($"  [Q] Quit");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("\n  Select an option: ");
                    Console.ResetColor();

                    string choice = Console.ReadLine().ToUpper();
                    switch (choice)
                    {
                        case "P":
                            if (currentPage > 0)
                                currentPage--;
                            break;
                        case "N":
                            if (currentPage < totalPages - 1)
                                currentPage++;
                            break;
                        case "D":
                            ViewLogDetails(logs, startIdx, endIdx);
                            break;
                        case "Q":
                            return;
                        default:
                            break;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private static void ViewLogDetails(List<SystemLog> logs, int startIdx, int endIdx)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter LogID to view details (or press Enter to go back): ");
            Console.ResetColor();
            string logID = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(logID))
                return;

            SystemLog selectedLog = logs.FirstOrDefault(l => l.LogID == logID);
            if (selectedLog.LogID == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Log not found. Press Enter to continue.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║           LOG DETAILS                    ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  Log ID:           {selectedLog.LogID}");
            Console.WriteLine($"  Timestamp:        {selectedLog.Timestamp:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  User ID:          {selectedLog.UserID}");
            Console.WriteLine($"  User Role:        {selectedLog.UserRole}");
            Console.WriteLine($"  Action Type:      {selectedLog.ActionType}");
            Console.WriteLine($"  Entity Affected:  {selectedLog.EntityAffected}");
            Console.WriteLine($"  Details:          {selectedLog.Details}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }
    }

    static class BaggageService
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║             BAGGAGE OVERSIGHT            ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] View All Baggage");
                Console.WriteLine("  [2] Update baggage status");
                Console.WriteLine("  [3] Generate a Lost Baggage Report");
                Console.WriteLine("  [0] Back to Admin Menu");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        ViewAllBaggage();
                        break;
                    case "2":
                        UpdateBaggage();
                        break;
                    case "3":
                        LostBaggageReport();
                        break;
                    case "0":
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void ViewAllBaggage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║              VIEW ALL BAGGAGE            ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            while (true)
            {
                string flightNumber = FlightService.GetFlightNumber();

                if (flightNumber == "0")
                    return;

                if (string.IsNullOrEmpty(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight number cannot be empty.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight does not exist. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                List<Baggage> flightBaggages = DataStore.Baggages
                    .Where(b => DataStore.Tickets.ContainsKey(b.TicketID) &&
                    DataStore.Tickets[b.TicketID].FlightNumber == flightNumber)
                    .ToList();

                if (flightBaggages.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight has no baggage. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n{"Baggage ID",-12} {"Type",-12} {"Weight (kg)",-14} {"Status"}");
                Console.WriteLine(new string('-', 55));
                foreach (Baggage b in flightBaggages)
                    Console.WriteLine($"{b.BaggageID,-12} {b.Type,-12} {b.WeightKg,-14} {b.Status}");
                Console.WriteLine(new string('-', 55));
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void UpdateBaggage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║               UPDATE BAGGAGE             ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            while (true)
            {
                string flightNumber = FlightService.GetFlightNumber();

                if (flightNumber == "0")
                    return;

                if (string.IsNullOrEmpty(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight number cannot be empty.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                if (!DataStore.Flights.ContainsKey(flightNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight does not exist. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                List<Baggage> flightBaggages = DataStore.Baggages
                    .Where(b => DataStore.Tickets.ContainsKey(b.TicketID) &&
                    DataStore.Tickets[b.TicketID].FlightNumber == flightNumber)
                    .ToList();

                if (flightBaggages.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Flight has no baggage. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Checked-In");
                Console.WriteLine("  [2] Loaded");
                Console.WriteLine("  [3] Lost");
                Console.WriteLine("  [4] Delivered");
                Console.WriteLine("  [0] Cancel the update");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select new status: ");
                Console.ResetColor();

                BaggageStatus newStatus;

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": newStatus = BaggageStatus.CheckedIn; break;
                    case "2": newStatus = BaggageStatus.Loaded; break;
                    case "3": newStatus = BaggageStatus.Lost; break;
                    case "4": newStatus = BaggageStatus.Delivered; break;
                    case "0": return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        continue;   // <-- skip the update entirely
                }

                for (int i = 0; i < DataStore.Baggages.Count; i++)
                {
                    if (flightBaggages.Any(b => b.BaggageID == DataStore.Baggages[i].BaggageID))
                    {
                        Baggage b = DataStore.Baggages[i];
                        b.Status = newStatus;
                        DataStore.Baggages[i] = b;
                    }
                }

                CsvHelper.SaveBaggages();

                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                    "UPDATE", "Baggage",
                    $"Flight {flightNumber} — {flightBaggages.Count} baggage item(s) updated to {newStatus}.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  {flightBaggages.Count} baggage item(s) updated to {newStatus}. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        static void LostBaggageReport()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║            LOST BAGGAGE REPORT           ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            List<Baggage> lostBaggages = DataStore.Baggages
                .Where(b => b.Status == BaggageStatus.Lost)
                .ToList();
            
            if (lostBaggages.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  No lost baggage found. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            int totalPages = (int)Math.Ceiling(lostBaggages.Count / (double)Constants.PageSize);
            int currentPage = 1;

            while (true)
            {
                List<Baggage> pageItems = lostBaggages
                    .Skip((currentPage - 1) * Constants.PageSize)
                    .Take(Constants.PageSize)
                    .ToList();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    $"\n  {"BaggageID",-12} {"TicketID",-12} {"PassengerID",-14} {"Flight",-10} {"Type",-12} {"Weight",-8} {"Status",-10}"
                );
                Console.WriteLine(new string('-', 85));
                Console.ResetColor();

                foreach (Baggage b in pageItems)
                {
                    string passengerID = DataStore.Tickets.ContainsKey(b.TicketID)
                        ? DataStore.Tickets[b.TicketID].PassengerID
                        : "-";

                    string flightNumber = DataStore.Tickets.ContainsKey(b.TicketID)
                        ? DataStore.Tickets[b.TicketID].FlightNumber
                        : "-";

                    Console.WriteLine(
                        $"  {b.BaggageID,-12}" +
                        $" {b.TicketID,-12}" +
                        $" {passengerID,-14}" +
                        $" {flightNumber,-10}" +
                        $" {b.Type,-12}" +
                        $" {b.WeightKg,-8}" +
                        $" {b.Status,-10}"
                    );
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  Page {currentPage} of {totalPages}  |  Total Lost: {lostBaggages.Count}");
                Console.WriteLine("  [N] Next   [P] Previous   [E] Export   [0] Back");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Choice: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (input == "0") return;
                else if (input == "N" && currentPage < totalPages) currentPage++;
                else if (input == "P" && currentPage > 1) currentPage--;
                else if (input == "E") { ExportLostBaggageReport(lostBaggages); return; }
            }
        }

        static void ExportLostBaggageReport(List<Baggage> lostBaggages)
        {
            string fileName = $"lost_baggage_{DateTime.Now:yyyy-MM-dd_HHmm}.txt";
            string filePath = Path.Combine(Constants.ReportsFolder, fileName);

            Directory.CreateDirectory(Constants.ReportsFolder);

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("╔══════════════════════════════════════════╗");
                writer.WriteLine("║            LOST BAGGAGE REPORT           ║");
                writer.WriteLine("╚══════════════════════════════════════════╝");
                writer.WriteLine($"  Generated   : {DateTime.Now:yyyy-MM-dd HH:mm}");
                writer.WriteLine($"  Exported By : {Session.CurrentUserID} ({Session.CurrentUserRole})");
                writer.WriteLine($"  Total Lost  : {lostBaggages.Count}");
                writer.WriteLine(new string('-', 85));
                writer.WriteLine(
                    $"  {"BaggageID",-12} {"TicketID",-12} {"PassengerID",-14} {"Flight",-10} {"Type",-12} {"Weight",-8} {"Status",-10}"
                );
                writer.WriteLine(new string('-', 85));

                foreach (Baggage b in lostBaggages)
                {
                    string passengerID = DataStore.Tickets.ContainsKey(b.TicketID)
                        ? DataStore.Tickets[b.TicketID].PassengerID
                        : "-";

                    string flightNumber = DataStore.Tickets.ContainsKey(b.TicketID)
                        ? DataStore.Tickets[b.TicketID].FlightNumber
                        : "-";

                    writer.WriteLine(
                        $"  {b.BaggageID,-12}" +
                        $" {b.TicketID,-12}" +
                        $" {passengerID,-14}" +
                        $" {flightNumber,-10}" +
                        $" {b.Type,-12}" +
                        $" {b.WeightKg,-8}" +
                        $" {b.Status,-10}"
                    );
                }
            }

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                "EXPORT", "Baggage",
                $"Lost baggage report exported to {fileName}. {lostBaggages.Count} record(s).");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Report exported to: {filePath}. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }
    }

    static class PromotionService
    {
        public static void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║            PROMOTION MANAGEMENT          ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Add New Promtion");
                Console.WriteLine("  [2] View All Promotions");
                Console.WriteLine("  [3] Update Promotion");
                Console.WriteLine("  [4] Delete Promotion");
                Console.WriteLine("  [5] View Usage Summary");
                Console.WriteLine("  [0] Back to Admin Menu");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        AddPromotion();
                        break;
                    case "2":
                        ViewAllPromotions();
                        break;
                    case "3":
                        UpdatePromotion();
                        break;
                    case "4":
                        DeletePromotion();
                        break;
                    case "5":
                        ViewUsageSummary();
                        break;
                    case "0":
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        static string GetPromoCode()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter the Promo code (0 to cancel): ");
            Console.ResetColor();
            return Console.ReadLine();
        }

        static void AddPromotion()
        {

        }

        static void ViewAllPromotions()
        {

        }

        static void UpdatePromotion()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║             UPDATE PROMO CODE            ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            while (true)
            {
                string promoCode = GetPromoCode();
                if (promoCode == "0") return;

                if (!DataStore.Promotions.ContainsKey(promoCode))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Promo code not found. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Discount Percent");
                Console.WriteLine("  [2] Applicable Class");
                Console.WriteLine("  [3] Expiry Date");
                Console.WriteLine("  [4] Activate/Deactivate");
                Console.WriteLine("  [0] Back");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        UpdateDiscountPercentage(DataStore.Promotions[promoCode]);
                        break;
                    case "2":
                        UpdateApplicableClass(DataStore.Promotions[promoCode]);
                        break;
                    case "3":
                        UpdateExpiryDate(DataStore.Promotions[promoCode]);
                        break;
                    case "4":
                        //UpdateActivateDeactivate(ataStore.Promotions[promoCode]);
                        break;
                    case "0":
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void UpdateDiscountPercentage(Promotion promotion)
        {
            Console.WriteLine($"\n  Current Discount: {promotion.DiscountPercentage:0.##}%");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  New Discount Percentage (1-100): ");
            Console.ResetColor();

            if (!decimal.TryParse(Console.ReadLine(), out decimal newDiscount) ||
                newDiscount < 1 || newDiscount > 100)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid percentage. Must be between 1 and 100. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            promotion.DiscountPercentage = newDiscount;
            DataStore.Promotions[promotion.PromoCode] = promotion;
            CsvHelper.SavePromotions();

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                "UPDATE", "Promotion",
                $"Promo '{promotion.PromoCode}' DiscountPercentage updated to {newDiscount:0.##}%.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Discount updated successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void UpdateApplicableClass(Promotion promotion)
        {
            Console.WriteLine($"\n  Current Applicable Class: {promotion.ApplicableClass}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [1] Economy");
            Console.WriteLine("  [2] Business");
            Console.WriteLine("  [3] Both");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Choice: ");
            Console.ResetColor();

            PromotionApplicableClass newClass;

            switch (Console.ReadLine()?.Trim())
            {
                case "1": newClass = PromotionApplicableClass.Economy; break;
                case "2": newClass = PromotionApplicableClass.Business; break;
                case "3": newClass = PromotionApplicableClass.Both; break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid option. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
            }

            promotion.ApplicableClass = newClass;
            DataStore.Promotions[promotion.PromoCode] = promotion;
            CsvHelper.SavePromotions();

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                "UPDATE", "Promotion",
                $"Promo '{promotion.PromoCode}' ApplicableClass updated to {newClass}.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Applicable class updated successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void UpdateExpiryDate(Promotion promotion)
        {
            Console.WriteLine($"\n  Current Expiry Date: {promotion.EndDate}");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  New Expiry Date (yyyy-MM-dd): ");
            Console.ResetColor();

            // Future date validation. Expiry date should not be in the past.
            if (!DateTime.TryParseExact(Console.ReadLine()?.Trim(), "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime expiryDate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Invalid Expiry date. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            promotion.EndDate = expiryDate;
            DataStore.Promotions[promotion.PromoCode] = promotion;
            CsvHelper.SavePromotions();

            CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                "UPDATE", "Promotion",
                $"Promo '{promotion.PromoCode}' End Date updated to {expiryDate:yyyy-MM-dd}.");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Expiry Date updated successfully. Press Enter.");
            Console.ResetColor();
            Console.ReadLine();
        }

        static void DeletePromotion()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║             DELETE PROMO CODE            ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            while (true)
            {
                string promoCode = GetPromoCode();
                if (promoCode == "0") return;

                if (!DataStore.Promotions.ContainsKey(promoCode))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Promo code not found. Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                bool inUse = DataStore.Tickets.Values.Any(t => t.PromoCode == promoCode);

                if (inUse)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Cannot delete — promo code is referenced by existing tickets. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    return;
                }

                DataStore.Promotions.Remove(promoCode);
                CsvHelper.SavePromotions();

                CsvHelper.WriteSystemLog(Session.CurrentUserID, Session.CurrentUserRole,
                    "DELETE", "Promotion",
                    $"Promo code '{promoCode}' deleted.");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Promo code deleted successfully. Press Enter.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }
        }

        static void ViewUsageSummary()
        {

        }
    }

    static class AdminPortal
    {
        public static void CallTicketPriceCalculator()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║         Ticket Price Calculator          ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [0] Back");
                Console.ResetColor();

                // Origin Airport
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Enter origin airport code: ");
                Console.ResetColor();
                string originCode = Console.ReadLine().ToUpper();
                if (originCode == "0") return;

                if (!DataStore.Airports.ContainsKey(originCode))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid origin airport code. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Destination Airport
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Enter destination airport code: ");
                Console.ResetColor();
                string destCode = Console.ReadLine().ToUpper();
                if (destCode == "0") return;

                if (!DataStore.Airports.ContainsKey(destCode))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid destination airport code. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                if (originCode == destCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Origin and destination cannot be the same. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Travel Date
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Travel date (yyyy-MM-dd): ");
                Console.ResetColor();
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime travelDate))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Invalid date format. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Search flight
                Flight? foundFlight = DataStore.Flights.Values
                    .Cast<Flight?>()
                    .FirstOrDefault(f => f.Value.OriginAirportCode == originCode &&
                                         f.Value.DestinationAirportCode == destCode &&
                                         f.Value.ScheduledDeparture.Date == travelDate.Date);

                if (foundFlight == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  No flights found. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Seat Class
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Seat class [1] Economy  [2] Business: ");
                Console.ResetColor();
                TicketSeatClass seatClass;
                switch (Console.ReadLine())
                {
                    case "1": seatClass = TicketSeatClass.Business; break;
                    default: seatClass = TicketSeatClass.Economy; break;
                }

                // Check seat clss availability
                if (seatClass == TicketSeatClass.Economy && foundFlight.Value.AvailableEconomySeats <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  No economy seats available. Do you want to upgrade to business class [y/n]: ");
                    Console.ResetColor();
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        seatClass = TicketSeatClass.Business;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (seatClass == TicketSeatClass.Business && foundFlight.Value.AvailableBusinessSeats <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  No business seats available. Press Enter.");
                    Console.ResetColor();
                    Console.ReadLine();
                    continue;
                }

                // Passenger ID for loyalty tier
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Enter passenger ID (optional — press Enter to skip): ");
                Console.ResetColor();
                string passengerID = Console.ReadLine();
                Passenger passenger = new Passenger();
                passenger.TierStatus = LoyaltyTier.Bronze; // default if not found

                if (!string.IsNullOrEmpty(passengerID) && DataStore.Passengers.ContainsKey(passengerID))
                {
                    passenger = DataStore.Passengers[passengerID];
                }
                else if (!string.IsNullOrEmpty(passengerID))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Passenger not found. Using Bronze tier by default.");
                    Console.ResetColor();
                }

                // Promo Code
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  Promo code (optional — press Enter to skip): ");
                Console.ResetColor();
                string promoCode = Console.ReadLine();

                // Call pricing engine
                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
                Airport origin = DataStore.Airports[originCode];
                Airport destination = DataStore.Airports[destCode];

                TicketService.TicketPriceCalculator(foundFlight.Value, origin, destination, seatClass, travelDate, passenger, promoCode);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Calculate Again");
                Console.WriteLine("  [0] Back");
                Console.ResetColor();

                Console.Write("\n  Select an option: ");
                if (Console.ReadLine() != "1") return;
            }
        }

        public static void ShowDashboard(Admin admin)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        ADMIN DASHBOARD                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  Logged in as: {admin.FullName} | {DateTime.Now:yyyy-MM-dd HH:mm}");
            Console.ResetColor();

            // ── 1. Flights Today by Status ──────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Flights Today ──────────────────────────────────────");
            Console.ResetColor();

            List<Flight> todayFlights = DataStore.Flights.Values
                .Where(f => f.ScheduledDeparture.Date == DateTime.Today)
                .ToList();

            if (todayFlights.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("  No flights scheduled today.");
                Console.ResetColor();
            }
            else
            {
                var byStatus = todayFlights
                    .GroupBy(f => f.Status)     // group flights that share the same status together
                    .Select(g => new { Status = g.Key, Count = g.Count() });        // for each group, create an anonymous object

                foreach (var s in byStatus)     // g.Key is the status that all flights in this group share
                    Console.WriteLine($"  {s.Status,-15} {s.Count} flight(s)");     // how many flights are in this group
            }

            // ── 2. Total Passengers ─────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Passengers ─────────────────────────────────────────");
            Console.ResetColor();
            Console.WriteLine($"  Total Registered    {DataStore.Passengers.Count}");

            // ── 3. Tickets Sold ─────────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Tickets Sold ───────────────────────────────────────");
            Console.ResetColor();

            int ticketsToday = DataStore.Tickets.Values
                .Count(t => t.BookingDate.Date == DateTime.Today);

            int ticketsAllTime = DataStore.Tickets.Count;

            Console.WriteLine($"  {"Today",-25} {ticketsToday}");
            Console.WriteLine($"  {"All-Time",-25} {ticketsAllTime}");

            // ── 4. Revenue ──────────────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Revenue ────────────────────────────────────────────");
            Console.ResetColor();

            decimal revenueToday = DataStore.Tickets.Values
                .Where(t => t.BookingDate.Date == DateTime.Today &&
                            (t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Boarded))
                .Sum(t => t.FinalPrice);

            decimal revenueAllTime = DataStore.Tickets.Values
                .Where(t => t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Boarded)
                .Sum(t => t.FinalPrice);

            Console.WriteLine($"  {"Today",-25} {revenueToday:C}");
            Console.WriteLine($"  {"All-Time",-25} {revenueAllTime:C}");

            // ── 5. Seat Utilization ─────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Seat Utilization (Scheduled Flights) ───────────────");
            Console.ResetColor();

            List<Flight> scheduledFlights = DataStore.Flights.Values
                .Where(f => f.Status == FlightStatus.Scheduled)
                .ToList();

            if (scheduledFlights.Count == 0)
            {
                Console.WriteLine("  No scheduled flights.");
            }
            else
            {
                int totalSeats = scheduledFlights.Sum(f => f.AvailableBusinessSeats + f.AvailableEconomySeats);
                int soldSeats = scheduledFlights.Sum(f =>
                {
                    Aircraft a = DataStore.Aircrafts[f.AircraftRegNumber];
                    return (a.BusinessSeats - f.AvailableBusinessSeats) +
                           (a.EconomySeats - f.AvailableEconomySeats);
                });

                int originalTotal = scheduledFlights.Sum(f =>
                {
                    Aircraft a = DataStore.Aircrafts[f.AircraftRegNumber];
                    return a.BusinessSeats + a.EconomySeats;
                });

                double utilization = originalTotal == 0 ? 0 : (double)soldSeats / originalTotal * 100;
                Console.WriteLine($"  Seats Sold / Total  {soldSeats} / {originalTotal} ({utilization:F1}%)");
            }

            // ── 6. Top 3 Popular Routes ─────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Top 3 Popular Routes ───────────────────────────────");
            Console.ResetColor();

            var topRoutes = DataStore.Tickets.Values
                .Where(t => t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Boarded)
                .Where(t => DataStore.Flights.ContainsKey(t.FlightNumber))      // safety — skip if flight was deleted
                .Select(t => new
                {
                    Ticket = t,
                    Route = $"{DataStore.Flights[t.FlightNumber].OriginAirportCode} => {DataStore.Flights[t.FlightNumber].DestinationAirportCode}"
                })
                .GroupBy(x => x.Route)
                .OrderByDescending(g => g.Count())
                .Take(3);

            int rank = 1;
            foreach (var route in topRoutes)
                Console.WriteLine($"  {rank++}. {route.Key,-20} {route.Count()} ticket(s)");

            // ── 7. Top 3 Highest Revenue Flights ───────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Top 3 Highest Revenue Flights ──────────────────────");
            Console.ResetColor();

            var topRevenue = DataStore.Tickets.Values
                .Where(t => t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Boarded)
                .GroupBy(t => t.FlightNumber)
                .Select(g => new { FlightNumber = g.Key, Revenue = g.Sum(t => t.FinalPrice) })
                .OrderByDescending(g => g.Revenue)
                .Take(3);

            rank = 1;
            foreach (var item in topRevenue)
                Console.WriteLine($"  {rank++}. {item.FlightNumber,-12} {item.Revenue:C}");

            // ── 8. Crew Assigned to Today's Flights ────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Crew on Today's Flights ────────────────────────────");
            Console.ResetColor();

            List<string> todayFlightNumbers = todayFlights
                .Select(f => f.FlightNumber)
                .ToList();

            int crewToday = DataStore.FlightCrew
                .Where(fc => todayFlightNumbers.Contains(fc.FlightNumber))
                .Select(fc => fc.EmployeeID)
                .Distinct()
                .Count();

            Console.WriteLine($"  Crew Members Assigned   {crewToday}");

            // ── 9. Delayed Flights ──────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Delayed Flights ────────────────────────────────────");
            Console.ResetColor();

            List<Flight> delayedFlights = DataStore.Flights.Values
                .Where(f => f.Status == FlightStatus.Delayed)
                .ToList();

            if (delayedFlights.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("  No delayed flights.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {"Flight",-10} {"Route",-15} {"Scheduled",-20} {"Delayed By"}");
                Console.WriteLine(new string('-', 65));
                foreach (Flight f in delayedFlights)
                {
                    double hoursDelayed = (DateTime.Now - f.ScheduledDeparture).TotalHours;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  {f.FlightNumber,-10} {f.OriginAirportCode}=>{f.DestinationAirportCode,-10} {f.ScheduledDeparture:yyyy-MM-dd HH:mm,-20} {hoursDelayed:F1}h");
                    Console.ResetColor();
                }
            }

            // ── 10. Lost Baggage ────────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ── Lost Baggage ───────────────────────────────────────");
            Console.ResetColor();

            List<Baggage> lostBaggage = DataStore.Baggages
                .Where(b => b.Status == BaggageStatus.Lost)
                .ToList();

            if (lostBaggage.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("  No lost baggage.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  {"Baggage ID",-12} {"Ticket ID",-12} {"Type",-12} {"Weight"}");
                Console.WriteLine(new string('-', 50));
                foreach (Baggage b in lostBaggage)
                    Console.WriteLine($"  {b.BaggageID,-12} {b.TicketID,-12} {b.Type,-12} {b.WeightKg}kg");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Press Enter to continue.");
            Console.ResetColor();
            Console.ReadLine();
        }

        public static void Show(Admin admin)
        {
            while (true)
            {
                Console.Clear();

                ShowDashboard(admin); // show dashboard first on login

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Flight Management");
                Console.WriteLine("  [2] Ticket Price Calculator");
                Console.WriteLine("  [3] Passenger Management");
                Console.WriteLine("  [4] Crew Management");
                Console.WriteLine("  [5] Promotions Management");
                Console.WriteLine("  [6] Baggage Oversight");
                Console.WriteLine("  [7] System Logs");
                Console.WriteLine("  [0] Logout");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        FlightService.Show();
                        break;
                    case "2":
                        CallTicketPriceCalculator();
                        break;
                    case "3":
                        PassengerService.Show();
                        break;
                    case "4":
                        //FlightService.Show();
                        break;
                    case "5":
                        PromotionService.Show();
                        break;
                    case "6":
                        BaggageService.Show();
                        break;
                    case "7":
                        SystemLogService.Show();
                        break;
                    case "0":
                        AuthService.Logout(admin.AdminID, admin.FullName, "Admin");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }
    }

    static class PassengerPortal
    {
        public static void Show(Passenger passenger)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║            Passenger Portal              ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n  Welcome, {passenger.FullName}");
                Console.WriteLine($"  Tier: {passenger.TierStatus} | Points: {passenger.LoyaltyPoints}");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Browse & Search Flights");
                Console.WriteLine("  [2] Manage My Tickets");
                Console.WriteLine("  [3] My Profile");
                Console.WriteLine("  [0] Logout");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        FlightService.Search(passenger);
                        break;
                    case "2":
                        TicketService.ManageMyTickets(passenger);
                        break;
                    case "3":
                        ShowProfile(passenger);
                        break;
                    case "0":
                        AuthService.Logout(passenger.PassengerID, passenger.FullName, "Passenger");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }

        public static void ChangePassword(Passenger passenger)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter current password: ");
            string currPassword = Console.ReadLine();
            if (currPassword == passenger.Password)
            {
                Console.Write("\n  Enter new password: ");
                string newPassword = Console.ReadLine();
                if (AuthService.IsValidPassword(newPassword))
                {
                    Console.Write("\n  Re-enter new password: ");
                    string newPassword2 = Console.ReadLine();
                    if (newPassword == newPassword2)
                    {
                        passenger.Password = newPassword;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n  Password Changed successfully! Press Enter.");
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  New password mismatch. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n  Password must be with has digit, uppercase, special char, and length 8+ . Press Enter to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  New password does not match current one. Press Enter to try again.");
                Console.ResetColor();
                Console.ReadLine();
            }
            Console.ResetColor();
        }

        public static void ShowProfile(Passenger passenger)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine($"║        {passenger.FullName}'s Profile         ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            // Get passenger's booking history
            Console.WriteLine("\n============= Booking History ==============");
            Console.WriteLine($"\n{"Ticket ID",-10} {"Flight",-8} {"From",-6} {"To",-6} {"Departure",-18} {"Class",-10} {"Status"}");
            Console.WriteLine(new string('-', 75));
            List<Ticket> bookingHistory = DataStore.Tickets.Values
                .Where(t => t.PassengerID == passenger.PassengerID)
                .ToList();
            foreach (Ticket t in bookingHistory)
            {
                Flight f = DataStore.Flights[t.FlightNumber];
                Console.WriteLine($"{t.TicketID,-10} {f.FlightNumber,-8} {f.OriginAirportCode,-6} {f.DestinationAirportCode,-6} {f.ScheduledDeparture.ToString("yyyy-MM-dd HH:mm"),-18} {t.SeatClass,-10} {t.Status}");
            }

            // Get passenger's loyalty points history
            Console.WriteLine("\n\n========== Loyalty Points History ==========");
            Console.WriteLine($"\n{"Ticket ID",-12} {"Points Changed",-23} {"Reason",-22} {"Transaction Date",-20}");
            Console.WriteLine(new string('-', 80));
            foreach (LoyaltyLog l in DataStore.LoyaltyLogs)
            {
                if (l.PassengerID == passenger.PassengerID)
                {
                    Console.WriteLine($"\n{l.TicketID,-17} {l.PointsChanged,-13} {l.Reason,-23} {l.TransactionDate,-20}");
                }
            }

            // Get passenger's loyalty tier
            Console.WriteLine($"\n\nLoyalty Tier: {passenger.TierStatus}");
            string nextTier = "";
            int remainPoints = 0;
            switch (passenger.TierStatus)
            {
                case LoyaltyTier.Bronze:
                    nextTier = "Silver";
                    remainPoints = Constants.SilverThreshold - passenger.LoyaltyPoints;
                    break;
                case LoyaltyTier.Silver:
                    nextTier = "Gold";
                    remainPoints = Constants.GoldThreshold - passenger.LoyaltyPoints;
                    break;
                case LoyaltyTier.Gold:
                    nextTier = "Platinum";
                    remainPoints = Constants.PlatinumThreshold - passenger.LoyaltyPoints;
                    break;
                case LoyaltyTier.Platinum:
                    nextTier = "Already in the highest tier.";
                    break;
                default:
                    break;
            }
            Console.WriteLine($"Next Tier: {nextTier}");
            Console.WriteLine($"Reamining Points: {remainPoints}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  [1] Change Password");
            Console.WriteLine("  [Enter] Back to Passenger Portal");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Select an option: ");
            Console.ResetColor();

            switch (Console.ReadLine())
            {
                case "1":
                    ChangePassword(passenger);
                    break;
                default:
                    break;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Load all data from CSV files into memory
            CsvHelper.LoadAirports();
            CsvHelper.LoadAirlines();
            CsvHelper.LoadAircrafts();
            CsvHelper.LoadFlights();
            CsvHelper.LoadPassengers();
            CsvHelper.LoadCrewMembers();
            CsvHelper.LoadFlightCrew();
            CsvHelper.LoadTickets();
            CsvHelper.LoadBaggages();
            CsvHelper.LoadPromotions();
            CsvHelper.LoadAdmins();
            CsvHelper.LoadLoyaltyLogs();
            CsvHelper.LoadSystemLogs();
            CsvHelper.LoadErrorLogs();

            // Main menu
            bool flag = false;
            while (!flag)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║       AIRLINE MANAGEMENT SYSTEM          ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  [1] Login");
                Console.WriteLine("  [2] Register");
                Console.WriteLine("  [0] Exit");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("\n  Select an option: ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AuthService.Login();
                        break;
                    case "2":
                        AuthService.Register();
                        break;
                    case "0":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n  Goodbye!");
                        Console.ResetColor();
                        flag = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n  Invalid option. Press Enter to try again.");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}
