namespace AirlineManagementSystem
{
    // Define all enums (based on attributes with multible fixed values)
    enum AircraftStatus {Active, UnderMaintenance, Retired}
    enum FlightStatus {Scheduled, Boarding, Departed, Arrived, Delayed, Cancelled}
    enum LoyaltyTier {Bronze, Silver, Gold, Platinum}
    enum CrewMemberRole {Pilot, CoPilot, CabinCrew, GroundStaff}
    enum TicketSeatClass {Business, Economy}
    enum TicketStatus {Confirmed, Cancelled, CheckedIn, Boarded}
    enum BaggageType {Cabin, Hold, Oversized}
    enum BaggageStatus {CheckedIn, Loaded, Lost, Delivered}
    enum PromotionApplicableClass {Economy, Business, Both}

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
        public const string AirportsFile = "Data\\airports.csv";
        public const string AirlinesFile = "Data\\airlines.csv";
        public const string AircraftsFile = "Data\\aircrafts.csv";
        public const string FlightsFile = "Data\\flights.csv";
        public const string PassengersFile = "Data\\passengers.csv";
        public const string CrewMembersFile = "Data\\crew_members.csv";
        public const string FlightCrewFile = "Data\\flight_crew.csv";
        public const string TicketsFile = "Data\\tickets.csv";
        public const string BaggagesFile = "Data\\baggages.csv";
        public const string PromotionsFile = "Data\\promotions.csv";
        public const string AdminsFile = "Data\\admins.csv";
        public const string LoyaltyLogFile = "Data\\loyalty_log.csv";
        public const string SystemLogFile = "Data\\system_log.csv";
        public const string ErrorLogFile = "Data\\error_log.csv";
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

    // Read CSVs
    static class CsvHelper
    {
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
                    newCrewMember.IsAvailable = bool.Parse(record[7]);

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

        //Reads baggage.csv
        public static void LoadBaggage()
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
    }

    internal class Program
    {      
        static void Main(string[] args)
        {
            
        }
    }
}
