using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka_Otomasyon
{
    public class IslemBilgisi
    {
        public decimal Tutar { get; set; }
        public string Detay { get; set; }
        public DateTime IslemTarihi { get; set; }
        public decimal GunlukParaCekme { get; set; }
        public long kaynakHesapNo { get; set; }
        public long hedefHesapNo { get; set; }
        public decimal kaynakBakiye { get; set; }
        public decimal hedefBakiye { get; set; }
    }
}
