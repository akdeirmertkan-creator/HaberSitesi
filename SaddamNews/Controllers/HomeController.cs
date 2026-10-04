using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaddamNews.Data;
using SaddamNews.Models;
using SaddamNews.Models.ViewModels;

namespace SaddamNews.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private const int PageSize = 6;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        if (page < 1) page = 1;

        var query = _context.News.AsNoTracking().OrderByDescending(n => n.CreatedAt);
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        if (totalPages == 0) totalPages = 1;

        var newsList = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            NewsList = newsList,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var news = await _context.News
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (news == null)
        {
            return NotFound();
        }

        return View(news);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
