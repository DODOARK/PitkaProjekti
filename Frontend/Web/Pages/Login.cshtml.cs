using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pitka_Projekti_Front.Models;
using System.Text.Json;
using System.Text;
using UserData;

namespace Pitka_Projekti_Front.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UserModel IUser { get; set; }
        public async Task<IActionResult> OnPost()
        {
            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            var userData = new
            {
                name = IUser.name,
                password = IUser.password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(userData),
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync($"/API/V1/Login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Read the response content as integer
                string responseBody = await response.Content.ReadAsStringAsync();

                if (int.TryParse(responseBody, out int userId))
                {
                    UserId.user_id = userId;
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["failure"] = "Failed to login.";
                    return RedirectToPage("Index");
                }
            }
            else
            {
                TempData["failure"] = "Failed to login.";
                return RedirectToPage("Index");
            }
        }
    }
}
