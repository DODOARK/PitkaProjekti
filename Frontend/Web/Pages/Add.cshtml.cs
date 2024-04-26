using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pitka_Projekti_Front.Models;
using System.Text.Json;
using System.Text;
using UserData;


namespace Pitka_Projekti_Front.Pages
{
    public class AddModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AddModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public TaskModel? TaskModels { get; set; }

        public async Task<IActionResult> OnPost()
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(TaskModels),
                Encoding.UTF8,
                "application/json");

            string jsonContentString = await jsonContent.ReadAsStringAsync();
            Console.WriteLine("JSON Content: " + jsonContentString);

            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.PostAsync($"API/V1/{UserId.user_id}/Tasks", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "New task was added.";
                return RedirectToPage("Index");
            }
            else
            {
                TempData["failure"] = "Failed to add task";
                return RedirectToPage("Index");
            }
        }
    }
}

