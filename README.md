# DOT103_Vize_Gorkem_Guner – Zombi Zindanı

DOT 103 Programlamaya Giriş dersi uygulamalı ara sınav projesi: C# ile yazılmış, konsolda çalışan sıra tabanlı bir zindan oyunu.

- **Öğrenci:** Görkem Güner
- **Ders:** DOT 103 Programlamaya Giriş – Öğr. Gör. Bilge Kağan Pamuk
- **Program:** OSTİM Teknik Üniversitesi BTMYO, Dijital Oyun Tasarımı ve Teknolojileri

## Oyun

Oyuncu zombilerle dolu bir zindanda adım adım ilerler. Her adımda bir kavşağa gelir ve sol, düz ya da sağ yolu seçer. Odalarda sandık bulma, koridorlarda zombiyle karşılaşma ihtimali daha yüksektir; yolda tuzaklar da vardır. 5. adımda Kasap, 10. adımda Zombi Kral ile boss savaşı yapılır. Zombi Kral yenilirse çıkış kapısı açılır ve oyun kazanılır.

Proje, kılavuzdaki Seçenek B (dallanan zindan odaları) üzerine kurulu; savaşlarda Seçenek A'daki sıra tabanlı savaş (Saldır / Savun / İksir) kullanılıyor.

## Çalıştırma

1. `DOT103_Vize_Gorkem_Guner.sln` dosyasını Visual Studio ile aç.
2. **F5** ile başlat.

Gereksinim: Visual Studio ve .NET 11 SDK.

## Kontroller

| Ekran | Seçenekler |
|---|---|
| Ana menü | `1` Zindana gir · `2` Nasıl oynanır? · `3` Çıkış |
| Kavşak | `1` Sol · `2` Düz · `3` Sağ · `4` İksir iç |
| Savaş | `1` Saldır · `2` Savun · `3` İksir iç |

Her seçimden sonra Enter'a basılır. Geçersiz bir tuşta oyun çökmez; uyarı verip tekrar sorar.

## Kılavuzdaki 8 kural ve koddaki yerleri

Satır numaraları `DOT103_Vize_Gorkem_Guner/Program.cs` dosyasına göredir.

| # | Kural | Koddaki yeri |
|---|---|---|
| 1 | Ana oyun döngüsü | `while (oyunAktif)` (satır 82); oyun bitince `oyunAktif = false` (78, 121, 126, 140) |
| 2 | Durum değişkenleri | `oyuncuCan`, `saldiriGucu`, `altin`, `iksirSayisi` ve diğerleri (15–23) |
| 3 | Rastgele zar | `static Random zar = new Random();` döngülerin dışında bir kez (26); d20 zarı `zar.Next(1, 21)` → 1–20, üst sınır hariç (484, 558, 619) |
| 4 | Menü ve girdi güvenliği | do-while menüler: `AnaMenu` (173), `KavsakMenusu` (272), `SavasMenusu` (642); güvenli girdi: `GirdiAl` (762) |
| 5 | break / continue | Düşman ölünce `break` (567); sersemleme ve savunmada `continue` (592, 600, 614) |
| 6 | Fail-safe | `MAX_TUR = 40` (8); 40 turu geçen savaş berabere biter (538–543) |
| 7 | Dizi | `envanter` (30); `zombiIsimleri`, `zombiCanlari`, `zombiGucleri` (33–35); `silahlar`, `silahBonuslari` (38–39); `tuzaklar` (43); `sesler` (52) |
| 8 | Metotlar | `Main` dışında 19 metot; ör. `DurumEkraniCiz` (206), `Savas` (522), `HasarHesapla` (676) |

## Dosya yapısı

```
DOT103_Vize_Gorkem_Guner/
├── DOT103_Vize_Gorkem_Guner.sln
├── README.md
└── DOT103_Vize_Gorkem_Guner/
    ├── DOT103_Vize_Gorkem_Guner.csproj
    └── Program.cs        ← oyunun tüm kodu
```
