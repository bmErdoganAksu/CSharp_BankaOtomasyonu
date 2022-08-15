using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banka_Otomasyon
{

    public class Hesap
    {
        static string con = "Data Source=LAPTOP-9IQ5NO3T;Initial Catalog=Musteriler;Integrated Security=True";
        SqlConnection baglan = new SqlConnection(con);
        SqlCommand komut;
        SqlDataAdapter da;
        SqlDataReader dr;

        public long HesapNo { get; set; }
        public long Bakiye { get; set; }

        public List<IslemBilgisi> IslemBilgileri = new List<IslemBilgisi>();

       

        void bakiyeGetir(string tc,string hesapNo)
        {
            /////veritabanından gönderici hesaptaki bakiyeyi çekme kodu/////////////
            string BAKİYE;
            
            string text ="SELECT * FROM Hesap where tc_no=@tc_no and hesap_no=@hesap_no";
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            komut = new SqlCommand(text, baglan);//sorguyu komuta bağla
            komut.Parameters.AddWithValue("@tc_no", tc);//komuta bağladığın sorguyu tc no ya göre yap
            komut.Parameters.AddWithValue("@hesap_no", hesapNo);
            baglan.Open();
            dr = komut.ExecuteReader();
            ///Hesap no varsa farklı bir hesap no üret kodu
            if (dr.Read())
            {
                BAKİYE = dr["bakiye"].ToString();

                Bakiye = Convert.ToInt64(BAKİYE);

                baglan.Close();
                dr.Close();
            }
            baglan.Close();
            dr.Close();
        }

        void TransferBakiyeleriGetir(string hesapNo, Hesap h)
        {
            /////veritabanından gönderici hesaptaki bakiyeyi çekme kodu/////////////
            string BAKİYE;
            komut = new SqlCommand("SELECT * FROM Hesap where hesap_no='" + HesapNo + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            ///Hesap no varsa farklı bir hesap no üret kodu
            if (dr.Read())
            {
                BAKİYE = dr["bakiye"].ToString();

                h.Bakiye = Convert.ToInt64(BAKİYE);
                // MessageBox.Show("h.Bakiye :"+ h.Bakiye.ToString());
                baglan.Close();
                dr.Close();
            }
            else
            {
                MessageBox.Show("Veritabanından Bakiye Çekme Hatası");
                dr.Close();
                baglan.Close();
            }
            dr.Close();
            baglan.Close();
        }


        public void GondericiBakiyeGuncelle(string tc, string gonHesapNo, string alHesapNo, string tutar, IslemBilgisi ibG)
        {
            IslemBilgileri.Add(ibG);
            ibG.Detay = alHesapNo + " Nolu Hesaba Giden Transfer";
            Bakiye = Bakiye - Convert.ToInt64(tutar);
            //MessageBox.Show(Bakiye.ToString());
            /// veritabanına kayıt kodu/////////////////////////
            komut = new SqlCommand("SELECT * FROM Hesap where tc_no='" + tc + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                komut = new SqlCommand(
                             "UPDATE Hesap SET bakiye='" + Bakiye + "'" +
                             "WHERE hesap_no='" + gonHesapNo + "'AND tc_no='" + tc + "'" +
                             "UPDATE Hesap SET detay='" + ibG.Detay + "'" +
                             "WHERE hesap_no='" + gonHesapNo + "'AND tc_no='" + tc + "'", baglan);
                baglan.Open();
                komut.ExecuteNonQuery();//sonuçları çalıştır.
                baglan.Close();
            }
            else
            {
                MessageBox.Show("Gönderici Bakiye Güncelleme HATASI");
                baglan.Close();
                dr.Close();
            }
        }



        public void AliciBakiyeGuncelle(string tc, string gonHesapNo, string alHesapNo, string tutar, IslemBilgisi ibA)
        {
            IslemBilgileri.Add(ibA);
            ibA.Detay = gonHesapNo + " Nolu Hesaptan Gelen Transfer";
            Bakiye = Bakiye + Convert.ToInt64(tutar);
            //MessageBox.Show(Bakiye.ToString());
            /// veritabanına kayıt kodu/////////////////////////
            komut = new SqlCommand("SELECT * FROM Hesap where tc_no='" + tc + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            baglan.Close();

            komut = new SqlCommand(
                          "UPDATE Hesap SET bakiye='" + Bakiye + "'" +
                             "WHERE hesap_no='" + alHesapNo + "'AND tc_no='" + tc + "'" +
                             "UPDATE Hesap SET detay='" + ibA.Detay + "'" +
                             "WHERE hesap_no='" + alHesapNo + "'AND tc_no='" + tc + "'", baglan);
            baglan.Open();
            komut.ExecuteNonQuery();//sonuçları çalıştır.
            baglan.Close();
        }

        
        public void ParaCek(string tc, string hesapno, string bakiye, IslemBilgisi ib)
        {

            /////databaseden hesap numarasına göre hesaptaki bakiyeyi çekme kodu/////////////
            bakiyeGetir(tc,hesapno);
            ////////////////////////////////////////////////////////////////////
            IslemBilgileri.Add(ib);
            Bakiye = Bakiye - Convert.ToInt64(bakiye);
            //MessageBox.Show(Bakiye.ToString());
            /// veritabanına kayıt kodu/////////////////////////
            baglan.Open();
            dr = komut.ExecuteReader();
            komut = new SqlCommand(
                         "UPDATE Hesap SET bakiye='" + Bakiye + "'" +
                          "WHERE hesap_no = '" + hesapno + "' AND tc_no = '" + tc + "'"+
                         "UPDATE Hesap SET detay='" + ib.Detay + "'" +
                         "WHERE hesap_no = '" + hesapno + "' AND tc_no = '" + tc + "'", baglan);
            dr.Close();
            komut.ExecuteNonQuery();//sonuçları çalıştır.
            baglan.Close();
        }
        public void Iptal(IslemBilgisi ib)
        {
            IslemBilgileri.Remove(ib);
        }



        public void ParaYatir(string tc, string hesapno, string bakiye, IslemBilgisi ib)
        {
            /////databaseden hesap numarasına göre hesaptaki bakiyeyi çekme kodu/////////////
            bakiyeGetir(tc,hesapno);
            ////////////////////////////////////////////////////////////////////
            Bakiye = Bakiye + Convert.ToInt64(bakiye);
            IslemBilgileri.Add(ib);
            /// veritabanına kayıt kodu/////////////////////////
            baglan.Open();
            dr = komut.ExecuteReader();
                komut = new SqlCommand(
                         "UPDATE Hesap SET bakiye='" + Bakiye + "'" +
                         "WHERE hesap_no = '"+hesapno+"' AND tc_no = '"+tc+"'"+
                         "UPDATE Hesap SET detay='" + ib.Detay + "'" +
                         "WHERE hesap_no = '" + hesapno + "' AND tc_no = '" + tc + "'", baglan);
                dr.Close();
                komut.ExecuteNonQuery();//sonuçları çalıştır.

                baglan.Close();
            ////////////////////////////////////////////////
        }


        public void KrediAl(IslemBilgisi ib)
        {
            IslemBilgileri.Add(ib);
        }
    }
}
