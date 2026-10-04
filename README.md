# Orta Doğu Bülteni - Haber Sitesi

Görsel Programlama Dersi için .NET 10 MVC ve Entity Framework Core kullanılarak geliştirilmiş dinamik haber sitesi.

## Özellikler

- **.NET 10 & ASP.NET Core MVC** mimarisi.
- **SQLite & Entity Framework Core** ile veritabanı yönetimi ve başlangıçta otomatik migration (`Database.Migrate`).
- **Özel Tema:** Askeri yeşil / haki renk paleti ve Saddam Hüseyin konsepti.
- **Dinamik Dark / Light Mod:** Kullanıcının sistem/tarayıcı temasını otomatik algılar (`prefers-color-scheme`).
- **AOS (Animate On Scroll):** Yüksek kaliteli kaydırma ve sayfa geçiş animasyonları.
- **Kimlik Doğrulama & Yetkilendirme (Cookie Authentication):**
  - **Admin:** Haber ekleme, düzenleme ve silme yetkisi.
  - **Editör:** Haber ekleme ve düzenleme yetkisi.
  - Sabit zamanlı şifre doğrulaması (`FixedTimeEquals`), Open Redirect ve Brute-force saldırılarına karşı Rate Limiting koruması.
- **Okuyucu Görünümü:** Sayfalanmış (Pagination) haber akışı ve haber detay sayfası (`Devamını Oku`).

## Kurulum ve Çalıştırma

1. Bağımlılıkları yükleyin ve projeyi derleyin:
   ```bash
   dotnet build
   ```

2. `.env.example` dosyasını `.env` olarak kopyalayın ve hesap bilgilerinizi belirleyin:
   ```bash
   cp .env.example .env
   ```

3. Uygulamayı başlatın:
   ```bash
   dotnet run --project SaddamNews
   ```

4. Tarayıcıda `http://localhost:5274` adresine gidin.
