using System.ComponentModel.DataAnnotations;

namespace SaddamNews.Models.ViewModels;

public class NewsCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık gereklidir.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    [Display(Name = "Haber Başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik gereklidir.")]
    [Display(Name = "İçerik")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Görsel URL")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
    public string? ImageUrl { get; set; }
}
