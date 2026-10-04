namespace SaddamNews.Models.ViewModels;

public class HomeIndexViewModel
{
    public IEnumerable<News> NewsList { get; set; } = Enumerable.Empty<News>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
