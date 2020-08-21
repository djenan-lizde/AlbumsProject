using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Albums.Configuration;
using Albums.Models;
using Albums.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReflectionIT.Mvc.Paging;

namespace Albums.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpService _httpService;
        private readonly IOptions<AppSettings> _options;

        public UsersController(IHttpService httpService, IOptions<AppSettings> options)
        {
            _httpService = httpService;
            _options = options;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var uri = new Uri(_options.Value.ApiUrl + "/users");

            List<User> users = await _httpService.Get<List<User>>(uri.ToString());

            var model = PagingList.Create(users, 5, page);

            return View(model);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var uriBuilder = new UriBuilder(_options.Value.ApiUrl)
            {
                Path = "users",
                Port = -1,
                Query = $"id={Id}"
            };

            List<User> users = await _httpService.Get<List<User>>(uriBuilder.ToString());

            return View(users.FirstOrDefault());
        }
        public IActionResult Save(User user)
        {
            try
            {
                string filename = "User.txt";
                string fullPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                string json = JsonConvert.SerializeObject(user, Formatting.Indented);
                if (!System.IO.File.Exists(fullPath))
                {
                    System.IO.File.WriteAllText(fullPath, json);
                }
                else
                {
                    System.IO.File.AppendAllText(fullPath, json);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
