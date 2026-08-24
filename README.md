<div align="center">
  <h1>🚦 Trafik Cezası Yönetim Modülü (Traffic Fine Manager)</h1>
  <p>
    <strong>ASP.NET Core MVC 9.0 ile geliştirilmiş, rol tabanlı ve çok aşamalı onay mekanizmasına sahip kurumsal trafik cezası takip sistemi.</strong>
  </p>
  <p>
    <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 9.0" />
    <img src="https://img.shields.io/badge/Entity_Framework_Core-Code_First-0078D4?style=for-the-badge&logo=nuget" alt="EF Core" />
    <img src="https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap" alt="Bootstrap 5.3" />
    <img src="https://img.shields.io/badge/Architecture-MVC-FF6F00?style=for-the-badge" alt="MVC" />
  </p>
</div>

---

## 📑 İçindekiler
1. [Proje Hakkında](#-proje-hakkında)
2. [Sistem Gereksinimleri](#-sistem-gereksinimleri)
3. [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma-çok-basit)
4. [Test Kullanıcıları ve Şifreler](#-test-kullanıcıları-ve-şifreler)
5. [Sistemin Detaylı Kullanım Rehberi (Adım Adım)](#-sistemin-detaylı-kullanım-rehberi-adım-adım)
6. [Öne Çıkan Özellikler](#-öne-çıkan-özellikler)
7. [Dinamik Seeding ve Büyük Veri](#-dinamik-seeding-yaşayan-veritabanı-ve-büyük-veri)
8. [Kullanılan Teknolojiler & Mimari](#-kullanılan-teknolojiler--mimari)
9. [Veritabanı Diyagramı](#-veritabanı-diyagramı-kavramsal-mimari)

---

## 📖 Proje Hakkında

Bu proje, bir şirket bünyesindeki araçlara kesilen trafik cezalarının sisteme girilmesi, **Standart Kullanıcı ➔ Yönetici ➔ Finansman** hiyerarşisi üzerinden onay sürecinden geçirilmesi ve tüm işlemlerin tarihçesinin tutulması amacıyla geliştirilmiştir. 

Proje, geniş bir **JSON Data Seeding** altyapısı sayesinde Türkiye'deki tüm il, ilçe, marka ve modelleri dinamik olarak veritabanına aktarır. Hem güvenlik (yetkilendirme) hem de kullanıcı deneyimi (UX) en üst düzeyde tutulmuştur.

**Sistemin İşleyişi (Kullanım Senaryosu):**
1. **Kayıt (Standart Kullanıcı):** Plaka girilerek trafik cezası sisteme işlenir. Girilen plaka sistemde henüz yoksa, kullanıcıdan araç detayları (Marka/Model) istenir ve araç otomatik olarak envantere kaydedilir. Ceza bu aşamada "Yeni" statüsündedir.
2. **Kontrol (Yönetici):** Yönetici, panele düşen yeni cezaları inceler. Sürücüye ait hassas verileri (TC Kimlik No) sistemde sadece yönetici görebilir. Bilgiler doğruysa cezayı onaylayarak "Finans Onayında" statüsüne geçirir veya bir iptal gerekçesi yazarak reddeder.
3. **Tahsilat (Finansman):** Finans departmanı, yöneticiden onay almış cezaların tahsilatını yaparak süreci "Tamamlandı" statüsüyle sonlandırır.
4. **Takip (Audit Log):** Ceza üzerindeki tüm bu statü değişiklikleri; işlemi yapan kişi, saat ve açıklama bilgisiyle birlikte "Tarihçe" alanında şeffaf bir şekilde loglanır.

---

## 💻 Sistem Gereksinimleri
Projeyi çalıştırmak için bilgisayarınızda aşağıdaki yazılımların kurulu olması gerekmektedir:
- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download)**
- **SQL Server** (Visual Studio kullanıyorsanız LocalDB otomatik olarak gelmektedir)
- **Modern bir web tarayıcı** (Edge, Chrome, Safari vb.)

---

## 🚀 Kurulum ve Çalıştırma (Çok Basit!)

Projeyi yerel makinenizde (Localhost) çalıştırmak için karmaşık veritabanı komutlarıyla (Migration vb.) uğraşmanıza gerek yoktur. Projeye **Oto-Kurulum (Auto-Migration)** özelliği eklenmiştir.

### Kurulum Adımları

1. Projeyi klonlayın veya zip dosyasından çıkartın:
   ```bash
   git clone <repo-url>
   ```
2. Proje dizinine gidin:
   ```bash
   cd TrafficFineManager/trafficFineManager
   ```
3. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```

> **Not:** Uygulama ilk kez çalıştırıldığında (Oto-Migration sayesinde) veritabanı SQL Server üzerinde otomatik olarak oluşturulacak, tablolar kurulacak ve tüm il/ilçe/marka/model json verileri parse edilip içine gömülecektir. Bu işlem ilk açılışta 10-15 saniye sürebilir. Sonrasında tarayıcınızdan komut satırında yazan adrese (genellikle `http://localhost:5087`) giderek sistemi kullanabilirsiniz.

### ⚠️ Windows Harici (Mac / Linux) Kullanıcıları İçin Not
Proje mülakat gereksinimleri doğrultusunda **MSSQL (Microsoft SQL Server)** kullanılarak geliştirilmiştir ve varsayılan bağlantı adresi Windows'a özel olan `(localdb)\mssqllocaldb` olarak ayarlanmıştır.
Eğer projeyi Mac veya Linux ortamında çalıştıracaksanız:
1. Bilgisayarınızda (Örn: Docker üzerinden) çalışan bir SQL Server ayağa kaldırın.
2. `appsettings.json` dosyası içerisindeki `DefaultConnection` değerini kendi SQL Server bilgilerinize göre (Örn: `Server=localhost,1433;User Id=sa;Password=...;TrustServerCertificate=True`) güncelleyin.
3. `dotnet run` komutunu çalıştırdığınızda veritabanı ve tüm Seed verileri aynı şekilde otomatik olarak kurulacaktır.

---

## 🔑 Test Kullanıcıları ve Şifreler

Cookie Authentication altyapısı sayesinde şifreler veritabanında Hash'lenerek (güvenli bir şekilde) saklanmıştır. Projeyi test etmek için aşağıdaki varsayılan hesapları kullanabilirsiniz:

| Rol | Email (Kullanıcı Adı) | Şifre | Yetkiler |
| :--- | :--- | :--- | :--- |
| **Standart Kullanıcı** | `ahmet@test.com` | `123456` | Sadece ceza girişi yapabilir. Kendi girdiği cezaları görebilir. TC Kimlik No göremez. |
| **Yönetici** | `ayse@test.com` | `123456` | Tüm cezaları ve **sürücü TC Kimlik numaralarını** görebilir. Cezaları onaylayıp/reddedebilir. |
| **Finansman** | `fatma@test.com` | `123456` | Yöneticiden onay almış cezaları tamamlayıp süreci bitirebilir. TC Kimlik No göremez. |

---

## 📖 Sistemin Detaylı Kullanım Rehberi (Adım Adım)

Projenin tüm yeteneklerini keşfetmek ve test etmek için aşağıdaki adımları uygulayabilirsiniz:

### 1. Sisteme Giriş (Login)
Uygulamayı açtığınızda sizi bir Login ekranı karşılar. Yukarıdaki 3 farklı roldeki test kullanıcılarından biriyle giriş yapabilirsiniz. Şifrelerin tamamı `123456` olarak belirlenmiştir. Girdiğiniz role göre sol menüdeki butonlar ve tabloda göreceğiniz veriler dinamik olarak değişir.

### 2. Yeni Ceza Ekleme ve Araç Yönetimi
`ahmet@test.com` (Standart Kullanıcı) ile giriş yapın. Sol menüden **"Yeni Ceza Ekle"** sayfasına girin.
- **Plaka Kontrolü:** Formdaki plaka alanına bir plaka girin (Örn: 34ABC123). Plaka yazmayı bitirdiğinizde sistem arkaplanda (AJAX) plakayı sorgular. 
- Eğer araç veritabanında yoksa, formun altına dinamik olarak Marka ve Model seçme kutuları gelir.
- **Select2 ile Arama:** Yüzlerce marka ve model içinde kaybolmadan, doğrudan kutuya yazarak (Örn: "Renault") seçiminizi yapabilirsiniz. 
- Formu kaydedin. SweetAlert2 ile "Başarılı" bildirimi alacaksınız ve eklediğiniz ceza artık **"Yeni"** statüsündedir.

### 3. Dashboard ve Ceza Listesi (Rol Farkları)
Sistemden çıkış yapıp `ayse@test.com` (Yönetici) ile girin.
- Dashboard'da o güne kadar kesilmiş tüm cezaları ve durumlarını görebilirsiniz.
- Tabloda herhangi bir satıra tıkladığınızda cezanın **Detay Modal'ı (Popup)** açılır. 
- **Güvenlik Testi:** Yalnızca Yönetici hesabındayken "Sürücü TC No" alanı görünür. Standart Kullanıcı veya Finansman ile girdiğinizde bu alan HTML seviyesinde bile "***" (Gizli) olarak render edilir.

### 4. Onay Mekanizması ve Finansman
Yönetici panosunda, henüz onaylanmamış ("Yeni" statüsünde) cezaların sağ tarafında **✓ (Onayla)** ve **X (Reddet)** butonları bulunur.
- Yönetici cezayı onayladığında, cezanın statüsü anında **"Finans Onayında"** durumuna geçer.
- Sistemden çıkıp `fatma@test.com` (Finansman) ile giriş yapın. Finansman sadece tahsilatı yapılacak olan cezalarla ilgilenir. İlgili cezaya tıklayıp onayladığında ceza **"Tamamlandı"** olur ve süreç biter.

### 5. Detay ve Log Takibi (Audit Log)
Cezanın yaşam döngüsü boyunca geçirdiği tüm aşamalar loglanır. 
- Herhangi bir role sahipken, ceza listesinde satırın en sağındaki **Log (Göz) ikonuna** veya Ceza Geçmişi sayfasına tıklayın.
- Karşınıza çıkan pencerede: *"Ahmet cezayı ekledi (Saat 10:00)"*, *"Ayşe cezayı inceledi ve onayladı (Saat 11:30)"*, *"Fatma tahsilatı yaptı (Saat 14:00)"* gibi tüm tarihçeyi kimin yaptığını detaylarıyla görebilirsiniz.
- Eğer yönetici cezayı iptal ederse, girdiği "İptal Nedeni" de bu log ekranında listelenir.

---

## ✨ Öne Çıkan Özellikler

### 🛡️ Rol Tabanlı Yetkilendirme ve Veri Güvenliği
- **Standart Kullanıcı:** Sadece yeni ceza kaydı girebilir.
- **Yönetici:** Sistemdeki tüm cezaları görebilir. **"İhlali yapan sürücünün TC Kimlik Numarası"** sadece yöneticiler tarafından görüntülenebilir (HTML kaynak kodundan dahi gizlenmiştir). Cezaları onaylayıp Finansman aşamasına aktarabilir veya reddedebilir.
- **Finansman:** Yöneticinin onayladığı cezaları ödenmek üzere onaylar ve süreci tamamlar.

### 🔄 Akıllı Araç Yönetimi (Vehicle Management)
- Kullanıcıyı harici bir araç yönetim paneliyle yormadan, ceza girişi sırasında yazılan plaka sistemde yoksa **otomatik olarak veritabanına araç olarak kaydedilir**.

### 🎨 Gelişmiş Kullanıcı Deneyimi (UX)
- Klasik hata ve yönlendirme sayfaları yerine **SweetAlert2** ile şık Toast/Pop-up bildirimleri.
- İçerisinde yüzlerce veri bulunan (İlçe, Model vb.) listelerde kaybolmamak için **Select2** entegrasyonu ile akıllı metin araması.

---

## ⚡ Dinamik Seeding (Yaşayan Veritabanı ve Büyük Veri)

Sistemin tüm özelliklerinin anında test edilebilmesi için projeye kapsamlı bir veri seti (Seeding) entegre edilmiştir. Uygulama ilk kez çalıştırıldığında:
- **İl ve İlçeler:** `Data/SeedData/ilce.json` dosyasından okunarak Türkiye'nin **81 ili ve 900+ ilçesi** otomatik olarak veritabanına aktarılır.
- **Marka ve Modeller:** `Data/SeedData/arac_listesi.json` dosyasından okunarak otomotiv dünyasındaki **150'den fazla gerçek marka ve binlerce model** veritabanına işlenir.
- **Rastgele Test Verisi:** Bu gerçek veriler harmanlanarak sistemde rastgele 10 adet test aracı ve bu araçlara kesilmiş 20 farklı trafik cezası (farklı onay durumlarında) otomatik üretilir. Yani projeyi çalıştırdığınız an yaşayan, dopdolu bir dashboard sizi karşılar!

---

## 🛠️ Kullanılan Teknolojiler & Mimari

- **Framework:** .NET 9.0 (ASP.NET Core MVC)
- **Veritabanı:** Microsoft SQL Server (LocalDB)
- **ORM:** Entity Framework Core (Code-First)
- **Validasyon:** FluentValidation (Server-side & Client-side uyumlu)
- **Frontend UI:** Bootstrap 5.3, HTML5, CSS3
- **Frontend Kütüphaneleri:** jQuery, Select2, SweetAlert2, FontAwesome
- **Kimlik Doğrulama:** Cookie Authentication

---

## 🏗️ Veritabanı Diyagramı (Kavramsal Mimari)

- `Users`: Kullanıcı bilgileri ve şifre hashleri.
- `Roles`: Standart Kullanıcı, Yönetici, Finansman.
- `Vehicles`: Araç ruhsat bilgileri, Plaka, TC/VKN.
- `TrafficFines`: Trafik cezası ana kayıt tablosu.
- `TrafficFineHistories`: Cezalardaki statü değişimlerinin log tablosu.
- `Brands` & `Models`: JSON'dan beslenen marka/model havuzu.
- `Cities` & `Districts`: İl ve İlçe havuzu.
- `FineTypes`: Ceza maddeleri ve güncel tutarları (Örn: 47/1-b Kırmızı Işık).

---
<div align="center">
  <p>Bu proje mülakat (Case Study) yönergelerine tam uyumlu olarak, en iyi mühendislik pratikleri (Clean Code, Solid Prensipleri, Güvenlik) gözetilerek geliştirilmiştir.</p>
</div>


