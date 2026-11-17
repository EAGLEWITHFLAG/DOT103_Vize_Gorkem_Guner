using System;

namespace ZombiZindani
{
    class Program
    {
        // ============================ SABİTLER ============================
        const int MAX_TUR = 40;             // Fail-safe: bir savaş en fazla 40 tur sürebilir
        const int BOSS1_ADIM = 5;           // 5. adımda 1. boss (Kasap) çıkar
        const int BOSS2_ADIM = 10;          // 10. adımda son boss (Zombi Kral) çıkar
        const int IKSIR_GUCU = 40;          // Bir iksir 40 can doldurur
        const double KRITIK_CARPAN = 1.5;   // Kritik vuruşta hasar 1.5 katına çıkar

        // ==================== OYUNCU DURUM DEĞİŞKENLERİ ====================
        static string oyuncuAdi = "Hayatta Kalan";
        static int oyuncuCan = 100;         // HP
        static int maxCan = 100;            // Can en fazla bu kadar olabilir
        static int saldiriGucu = 10;        // ATK
        static int altin = 0;
        static int iksirSayisi = 2;
        static int adim = 0;                // Zindanda kaçıncı adımdayız
        static int oldurulenZombi = 0;
        static bool oyunAktif = true;       // false olunca ana oyun döngüsü biter

        // Random nesnesi döngülerin DIŞINDA, programda sadece BİR KEZ oluşturuluyor
        static Random zar = new Random();

        // ============================ DİZİLER ============================
        // Envanter (çanta): 6 yuva var, "" olan yuva boş demek
        static string[] envanter = { "Beyzbol Sopası", "", "", "", "", "" };

        // Düşman listesi: aynı sıradaki elemanlar aynı zombiye ait
        static string[] zombiIsimleri = { "Yürüyen Zombi", "Koşucu Zombi", "Şişko Zombi", "Asker Zombi" };
        static int[] zombiCanlari = { 28, 20, 45, 35 };
        static int[] zombiGucleri = { 7, 9, 5, 8 };

        // Sandıklardan sırayla çıkan silahlar ve saldırıya ekledikleri güç
        static string[] silahlar = { "Çivili Sopa", "Yangın Baltası", "Katana" };
        static int[] silahBonuslari = { 3, 5, 7 };
        static int bulunanSilahSayisi = 0;

        // Tuzak mesajları (her seferinde rastgele biri seçilir)
        static string[] tuzaklar =
        {
            "Yerden sivri kazıklar fırladı!",
            "Tavandan kocaman bir taş düştü!",
            "Zehirli bir iğneye bastın!",
            "Çürük tahtalar ayağının altında kırıldı!"
        };

        // Kavşaklarda duyulan sesler (sadece atmosfer için)
        static string[] sesler =
        {
            "Uzaktan inleme sesleri geliyor...",
            "Tavandan su damlıyor... tıp... tıp...",
            "Duvarda kanla yazılmış: GERİ DÖN!",
            "Bir yerlerde zincir şıngırdıyor.",
            "Burnuna çürük et kokusu geliyor.",
            "Meşalen titriyor, gölgeler sanki kıpırdıyor..."
        };

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;   // Türkçe karakterler düzgün görünsün

            BaslikCiz();

            Console.Write("Karakterinin adı ne? ");
            string isim = GirdiAl();
            if (isim != "")
            {
                oyuncuAdi = isim;
            }

            // Ana menüden "Çıkış" seçilirse oyun hiç başlamaz
            if (AnaMenu() == false)
            {
                oyunAktif = false;
            }

            // ==================== ANA OYUN DÖNGÜSÜ (GAME LOOP) ====================
            while (oyunAktif)
            {
                adim++;
                Console.Clear();
                DurumEkraniCiz();

                if (adim == BOSS1_ADIM)
                {
                    // ---------- 1. BOSS: KASAP ----------
                    RenkliYaz("Koridorun sonunda kanlı bir mutfak kapısı var...", ConsoleColor.DarkRed);
                    Console.WriteLine("İçeriden balta sesleri geliyor. Geri dönüş yok, kapıyı itiyorsun!");
                    DevamEt();
                    string sonuc = Savas("KASAP", 100, 8, true);

                    if (sonuc == "kazandi")
                    {
                        RenkliYaz("Kasap'ı alt ettin! Elinden düşen satırı aldın.", ConsoleColor.Green);
                        if (EnvantereEkle("Kasap Satırı"))
                        {
                            saldiriGucu += 5;
                            RenkliYaz("Kasap Satırı: saldırı gücün +5 arttı!", ConsoleColor.Green);
                        }
                        altin += 40;
                        iksirSayisi++;
                        RenkliYaz("Mutfak dolabında 40 altın ve 1 iksir buldun.", ConsoleColor.Yellow);
                    }
                }
                else if (adim == BOSS2_ADIM)
                {
                    // ---------- SON BOSS: ZOMBİ KRAL ----------
                    RenkliYaz("Devasa bir taht salonuna girdin. Arkandaki kapı kilitlendi!", ConsoleColor.DarkRed);
                    Console.WriteLine("Tahttaki dev zombi yavaşça ayağa kalkıyor... ZOMBİ KRAL!");
                    Console.WriteLine("Tahtın arkasında ÇIKIŞ KAPISI var. Önünde tek engel o.");
                    DevamEt();
                    string sonuc = Savas("ZOMBİ KRAL", 160, 11, true);

                    if (sonuc == "kazandi")
                    {
                        OyunSonu("zafer");
                        oyunAktif = false;
                    }
                    else if (sonuc == "berabere")
                    {
                        OyunSonu("berabere");
                        oyunAktif = false;
                    }
                }
                else
                {
                    // ---------- NORMAL ADIM: önce kavşak seçimi, sonra olay ----------
                    bool odaMi = KavsakMenusu();
                    OlayYasat(odaMi);
                }

                // Can bittiyse oyun kaybedilir
                if (oyuncuCan <= 0)
                {
                    OyunSonu("yenilgi");
                    oyunAktif = false;
                }

                // Oyun sürüyorsa ekran temizlenmeden önce oyuncu yazıları okuyabilsin
                if (oyunAktif)
                {
                    DevamEt();
                }
            }

            Console.WriteLine();
            Console.Write("Oyun kapandı. Çıkmak için Enter'a bas...");
            Console.ReadLine();
        }

        // Oyunun açılış başlığını çizer
        static void BaslikCiz()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("##################################################");
            Console.WriteLine("#                                                #");
            Console.WriteLine("#           Z O M B İ   Z İ N D A N I            #");
            Console.WriteLine("#                                                #");
            Console.WriteLine("#     Zombileri temizle, bossları yen ve         #");
            Console.WriteLine("#     zindandan canlı çıkmaya çalış!             #");
            Console.WriteLine("#                                                #");
            Console.WriteLine("##################################################");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Ana menü: do-while sayesinde en az bir kez gösterilir ve
        // geçerli bir seçim gelene kadar tekrar eder.
        static bool AnaMenu()
        {
            string secim;
            do
            {
                Console.WriteLine();
                Console.WriteLine("[1] Zindana gir");
                Console.WriteLine("[2] Nasıl oynanır?");
                Console.WriteLine("[3] Çıkış");
                Console.Write("Seçimin: ");
                secim = GirdiAl();

                if (secim == "2")
                {
                    Console.WriteLine();
                    RenkliYaz("--- NASIL OYNANIR? ---", ConsoleColor.Cyan);
                    Console.WriteLine("- Her adımda bir kavşağa gelirsin: sol, düz ya da sağ yolu seçersin.");
                    Console.WriteLine("- Koridorlarda zombi çok olur, odalarda sandık bulma şansın daha yüksek.");
                    Console.WriteLine("- Savaşta 1 = Saldır, 2 = Savun, 3 = İksir iç.");
                    Console.WriteLine("- Zar 18-20 gelirse KRİTİK vurursun ve düşman bir tur sersemler.");
                    Console.WriteLine("- Boss 'güç topluyor' derse hemen SAVUN, yoksa çok sert vurur!");
                    Console.WriteLine("- " + BOSS1_ADIM + ". adımda Kasap, " + BOSS2_ADIM + ". adımda Zombi Kral seni bekliyor.");
                }
                else if (secim != "1" && secim != "3")
                {
                    RenkliYaz("Geçersiz seçim! Lütfen 1, 2 veya 3 yaz.", ConsoleColor.Red);
                }
            } while (secim != "1" && secim != "3");

            return secim == "1";   // 1 seçildiyse true (oyna), 3 seçildiyse false (çık)
        }

        // Oyuncunun durum panelini çizer (can, saldırı, altın, iksir, çanta)
        static void DurumEkraniCiz()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine(" ZOMBİ ZİNDANI                       Adım: " + adim + " / " + BOSS2_ADIM);
            Console.WriteLine("==================================================");
            Console.Write(" " + oyuncuAdi + "  Can: ");
            CanBariCiz(oyuncuCan, maxCan);
            Console.WriteLine(" Saldırı: " + saldiriGucu + "   Altın: " + altin + "   İksir: " + iksirSayisi + "   Ölen zombi: " + oldurulenZombi);

            // Çantadaki dolu yuvaları for döngüsüyle yazdır
            Console.Write(" Çanta: ");
            for (int i = 0; i < envanter.Length; i++)
            {
                if (envanter[i] != "")
                {
                    Console.Write("[" + envanter[i] + "] ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine();
        }

        // Can barını 20 kutu olarak çizer. Örnek: [##############------] 72/100
        static void CanBariCiz(int mevcut, int enFazla)
        {
            if (mevcut < 0)
            {
                mevcut = 0;   // Ekranda eksi can görünmesin
            }

            int doluKutu = mevcut * 20 / enFazla;

            // Can azaldıkça bar yeşilden kırmızıya döner
            if (doluKutu > 10)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (doluKutu > 5)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write("[");
            for (int i = 0; i < 20; i++)
            {
                if (i < doluKutu)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write("-");
                }
            }
            Console.Write("] ");
            Console.ResetColor();
            Console.WriteLine(mevcut + "/" + enFazla);
        }

        // Kavşağı zarla oluşturur ve oyuncuya do-while menüsüyle sorar.
        // Oyuncu odaya girerse true, koridora girerse false döndürür.
        static bool KavsakMenusu()
        {
            // Rastgele bir ortam sesi (Next'te üst sınır dahil değil: 0 ile Length-1 arası)
            Console.WriteLine(sesler[zar.Next(0, sesler.Length)]);

            // 0 gelirse üç yol var, 1 gelirse önü duvar (sadece sağ ve sol)
            bool duzAcik = (zar.Next(0, 2) == 0);

            // Sol ve sağ taraf oda mı koridor mu, onu da zar belirliyor
            bool solOda = (zar.Next(0, 2) == 0);
            bool sagOda = (zar.Next(0, 2) == 0);

            string solYazi = "Sol koridora gir";
            if (solOda)
            {
                solYazi = "Sol odaya gir";
            }

            string sagYazi = "Sağ koridora gir";
            if (sagOda)
            {
                sagYazi = "Sağ odaya gir";
            }

            if (duzAcik)
            {
                Console.WriteLine("Koridor üç yola ayrılıyor.");
            }
            else
            {
                Console.WriteLine("Önün duvar! Yol sağa ve sola ayrılıyor.");
            }

            bool odaMi = false;
            bool secimYapildi = false;

            do
            {
                Console.WriteLine();
                Console.WriteLine("[1] " + solYazi);
                if (duzAcik)
                {
                    Console.WriteLine("[2] Koridorda düz ilerle");
                }
                else
                {
                    RenkliYaz("[2] (duvar, düz gidilemez)", ConsoleColor.DarkGray);
                }
                Console.WriteLine("[3] " + sagYazi);
                Console.WriteLine("[4] İksir iç (" + iksirSayisi + " adet)");
                Console.Write("Seçimin: ");
                string secim = GirdiAl();

                switch (secim)
                {
                    case "1":
                        odaMi = solOda;
                        secimYapildi = true;
                        break;
                    case "2":
                        if (duzAcik)
                        {
                            odaMi = false;
                            secimYapildi = true;
                        }
                        else
                        {
                            RenkliYaz("Önünde duvar var, düz gidemezsin!", ConsoleColor.Red);
                        }
                        break;
                    case "3":
                        odaMi = sagOda;
                        secimYapildi = true;
                        break;
                    case "4":
                        if (iksirSayisi > 0 && oyuncuCan < maxCan)
                        {
                            IksirIc();
                        }
                        else
                        {
                            RenkliYaz("İksir içemezsin! (İksirin yok ya da canın zaten dolu)", ConsoleColor.Red);
                        }
                        break;
                    default:
                        RenkliYaz("Geçersiz seçim! 1, 2, 3 veya 4 yazmalısın.", ConsoleColor.Red);
                        break;
                }
            } while (secimYapildi == false);

            Console.WriteLine();
            return odaMi;
        }

        // Girilen yerde ne olacağını zar belirler (1-100 arası şans).
        // Odalarda sandık, koridorlarda zombi çıkma ihtimali daha yüksek.
        static void OlayYasat(bool odaMi)
        {
            int sans = zar.Next(1, 101);   // 1-100 arası (101 dahil değil)

            if (odaMi)
            {
                Console.WriteLine("Gıcırdayan kapıyı itip odaya girdin...");
                if (sans <= 30)
                {
                    ZombiSaldirisi();      // %30 zombi
                }
                else if (sans <= 65)
                {
                    SandikAc();            // %35 sandık
                }
                else if (sans <= 85)
                {
                    TuzakOlayi();          // %20 tuzak
                }
                else
                {
                    BosYer();              // %15 boş
                }
            }
            else
            {
                Console.WriteLine("Karanlık koridorda ilerliyorsun...");
                if (sans <= 55)
                {
                    ZombiSaldirisi();      // %55 zombi
                }
                else if (sans <= 70)
                {
                    TuzakOlayi();          // %15 tuzak
                }
                else if (sans <= 90)
                {
                    BosYer();              // %20 boş
                }
                else
                {
                    SandikAc();            // %10 sandık
                }
            }
        }

        // Düşman dizisinden rastgele bir zombi seçip savaş başlatır
        static void ZombiSaldirisi()
        {
            // 4 zombi var: Next(0, 4) -> 0, 1, 2 veya 3 verir (4 dahil değil)
            int zombiNo = zar.Next(0, zombiIsimleri.Length);
            string sonuc = Savas(zombiIsimleri[zombiNo], zombiCanlari[zombiNo], zombiGucleri[zombiNo], false);

            if (sonuc == "kazandi")
            {
                oldurulenZombi++;
                int ganimet = zar.Next(5, 16);   // 5-15 altın
                altin += ganimet;
                RenkliYaz("Zombinin üstünden " + ganimet + " altın düştü!", ConsoleColor.Yellow);
            }
            else if (sonuc == "berabere")
            {
                Console.WriteLine("Zombi topallayarak karanlığa kaçtı.");
            }
        }

        // Sandık: altın, iksir, silah ya da sürpriz zombi çıkabilir
        static void SandikAc()
        {
            Console.WriteLine("Tozlu bir sandık buldun. Kapağını yavaşça kaldırıyorsun...");
            int sans = zar.Next(1, 101);

            if (sans <= 35)
            {
                int bulunanAltin = zar.Next(10, 31);   // 10-30 altın
                altin += bulunanAltin;
                RenkliYaz("Sandıktan " + bulunanAltin + " altın çıktı!", ConsoleColor.Yellow);
            }
            else if (sans <= 65)
            {
                iksirSayisi++;
                RenkliYaz("Sandıktan bir can iksiri çıktı! (+1 iksir)", ConsoleColor.Green);
            }
            else if (sans <= 85)
            {
                // Silahlar sırayla bulunur, hepsi bulunduysa altın verilir
                if (bulunanSilahSayisi < silahlar.Length)
                {
                    string yeniSilah = silahlar[bulunanSilahSayisi];
                    int bonus = silahBonuslari[bulunanSilahSayisi];
                    if (EnvantereEkle(yeniSilah))
                    {
                        saldiriGucu += bonus;
                        RenkliYaz(yeniSilah + " buldun! Saldırı gücün +" + bonus + " arttı.", ConsoleColor.Green);
                    }
                    bulunanSilahSayisi++;
                }
                else
                {
                    altin += 25;
                    RenkliYaz("Sandıkta altın bir yüzük vardı! (+25 altın)", ConsoleColor.Yellow);
                }
            }
            else
            {
                RenkliYaz("SANDIĞIN İÇİNDEN BİR ZOMBİ FIRLADI!", ConsoleColor.Red);
                ZombiSaldirisi();
            }
        }

        // Tuzak: d20 zarı 10 veya üstü gelirse oyuncu kurtulur
        static void TuzakOlayi()
        {
            RenkliYaz(tuzaklar[zar.Next(0, tuzaklar.Length)], ConsoleColor.Red);
            Console.WriteLine("Kaçmak için zar atıyorsun... (10 ve üstü lazım)");

            int zarSonucu = zar.Next(1, 21);   // d20: 1-20 arası (21 dahil değil)
            Console.WriteLine("Zar: " + zarSonucu);

            if (zarSonucu >= 10)
            {
                RenkliYaz("Son anda kenara atladın, kurtuldun!", ConsoleColor.Green);
            }
            else
            {
                int hasar = zar.Next(8, 16);   // 8-15 hasar
                oyuncuCan -= hasar;
                RenkliYaz("Kaçamadın! " + hasar + " hasar aldın.", ConsoleColor.Red);
            }
        }

        // Boş yer: ya biraz dinlenirsin ya da yerde altın bulursun
        static void BosYer()
        {
            if (zar.Next(0, 2) == 0)
            {
                Console.WriteLine("Burası sessiz ve boş. Duvara yaslanıp biraz soluklandın.");
                oyuncuCan += 10;
                if (oyuncuCan > maxCan)
                {
                    oyuncuCan = maxCan;
                }
                RenkliYaz("+10 can", ConsoleColor.Green);
            }
            else
            {
                int bulunanAltin = zar.Next(3, 9);   // 3-8 altın
                altin += bulunanAltin;
                RenkliYaz("Kimse yok... Yerde parlayan " + bulunanAltin + " altın buldun!", ConsoleColor.Yellow);
            }
        }

        // SIRA TABANLI SAVAŞ MOTORU
        // Geriye "kazandi", "kaybetti" veya "berabere" döndürür.
        static string Savas(string dusmanAdi, int dusmanCan, int dusmanGuc, bool bossMu)
        {
            int dusmanMaxCan = dusmanCan;
            int tur = 0;
            bool dusmanSersem = false;   // true ise düşman bu tur saldıramaz
            bool gucTopluyor = false;    // true ise boss sonraki tur ağır saldırı yapacak
            string sonuc = "kaybetti";

            Console.WriteLine();
            RenkliYaz(">>> " + dusmanAdi + " karşına çıktı! (Can: " + dusmanCan + ", Güç: " + dusmanGuc + ") <<<", ConsoleColor.Red);

            while (oyuncuCan > 0)
            {
                tur++;

                // FAIL-SAFE: savaş MAX_TUR'u geçerse berabere biter, oyun sonsuza kadar dönmez
                if (tur > MAX_TUR)
                {
                    RenkliYaz(MAX_TUR + " tur geçti! İkiniz de yorgun düştünüz. Savaş BERABERE.", ConsoleColor.DarkYellow);
                    sonuc = "berabere";
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("---------- TUR " + tur + " | " + dusmanAdi + " ----------");
                Console.Write("Sen    ");
                CanBariCiz(oyuncuCan, maxCan);
                Console.Write("Düşman ");
                CanBariCiz(dusmanCan, dusmanMaxCan);

                // ---------- OYUNCUNUN SIRASI ----------
                string secim = SavasMenusu();
                bool savunmada = false;

                if (secim == "1")
                {
                    int zarSonucu = zar.Next(1, 21);   // d20: 1-20 arası (21 dahil değil)
                    int verilenHasar = HasarHesapla(saldiriGucu, zarSonucu);
                    dusmanCan -= verilenHasar;
                    Console.WriteLine("Saldırdın! " + dusmanAdi + " " + verilenHasar + " hasar aldı.");

                    if (dusmanCan <= 0)
                    {
                        RenkliYaz(dusmanAdi + " yere yığıldı!", ConsoleColor.Green);
                        sonuc = "kazandi";
                        break;   // Düşman öldü: savaş döngüsünü HEMEN kır
                    }

                    if (zarSonucu >= 18)
                    {
                        dusmanSersem = true;   // Kritik vuruş düşmanı sersemletir
                    }
                }
                else if (secim == "2")
                {
                    savunmada = true;
                    Console.WriteLine("Kalkanını kaldırdın, saldırıyı bekliyorsun...");
                }
                else
                {
                    IksirIc();
                }

                // ---------- DÜŞMANIN SIRASI ----------

                // Sersemlemiş düşman bu tur saldıramaz: turun geri kalanını atla
                if (dusmanSersem)
                {
                    RenkliYaz(dusmanAdi + " sersemledi, bu tur saldıramıyor!", ConsoleColor.Cyan);
                    dusmanSersem = false;
                    continue;
                }

                // Boss %25 ihtimalle güç toplar: bu tur saldırmaz, sonraki tur sert vurur
                if (bossMu && gucTopluyor == false && zar.Next(1, 101) <= 25)
                {
                    gucTopluyor = true;
                    RenkliYaz(dusmanAdi + " güç topluyor! Sonraki saldırısı çok sert olacak. SAVUN!", ConsoleColor.Magenta);
                    continue;
                }

                // Önceki tur güç topladıysa şimdi ağır saldırı yapar
                int carpan = 1;
                if (gucTopluyor)
                {
                    gucTopluyor = false;
                    RenkliYaz(dusmanAdi + " AĞIR SALDIRI yapıyor!", ConsoleColor.Magenta);

                    if (savunmada)
                    {
                        dusmanSersem = true;
                        RenkliYaz("Kalkanınla tamamen durdurdun! " + dusmanAdi + " dengesini kaybetti.", ConsoleColor.Cyan);
                        continue;   // Savunma işe yaradı: hasar yok, turu atla
                    }
                    carpan = 2;     // Savunmadıysan 2 kat hasar yersin
                }

                int dusmanZari = zar.Next(1, 21);
                int alinanHasar = HasarHesapla(dusmanGuc, dusmanZari) * carpan;

                if (savunmada)
                {
                    alinanHasar = alinanHasar / 2;   // Savunmadayken normal hasar yarıya iner
                    Console.WriteLine("Kalkanın hasarın yarısını engelledi.");
                }

                oyuncuCan -= alinanHasar;
                RenkliYaz(dusmanAdi + " sana " + alinanHasar + " hasar verdi.", ConsoleColor.Red);
            }

            if (sonuc == "kaybetti")
            {
                RenkliYaz(dusmanAdi + " seni yere serdi...", ConsoleColor.DarkRed);
            }

            return sonuc;
        }

        // Savaş menüsü: geçerli seçim gelene kadar do-while ile tekrar sorar.
        // Yanlış tuşta program çökmez, oyuncu uyarılır.
        static string SavasMenusu()
        {
            string secim;
            bool gecerli = false;

            do
            {
                Console.WriteLine("[1] Saldır   [2] Savun   [3] İksir iç (" + iksirSayisi + ")");
                Console.Write("Seçimin: ");
                secim = GirdiAl();

                if (secim == "1" || secim == "2")
                {
                    gecerli = true;
                }
                else if (secim == "3" && iksirSayisi > 0 && oyuncuCan < maxCan)
                {
                    gecerli = true;
                }
                else if (secim == "3")
                {
                    RenkliYaz("İksir içemezsin! (İksirin yok ya da canın zaten dolu)", ConsoleColor.Red);
                }
                else
                {
                    RenkliYaz("Geçersiz seçim! Lütfen 1, 2 veya 3 yaz.", ConsoleColor.Red);
                }
            } while (gecerli == false);

            return secim;
        }

        // Hasar hesaplama: güce 0-5 arası ek hasar eklenir, sonra zara bakılır.
        // Zar 1 ise ıska (0 hasar), 18-20 ise kritik vuruş (1.5 kat).
        static int HasarHesapla(int guc, int zarSonucu)
        {
            int hasar = guc + zar.Next(0, 6);   // 0-5 arası ek hasar (6 dahil değil)

            if (zarSonucu == 1)
            {
                hasar = 0;
                Console.WriteLine("  Zar: 1 -> ISKA!");
            }
            else if (zarSonucu >= 18)
            {
                // double ile çarpıp sonucu tekrar int'e çeviriyoruz (tür dönüşümü)
                hasar = (int)(hasar * KRITIK_CARPAN);
                RenkliYaz("  Zar: " + zarSonucu + " -> KRİTİK VURUŞ!", ConsoleColor.Yellow);
            }
            else
            {
                Console.WriteLine("  Zar: " + zarSonucu);
            }

            return hasar;
        }

        // İksir içer: can artar ama maxCan'ı geçemez
        static void IksirIc()
        {
            iksirSayisi--;
            oyuncuCan += IKSIR_GUCU;
            if (oyuncuCan > maxCan)
            {
                oyuncuCan = maxCan;
            }
            RenkliYaz("İksiri içtin! Canın: " + oyuncuCan + "/" + maxCan, ConsoleColor.Green);
        }

        // Eşyayı çantadaki ilk boş yuvaya koyar. Çanta doluysa false döner.
        static bool EnvantereEkle(string esya)
        {
            for (int i = 0; i < envanter.Length; i++)
            {
                if (envanter[i] == "")
                {
                    envanter[i] = esya;
                    return true;
                }
            }

            RenkliYaz("Çantan dolu! " + esya + " yerde kaldı.", ConsoleColor.Red);
            return false;
        }

        // Oyun bitince sonucu ve istatistikleri gösterir
        static void OyunSonu(string durum)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            if (durum == "zafer")
            {
                RenkliYaz("Zombi Kral'ın boynundaki anahtarı aldın ve çıkış kapısını açtın.", ConsoleColor.Green);
                RenkliYaz("Güneş ışığı yüzüne vuruyor... ZİNDANDAN KURTULDUN! TEBRİKLER " + oyuncuAdi + "!", ConsoleColor.Green);
            }
            else if (durum == "berabere")
            {
                RenkliYaz("Zombi Kral karanlığa çekildi ama anahtarı da götürdü.", ConsoleColor.DarkYellow);
                RenkliYaz("Hayattasın ama zindandan çıkamadın. Oyun BERABERE bitti.", ConsoleColor.DarkYellow);
            }
            else
            {
                RenkliYaz("Gözlerin karardı... Zombilere yem oldun. OYUN BİTTİ!", ConsoleColor.Red);
            }
            Console.WriteLine("==================================================");
            Console.WriteLine("Ulaştığın adım   : " + adim);
            Console.WriteLine("Öldürdüğün zombi : " + oldurulenZombi);
            Console.WriteLine("Topladığın altın : " + altin);
            Console.WriteLine("==================================================");
        }

        // Yazıyı istenen renkte yazar, sonra rengi eski haline döndürür
        static void RenkliYaz(string mesaj, ConsoleColor renk)
        {
            Console.ForegroundColor = renk;
            Console.WriteLine(mesaj);
            Console.ResetColor();
        }

        // Klavyeden güvenli girdi alır: boşlukları siler, null gelirse "" yapar
        static string GirdiAl()
        {
            string girdi = Console.ReadLine();
            if (girdi == null)
            {
                girdi = "";
            }
            return girdi.Trim();
        }

        // Oyuncu Enter'a basana kadar bekler
        static void DevamEt()
        {
            Console.WriteLine();
            Console.Write("Devam etmek için Enter'a bas...");
            Console.ReadLine();
        }
    }
}