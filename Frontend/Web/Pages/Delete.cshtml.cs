using UserData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pitka_Projekti_Front.Models;
using System.Text.Json;

namespace Pitka_Projekti_Front.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public TaskModel? TaskModels { get; set; }


        public async Task OnGet(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.GetAsync($"/API/V1/{UserId.user_id}/Tasks/{id}");

            if (response.IsSuccessStatusCode)
            {
                using var contentStream = await response.Content.ReadAsStreamAsync();
                TaskModels = await JsonSerializer.DeserializeAsync<TaskModel>(contentStream);
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.DeleteAsync($"/API/V1/{UserId.user_id}/Tasks/{TaskModels.id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Task was deleted successfully.";
                return RedirectToPage("Index");
            }
            else
            {
                TempData["failure"] = "Task could not be deleted";
                return RedirectToPage("Index");
            }

        }
    }
}

