# Airline Management System — File Mapping Document

> All data is stored in CSV files. Each entity maps to one file. Junction entities get their own file.
> FK = Foreign Key | PK = Primary Key | NN = Not Null | UQ = Unique

\---

## 1\. airports.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|IATACode|string|PK, UQ, NN|3 uppercase letters (e.g. DXB)|
|FullName|string|NN|Full airport name|
|City|string|NN||
|Country|string|NN||
|TimeZoneOffset|float|NN|e.g. +4.0, -5.0|

\---

## 2\. airlines.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|ICAOCode|string|PK, UQ, NN|4 uppercase letters (e.g. OMAE)|
|Name|string|NN, UQ||
|RegistrationCountry|string|NN||
|ContactInfo|string|NN|Email or phone|

\---

## 3\. aircraft.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|RegistrationNumber|string|PK, UQ, NN|e.g. A6-ENA|
|AirlineICAO|string|FK → airlines.ICAOCode, NN|Owning airline|
|Model|string|NN|e.g. Boeing 737|
|Manufacturer|string|NN||
|TotalSeats|int|NN||
|BusinessSeats|int|NN||
|EconomySeats|int|NN||
|ManufacturingYear|int|NN||
|Status|string|NN|Active / UnderMaintenance / Retired|

\---

## 4\. flights.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|FlightNumber|string|PK, UQ, NN|e.g. WY101|
|OriginAirportCode|string|FK → airports.IATACode, NN|Departure airport|
|DestinationAirportCode|string|FK → airports.IATACode, NN|Arrival airport|
|AirlineICAO|string|FK → airlines.ICAOCode, NN||
|AircraftRegNumber|string|FK → aircraft.RegistrationNumber, NN||
|ScheduledDeparture|datetime|NN|ISO 8601 format|
|ScheduledArrival|datetime|NN|ISO 8601 format|
|ActualDeparture|datetime|nullable|Set after departure|
|ActualArrival|datetime|nullable|Set after arrival|
|Status|string|NN|Scheduled / Boarding / Departed / Arrived / Delayed / Cancelled|
|AvailableBusinessSeats|int|NN|Decremented on booking|
|AvailableEconomySeats|int|NN|Decremented on booking|
|BasePrice|decimal|NN|Economy base price|

\---

## 5\. passengers.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|PassengerID|string|PK, UQ, NN|Auto-generated (e.g. P00001)|
|FullName|string|NN||
|DateOfBirth|date|NN|For age calculation|
|Nationality|string|NN||
|PassportNumber|string|UQ, NN|No duplicates allowed|
|Email|string|UQ, NN|Used for login|
|Phone|string|NN||
|RegistrationDate|datetime|NN|Auto-set on register|
|LoyaltyPoints|int|NN|Default 0|
|TierStatus|string|NN|Bronze / Silver / Gold / Platinum|

\---

## 6\. crew\_members.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|EmployeeID|string|PK, UQ, NN|e.g. CR00001|
|FullName|string|NN||
|Role|string|NN|Pilot / CoPilot / CabinCrew / GroundStaff|
|Nationality|string|NN||
|LicenseNumber|string|nullable|Required for Pilot / CoPilot|
|AirlineICAO|string|FK → airlines.ICAOCode, NN|Affiliated airline|
|YearsOfExperience|int|NN||
|AvailabilityStatus|string|NN|Available / Unavailable|

\---

## 7\. flight\_crew.csv

> Junction entity resolving M:M between Flights and Crew Members

|Column|Type|Constraints|Notes|
|-|-|-|-|
|FlightNumber|string|FK → flights.FlightNumber, NN|Composite PK|
|EmployeeID|string|FK → crew\_members.EmployeeID, NN|Composite PK|
|AssignedDate|datetime|NN|When assignment was made|

**Composite PK:** (FlightNumber + EmployeeID)

\---

## 8\. tickets.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|TicketID|string|PK, UQ, NN|Auto-generated (e.g. TK00001)|
|PassengerID|string|FK → passengers.PassengerID, NN||
|FlightNumber|string|FK → flights.FlightNumber, NN||
|SeatClass|string|NN|Business / Economy|
|SeatNumber|string|NN|Auto-assigned (e.g. 12A)|
|BookingDate|datetime|NN|Auto-set on booking|
|Status|string|NN|Confirmed / Cancelled / CheckedIn / Boarded|
|FinalPrice|decimal|NN|After all discounts and taxes|
|LoyaltyPointsEarned|int|NN|Awarded on confirmation|
|PromoCode|string|FK → promotions.PromoCode, nullable|Optional|

\---

## 9\. baggage.csv

> Weak entity — depends on Tickets

|Column|Type|Constraints|Notes|
|-|-|-|-|
|BaggageID|string|PK, UQ, NN|Auto-generated (e.g. BG00001)|
|TicketID|string|FK → tickets.TicketID, NN|Owner ticket|
|WeightKg|decimal|NN|Must not exceed type limit|
|BaggageType|string|NN|Cabin / Hold / Oversized|
|Status|string|NN|CheckedIn / Loaded / Lost / Delivered|

\---

## 10\. promotions.csv

|Column|Type|Constraints|Notes|
|-|-|-|-|
|PromoCode|string|PK, UQ, NN|e.g. SUMMER25|
|DiscountPercentage|decimal|NN|e.g. 15.0|
|StartDate|date|NN||
|EndDate|date|NN||
|MaxUses|int|NN||
|CurrentUseCount|int|NN|Default 0|
|ApplicableClass|string|NN|Economy / Business / Both|
|IsActive|bool|NN|Set false when MaxUses reached|

\---

## Relationship Summary

|Relationship|Cardinality|Participation|Junction File|
|-|-|-|-|
|Airport → Flight (origin)|1:M|Total on Flight side|—|
|Airport → Flight (destination)|1:M|Total on Flight side|—|
|Airline → Aircraft|1:M|Partial on Aircraft side|—|
|Airline → Flight|1:M|Partial on Flight side|—|
|Aircraft → Flight|1:M|Partial on Flight side|—|
|Flight ↔ Crew Member|M:M|Partial both sides|flight\_crew.csv|
|Passenger → Ticket|1:M|Partial on Ticket side|—|
|Flight → Ticket|1:M|Total on Ticket side|—|
|Ticket → Baggage|1:M|Partial on Baggage side|—|
|Promotion → Ticket|1:M|Partial both sides|—|



