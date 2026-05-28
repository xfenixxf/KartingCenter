using KartingCenter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                    if (kart != null)
                    {
                        totalAmount += kart.Price;
                    }
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
    }
}