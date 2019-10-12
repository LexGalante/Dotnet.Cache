using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dotnet.Cache.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace Dotnet.Cache.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public HomeController(ILogger<HomeController> logger,
            IMemoryCache cache,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _memoryCache = cache;
            _distributedCache = distributedCache;
        }


        [HttpPost]
        public async Task<IActionResult> Post(PostViewModel viewModel)
        {
            //Salva o valor no memory cache
            _memoryCache.Set(key: viewModel.Key,
                       value: viewModel.Value,
                       options: new MemoryCacheEntryOptions() { }.SetSlidingExpiration(TimeSpan.FromMinutes(5)));
            //Salva valor no cache redis
            await _distributedCache.SetStringAsync(key: viewModel.Key,
                                            value: viewModel.Value,
                                            options: new DistributedCacheEntryOptions() { }.SetSlidingExpiration(TimeSpan.FromMinutes(5)));

            return Redirect($"/{viewModel.Key}");
        }
        
        [HttpGet]
        [Route("/{cacheKey?}")]
        public async Task<IActionResult> Index(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var valueInMemoryCache = string.Empty;                
                _memoryCache.TryGetValue<string>(cacheKey, out valueInMemoryCache);
                ViewBag.MemoryCache = valueInMemoryCache;                
                ViewBag.DistributedCache = await _distributedCache.GetStringAsync(cacheKey);
            }
            
            return View();
        }
    }
}
