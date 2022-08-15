using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka_Otomasyon
{
    public abstract class Musteri
    {
        public long MusteriNo { get; set; }
        public KimlikBilgisi kimlikBilgisi;
        public List<Hesap> Hesaplar = new List<Hesap>();
        public int validasyonKontrol = 0;
        public string MusteriTipi { get; set; }
        public Musteri()
        {
            kimlikBilgisi = new KimlikBilgisi();
        }
        public virtual string MusteriValidasyon()
        {
            string HataMesaji = "";
            if (kimlikBilgisi.TCKimlikNo == 0)
            {
                HataMesaji += "TC Kimlik No Boş Bırakılamaz\n";
                validasyonKontrol++;
            }
            if (kimlikBilgisi.Ad == null || kimlikBilgisi.Ad == "")
            {
                HataMesaji += "Ad Boş Bırakılamaz\n";
                validasyonKontrol++;
            }
            if (kimlikBilgisi.Soyad == null || kimlikBilgisi.Soyad == "")
            {
                HataMesaji += "Soyad Boş Bırakılamaz\n";
                validasyonKontrol++;
            }
            if (kimlikBilgisi.telefon == null || kimlikBilgisi.telefon == "")
            {
                HataMesaji += "Telefon Numarası Boş Bırakılamaz\n";
                validasyonKontrol++;
            }
            if (kimlikBilgisi.adres == null || kimlikBilgisi.adres == "")
            {
                HataMesaji += "Adres Boş Bırakılamaz\n";
                validasyonKontrol++;
            }
            return HataMesaji;
        }
        public virtual void HesapEkle(Hesap h)
        {
            Hesaplar.Add(h);
        }
        public virtual void HesapKapat(Hesap h)
        {
            Hesaplar.Remove(h);
        }
    }
}
