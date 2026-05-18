namespace AirlineManagementSystem
{
    // Define all enums (based on attributes with multible fixed values)
    enum AircraftStatus {Active, UnderMaintenance, Retired}
    enum FlightStatus {Scheduled, Boarding, Departed, Arrived, Delayed, Cancelled}
    enum LoyaltyTier { Bronze, Silver, Gold, Platinum}
    enum CrewMemberRole {Pilot, CoPilot, CabinCrew, GroundStaff}
    enum TicketSeatClass {Business, Economy}
    enum TicketStatus {Confirmed, Cancelled, CheckedIn, Boarded}
    enum BaggageType {Cabin, Hold, Oversized}
    enum BaggageStatus {CheckedIn, Loaded, Lost, Delivered}
    enum PromotionApplicableClass {Economy, Business, Both}

    

    internal class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
