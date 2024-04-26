using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserData;

using Pitka_Projekti_Front.Models;

namespace Pitka_Projekti_Front.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public List<TaskModel>? TaskModels { get; set; }

    public async Task OnGet()
    {
        var httpClient = _httpClientFactory.CreateClient("TaskAPI");

        using HttpResponseMessage response = await httpClient.GetAsync($"/API/V1/{UserId.user_id}/Tasks");

        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            if (contentStream != null)
            {
                TaskModels = await JsonSerializer.DeserializeAsync<List<TaskModel>>(contentStream);
            }
        }
    }
}
