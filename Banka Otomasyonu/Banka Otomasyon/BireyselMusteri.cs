using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banka_Otomasyon
{
    public class BireyselMusteri : Musteri
    {
        public const decimal HAVALEUCRETORANI = 0.02M;
        public BireyselMusteri()
        {
            MusteriTipi = "Bireysel";
        }
      
    }
}
