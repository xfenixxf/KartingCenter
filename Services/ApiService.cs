using KartingCenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace KartingCenter.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ApiService(string baseUrl = "http://localhost:55493/")
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<LoginResponseDTO> LoginAsync(string email, string password)
        {
            try
            {
                var loginRequest = new { Email = email, Password = password };
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/auth/login", loginRequest);
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<LoginResponseDTO>(responseBody);
                }
                else
                {
                    return new LoginResponseDTO
                    {
                        Success = false,
                        Message = $"Ошибка авторизации: {response.StatusCode}\n{responseBody}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }
       

        public async Task<bool> CreateLocationIfNotExists(Location location, Dictionary<string, int> map)
        {
            var existing = await GetLocationsAsync();
            var found = existing.FirstOrDefault(l => l.Name == location.Name);
            if (found != null)
            {
                map[location.Name] = found.Id;
                return true;
            }
            var created = await CreateLocationAsync(location);
            if (created != null)
            {
                map[location.Name] = created.Id;
                return true;
            }
            return false;
        }

        public async Task<bool> CreateTeamIfNotExists(Team team, Dictionary<string, int> map)
        {
            var existing = await GetTeamsAsync();
            var found = existing.FirstOrDefault(t => t.Name == team.Name);
            if (found != null)
            {
                map[team.Name] = found.Id;
                return true;
            }
            var created = await CreateTeamAsync(team);
            if (created != null)
            {
                map[team.Name] = created.Id;
                return true;
            }
            return false;
        }

        public async Task<bool> CreateClientIfNotExists(Client client, Dictionary<string, int> map)
        {
            var existing = await GetClientsAsync();
            var found = existing.FirstOrDefault(c => c.FullName == client.FullName && c.Phone == client.Phone);
            if (found != null)
            {
                map[client.FullName] = found.Id;
                return true;
            }
            var created = await CreateClientAsync(client);
            if (created != null)
            {
                map[client.FullName] = created.Id;
                return true;
            }
            return false;
        }

        public async Task<bool> CreateKartIfNotExists(Kart kart, Dictionary<string, int> map)
        {
            var existing = await GetKartsAsync();
            var found = existing.FirstOrDefault(k => k.SerialNumber == kart.SerialNumber);
            if (found != null)
            {
                map[kart.SerialNumber] = found.Id;
                return true;
            }
            var created = await CreateKartAsync(kart);
            if (created != null)
            {
                map[kart.SerialNumber] = created.Id;
                return true;
            }
            return false;
        }

        public async Task<bool> CreateRideIfNotExists(Ride ride)
        {
            var existing = await GetRidesAsync();
            var found = existing.FirstOrDefault(r => r.RideDate == ride.RideDate &&
                                                     r.StartTime == ride.StartTime &&
                                                     r.LocationId == ride.LocationId);
            if (found != null)
            {
                return true;  
            }
            var created = await CreateRideAsync(ride);
            return created != null;
        }
        public async Task<bool> CreateCartTypeIfNotExists(CartType cartType, Dictionary<string, int> map)
        {
            var existing = await GetCartTypesAsync();
            var found = existing.FirstOrDefault(c => c.Name == cartType.Name);
            if (found != null)
            {
                map[cartType.Name] = found.Id;
                return true;
            }

            var created = await CreateCartTypeAsync(cartType);
            if (created != null)
            {
                map[cartType.Name] = created.Id;
                return true;
            }
            return false;
        }
        public async Task<RegisterResult> RegisterAsync(string email, string password)
        {
            try
            {
                var request = new { Email = email, Password = password };
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/auth/register", request);
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<RegisterResult>(responseBody);
                }
                else
                {
                    return new RegisterResult { Success = false, Message = $"Ошибка регистрации: {response.StatusCode}" };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Message = $"Ошибка: {ex.Message}" };
            }
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/clients");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Client>>(json);
            }
            return new List<Client>();
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/clients", client);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Client>(json);
            }
            return null;
        }

        public async Task<bool> UpdateClientAsync(int id, Client client)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/clients/{id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Team>> GetTeamsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/teams");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Team>>(json);
            }
            return new List<Team>();
        }

        public async Task<List<Team>> GetActiveTeamsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/teams/active");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Team>>(json);
            }
            return new List<Team>();
        }

        public async Task<Team> CreateTeamAsync(Team team)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/teams", team);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Team>(json);
            }
            return null;
        }

        public async Task<bool> UpdateTeamAsync(int id, Team team)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/teams/{id}", team);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/teams/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CartType>> GetCartTypesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/carttypes");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CartType>>(json);
            }
            return new List<CartType>();
        }

        public async Task<CartType> CreateCartTypeAsync(CartType cartType)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/carttypes", cartType);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CartType>(json);
            }
            return null;
        }

        public async Task<bool> UpdateCartTypeAsync(int id, CartType cartType)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/carttypes/{id}", cartType);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCartTypeAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/carttypes/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Kart>> GetKartsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/karts");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Kart>>(json);
            }
            return new List<Kart>();
        }

        public async Task<List<Kart>> GetActiveKartsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/karts/active");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Kart>>(json);
            }
            return new List<Kart>();
        }

        public async Task<Kart> GetKartByIdAsync(int id)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/karts/{id}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Kart>(json);
            }
            return null;
        }

        public async Task<Kart> CreateKartAsync(Kart kart)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/karts", kart);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Kart>(json);
            }
            return null;
        }

        public async Task<bool> UpdateKartAsync(int id, Kart kart)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/karts/{id}", kart);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteKartAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/karts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/locations");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Location>>(json);
            }
            return new List<Location>();
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/locations", location);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Location>(json);
            }
            return null;
        }

        public async Task<bool> UpdateLocationAsync(int id, Location location)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/locations/{id}", location);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/locations/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Ride>> GetRidesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/rides");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Ride>>(json);
            }
            return new List<Ride>();
        }

        public async Task<Ride> GetRideByIdAsync(int id)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/rides/{id}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Ride>(json);
            }
            return null;
        }

        public async Task<List<Ride>> GetRidesByDateAsync(DateTime date)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            HttpResponseMessage response = await _client.GetAsync($"api/rides/bydate/{dateStr}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Ride>>(json);
            }
            return new List<Ride>();
        }

        public async Task<Ride> CreateRideAsync(Ride ride)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/rides", ride);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Ride>(json);
            }
            return null;
        }

        public async Task<bool> UpdateRideAsync(int id, Ride ride)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/rides/{id}", ride);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRideAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/rides/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Ride>> GetRidesByMonthAsync(int year, int month)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/rides/bymonth/{year}/{month}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Ride>>(json);
            }
            return new List<Ride>();
        }

        public async Task<List<RideParticipant>> GetRideParticipantsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/rideparticipants");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<RideParticipant>>(json);
            }
            return new List<RideParticipant>();
        }

        public async Task<RideParticipant> GetRideParticipantByIdAsync(int id)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/rideparticipants/{id}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RideParticipant>(json);
            }
            return null;
        }

        public async Task<bool> DeleteRideParticipantAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/rideparticipants/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<RideParticipant> CreateRideParticipantAsync(CreateRideParticipant participant)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/rideparticipants", participant);
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<RideParticipant>(responseBody);
                }
                else
                {
                    MessageBox.Show($"Ошибка {response.StatusCode}:\n{responseBody}", "Ошибка API", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<bool> UpdateRideTotalAmountAsync(int rideId)
        {
            try
            {
                var allParticipants = await GetRideParticipantsAsync();
                var rideParticipants = allParticipants.FindAll(p => p.RideId == rideId);
                decimal totalAmount = 0;
                foreach (var participant in rideParticipants)
                {
                    var kart = await GetKartByIdAsync(participant.KartId);
                    if (kart != null) totalAmount += kart.Price;
                }
                var ride = await GetRideByIdAsync(rideId);
                if (ride != null)
                {
                    ride.AmountPaid = totalAmount;
                    return await UpdateRideAsync(ride.Id, ride);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчёта суммы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<List<PartDTO>> GetPartsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/parts");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PartDTO>>(json);
            }
            return new List<PartDTO>();
        }

        public async Task<PartDTO> CreatePartAsync(CreatePartDTO part)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/parts", part);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PartDTO>(json);
            }
            return null;
        }

        public async Task<bool> UpdatePartAsync(int id, CreatePartDTO part)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/parts/{id}", part);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePartAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/parts/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TODTO>> GetTOAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/to");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TODTO>>(json);
            }
            return new List<TODTO>();
        }

        public async Task<TODTO> CreateTOAsync(CreateTODTO dto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/to", dto);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TODTO>(json);
            }
            return null;
        }

        public async Task<bool> UpdateTOAsync(int id, UpdateTODTO dto)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/to/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTOAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/to/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TypeTODTO>> GetTypeTOAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/typeto");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TypeTODTO>>(json);
            }
            return new List<TypeTODTO>();
        }

        public async Task<TypeTODTO> CreateTypeTOAsync(CreateTypeTODTO dto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/typeto", dto);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TypeTODTO>(json);
            }
            return null;
        }

        public async Task<bool> UpdateTypeTOAsync(int id, CreateTypeTODTO dto)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/typeto/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTypeTOAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/typeto/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PersonalDTO>> GetPersonalAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/personal");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PersonalDTO>>(json);
            }
            return new List<PersonalDTO>();
        }

        public async Task<PersonalDTO> CreatePersonalAsync(CreatePersonalDTO dto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/personal", dto);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PersonalDTO>(json);
            }
            return null;
        }

        public async Task<bool> UpdatePersonalAsync(int id, CreatePersonalDTO dto)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/personal/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePersonalAsync(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/personal/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/auth/roles");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Role>>(json);
            }
            return new List<Role>();
        }

        public async Task<DailyStats> GetDailyStatsAsync(DateTime date)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            HttpResponseMessage response = await _client.GetAsync($"api/dashboard/dailystats/{dateStr}");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DailyStats>(json);
            }
            return null;
        }
    }
}