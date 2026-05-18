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
        public const string AircraftFile = "Data\\aircraft.csv";
        public const string FlightsFile = "Data\\flights.csv";
        public const string PassengersFile = "Data\\passengers.csv";
        public const string CrewMembersFile = "Data\\crew_members.csv";
        public const string FlightCrewFile = "Data\\flight_crew.csv";
        public const string TicketsFile = "Data\\tickets.csv";
        public const string BaggageFile = "Data\\baggage.csv";
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
        public static Dictionary<string, Aircraft> Aircraft = new Dictionary<string, Aircraft>();
        public static Dictionary<string, Flight> Flights = new Dictionary<string, Flight>();
        public static Dictionary<string, Passenger> Passengers = new Dictionary<string, Passenger>();
        public static Dictionary<string, CrewMember> CrewMembers = new Dictionary<string, CrewMember>();
        public static Dictionary<string, Ticket> Tickets = new Dictionary<string, Ticket>();
        public static Dictionary<string, Promotion> Promotions = new Dictionary<string, Promotion>();
        public static Dictionary<string, Admin> Admins = new Dictionary<string, Admin>();

        // Lists
        public static List<Baggage> Baggage = new List<Baggage>();
        public static List<LoyaltyLog> LoyaltyLogs = new List<LoyaltyLog>();
        public static List<SystemLog> SystemLogs = new List<SystemLog>();
        public static List<ErrorLog> ErrorLogs = new List<ErrorLog>();
    }

    internal class Program
    {      
        static void Main(string[] args)
        {
            
        }
    }
}
