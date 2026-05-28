
using System;

namespace KartingCenter.Models
{
    public class CartType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal PricePerRace { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool IsPermanent { get; set; }
    }

    public class Kart
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public int CartTypeId { get; set; }
        public string CartTypeName { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class Ride
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string RideData { get; set; }
        public DateTime RideDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public decimal AmountPaid { get; set; }
        public int ParticipantsCount { get; set; }
    }

    public class RideParticipant
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int RideId { get; set; }
        public int KartId { get; set; }
        public string KartSerialNumber { get; set; }
        public DateTime ParticipantDate { get; set; }
    }

    public class CreateRideParticipant
    {
        public int ClientId { get; set; }
        public int TeamId { get; set; }
        public int RideId { get; set; }
        public int KartId { get; set; }
    }

    public class DailyStats
    {
        public DateTime Date { get; set; }
        public int TotalRides { get; set; }
        public string StickerColor { get; set; }
        public string StatusDescription { get; set; }
    }
}