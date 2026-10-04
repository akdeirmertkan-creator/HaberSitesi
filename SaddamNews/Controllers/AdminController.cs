using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaddamNews.Data;
using SaddamNews.Models;
using SaddamNews.Models.ViewModels;

namespace SaddamNews.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var newsList = await _context.News
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(newsList);
    }

    public IActionResult Create()
    {
        return View(new NewsCreateEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsCreateEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var news = new News
            {
                Title = model.Title,
                Content = model.Content,
                ImageUrl = model.ImageUrl,
                Author = User.Identity?.Name ?? "Admin",
                CreatedAt = DateTime.UtcNow
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var news = await _context.News.FindAsync(id);
        if (news == null) return NotFound();

        var model = new NewsCreateEditViewModel
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            ImageUrl = news.ImageUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NewsCreateEditViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var existingNews = await _context.News.FindAsync(id);
            if (existingNews == null)
            {
                return NotFound();
            }

            existingNews.Title = model.Title;
            existingNews.Content = model.Content;
            existingNews.ImageUrl = model.ImageUrl;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var news = await _context.News.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (news == null) return NotFound();

        return View(news);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
