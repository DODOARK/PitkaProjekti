using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pitka_Projekti_Front.Models;
using System.Text.Json;
using System.Text;
using UserData;

namespace Pitka_Projekti_Front.Pages
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
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
            if (TaskModels.description == "" || TaskModels.description == null)
            {
                TaskModels.description = "N/A";
            }

            var jsonContent = new StringContent(JsonSerializer.Serialize(TaskModels),
                Encoding.UTF8,
                "application/json");

            var httpClient = _httpClientFactory.CreateClient("TaskAPI");

            using HttpResponseMessage response = await httpClient.PutAsync($"API/V1/{UserId.user_id}/Tasks/{TaskModels.id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Task was edited succesfully";
                return RedirectToPage("Index");
            }
            else
            {
                TempData["failure"] = "There was a problem when editing the task";
                return RedirectToPage("Index");
            }
        }
    }
}


