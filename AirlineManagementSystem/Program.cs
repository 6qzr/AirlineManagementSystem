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

        public const int MaxFailedLoginAttempts = 3;
        public const int LockoutMinutes = 15;
        public const int MinPasswordLength = 8;
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

        /* 
        * ============== Save CSVs ==============
        */
        // Save Airport
        public static void SaveAirport()
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

        // Save Aircraft
        public static void SaveAircraft()
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

        // Save Baggage
        public static void SaveBaggage()
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

    static class AuthService
    {
        /* 
         * ============== Login ==============
         */
        public static void Login()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║                   Login                  ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n  Enter your email: ");
            Console.ResetColor();
            string email = Console.ReadLine();

            if (string.IsNullOrEmpty(email))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  Email cannot be empty. Press Enter to try again.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            Admin? foundAdmin = DataStore.Admins.Values
                .FirstOrDefault(a => a.Email == email);

            Passenger? foundPassenger = DataStore.Passengers.Values
                .FirstOrDefault(p => p.Email == email);

            if (!foundAdmin.HasValue && !foundPassenger.HasValue)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  No account found with that email. Press Enter to try again.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            // Lockout check
            if (foundAdmin.HasValue && foundAdmin.Value.LockoutUntil.HasValue && foundAdmin.Value.LockoutUntil.Value > DateTime.Now)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  Account locked. Try again after {foundAdmin.Value.LockoutUntil.Value.ToString("HH:mm:ss")}");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            if (foundPassenger.HasValue && foundPassenger.Value.LockoutUntil.HasValue && foundPassenger.Value.LockoutUntil.Value > DateTime.Now)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  Account locked. Try again after {foundPassenger.Value.LockoutUntil.Value.ToString("HH:mm:ss")}");
                Console.ResetColor();
                Console.ReadLine();
                return;
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
                return;
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
                    // AdminPortal.Show(a); → uncomment when built
                }
                else
                {
                    a.FailedLoginAttempts++;
                    if (a.FailedLoginAttempts >= Constants.MaxFailedLoginAttempts)
                    {
                        a.LockoutUntil = DateTime.Now.AddMinutes(Constants.LockoutMinutes);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n  Too many failed attempts. Account locked for {Constants.LockoutMinutes} minutes.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n  Wrong password. {Constants.MaxFailedLoginAttempts - a.FailedLoginAttempts} attempts remaining.");
                    }
                    Console.ResetColor();
                    DataStore.Admins[a.AdminID] = a;
                    CsvHelper.SaveAdmins();
                    Console.ReadLine();
                    return;
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
                    // PassengerPortal.Show(p); → uncomment when built
                }
                else
                {
                    p.FailedLoginAttempts++;
                    if (p.FailedLoginAttempts >= Constants.MaxFailedLoginAttempts)
                    {
                        p.LockoutUntil = DateTime.Now.AddMinutes(Constants.LockoutMinutes);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n  Too many failed attempts. Account locked for {Constants.LockoutMinutes} minutes.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\n  Wrong password. {Constants.MaxFailedLoginAttempts - p.FailedLoginAttempts} attempts remaining.");
                    }
                    Console.ResetColor();
                    DataStore.Passengers[p.PassengerID] = p;
                    CsvHelper.SavePassengers();
                    Console.ReadLine();
                    return;
                }
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
            CsvHelper.LoadBaggage();
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
                        //Register();
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
