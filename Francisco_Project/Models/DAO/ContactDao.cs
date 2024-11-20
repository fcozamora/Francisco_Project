using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Francisco_Project.Models.DTO;

namespace Francisco_Project.Models.DAO
{
    public class ContactDao
    {
        private readonly string _baseUrl = "https://saacapps.com";

        public async Task<string> AuthenticateAsync(string clientId, string tokenPass)
        {
            using (var client = new HttpClient())
            {
                var apiKeyBase64 = EncodeToBase64(clientId, tokenPass);
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/payout/auth.php");
                request.Headers.Add("Authorization", $"Basic {apiKeyBase64}");

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    string token = await response.Content.ReadAsStringAsync();
                    return token;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return null;
                }
            }
        }

        public async Task<List<ContactDto>> GetAllContactsAsync(string token)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/payout/contact.php");
                request.Headers.Add("Authorization", $"Bearer {token}");

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync();
                    List<ContactDto> contacts = JsonConvert.DeserializeObject<List<ContactDto>>(responseData);

                    return contacts;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return new List<ContactDto>();
                }
            }
        }

        public async Task<ContactDto> GetContactByEmailAsync(string token, string email)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/payout/contact.php?email={email}");
                request.Headers.Add("Authorization", $"Bearer {token}");

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync();
                    ContactDto contact = JsonConvert.DeserializeObject<ContactDto>(responseData);

                    return contact;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return null;
                }
            }
        }

        public async Task<ContactDto> CreateContactAsync(string token, ContactDto newContact)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/payout/contact.php");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Add("Content-Type", "application/json");

                var json = JsonConvert.SerializeObject(newContact);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseData = await response.Content.ReadAsStringAsync();
                    ContactDto createdContact = JsonConvert.DeserializeObject<ContactDto>(responseData);

                    return createdContact;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    return null;
                }
            }
        }

        private string EncodeToBase64(string clientId, string secret)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}");
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
