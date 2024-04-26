using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pitka_Projekti_Front.Models;
using System.Text.Json;
using System.Text;

namespace Pitka_Projekti_Front.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UserModel IUser { get; set; }

        public async Task<List<string>> GetUsernames()
        {
            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.GetAsync($"/API/V1/Users");

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                if (contentStream != null)
                {
                    List<string> usernames = await JsonSerializer.DeserializeAsync<List<string>>(contentStream);
                    return usernames;
                }
                return null;
            }
            return null;
        }
        public async Task<IActionResult> OnPost()
        {
            List<string> usernames = await GetUsernames();

            if (usernames != null && usernames.Contains(IUser.name))
            {
                TempData["failure"] = "Username is already taken. Please choose a different username.";
                return RedirectToPage("Index");
            }

            if (IUser.password != IUser.confirmPassword)
            {
                TempData["failure"] = "Make sure the passwords match.";
                return RedirectToPage("Index");
            }

            var userData = new
            {
                IUser.name,
                IUser.password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(userData),
                    Encoding.UTF8,
                    "application/json");

            Console.WriteLine(IUser);

            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.PostAsync($"/API/V1/Users", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "New user registered successfully.";
                return RedirectToPage("Index");
            }
            else
            {
                TempData["failure"] = "Failed to register user.";
                return RedirectToPage("Index");
            }
        }
    }
}
