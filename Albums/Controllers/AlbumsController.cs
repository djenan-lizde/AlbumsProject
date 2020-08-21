using Albums.Configuration;
using Albums.Models;
using Albums.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Albums.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IHttpService _httpService;
        private readonly IOptions<AppSettings> _options;

        public AlbumsController(IHttpService httpService, IOptions<AppSettings> options)
        {
            _httpService = httpService;
            _options = options;
        }

        public async Task<IActionResult> Index(int? Id)
        {
            var uriBuilder = new UriBuilder(_options.Value.ApiUrl)
            {
                Path = "albums",
                Port = -1,
                Query = Id.HasValue ? $"userId={Id}" : string.Empty
            };

            List<Album> albums = await _httpService.Get<List<Album>>(uriBuilder.ToString());

            return View(albums);
        }
        public async Task<IActionResult> Details(int Id)
        {
            var uriBuilder = new UriBuilder(_options.Value.ApiUrl)
            {
                Path = "photos",
                Port = -1,
                Query = $"albumId={Id}"
            };

            List<Photo> photos = await _httpService.Get<List<Photo>>(uriBuilder.ToString());

            return View(photos);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var uriBuilder = new UriBuilder(_options.Value.ApiUrl)
            {
                Path = "albums",
                Port = -1,
                Query = $"id={Id}"
            };

            List<Album> albums = await _httpService.Get<List<Album>>(uriBuilder.ToString());

            return View(albums.FirstOrDefault());
        }
        public IActionResult Save(Album album)
        {
            try
            {
                string filename = "Album.txt";
                string fullPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
                string json = JsonConvert.SerializeObject(album, Formatting.Indented);
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
            return RedirectToAction(nameof(Index), new { Id = album.UserId });
        }
    }
}
