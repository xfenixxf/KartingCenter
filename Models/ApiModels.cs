
using System;
using System.Collections.Generic;

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
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } 
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserInfoDTO User { get; set; }
    }

    public class UserInfoDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }

    public class Login
    {
        public int ID_login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Role
    {
        public int ID_Roles { get; set; }
        public string Name_role { get; set; }
    }
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }
    public class TypeTODTO
    {
        public int Id { get; set; }
        public string NameType { get; set; }
        public string Description { get; set; }
        public float? RecommendedInterval { get; set; }
    }

    public class CreateTypeTODTO
    {
        public string NameType { get; set; }
        public string Description { get; set; }
        public float? RecommendedInterval { get; set; }
    }

    public class PartDTO
    {
        public int Id { get; set; }
        public string NameParts { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }

    public class CreatePartDTO
    {
        public string NameParts { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }

    public class TODTO
    {
        public int Id { get; set; }
        public int KartId { get; set; }
        public string KartSerialNumber { get; set; }
        public string KartTypeName { get; set; }
        public int TypeTOId { get; set; }
        public string TypeTOName { get; set; }
        public int MasterId { get; set; }
        public string MasterFullName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public List<UsedPartDTO> UsedParts { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class CreateTODTO
    {
        public int KartId { get; set; }
        public int TypeTOId { get; set; }
        public int MasterId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public List<CreateUsedPartDTO> UsedParts { get; set; }
    }

    public class UpdateTODTO
    {
        public int TypeTOId { get; set; }
        public int MasterId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public List<CreateUsedPartDTO> UsedParts { get; set; }
    }

    public class UsedPartDTO
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public float PartPrice { get; set; }
        public float TotalPrice { get; set; }
    }
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }

    
    public class CreateUsedPartDTO
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
    }

    public class PersonalDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int LoginId { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class CreatePersonalDTO
    {
        public string FullName { get; set; }
        public int LoginId { get; set; }
        public int RoleId { get; set; }
    }
}