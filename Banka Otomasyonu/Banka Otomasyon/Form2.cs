using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banka_Otomasyon
{
    public partial class aksuBank : Form
    {
        static string con = "Data Source=LAPTOP-9IQ5NO3T;Initial Catalog=Musteriler;Integrated Security=True";
        SqlConnection baglan = new SqlConnection(con);
        SqlCommand komut;
        SqlDataAdapter da;
        DataSet ds;
        SqlDataReader dr;


        public long bireyselNo;
        public long bireyselHesapNo = 2000000;
        public int q = 0;
        public decimal ToplamTutar = 0;
        public Banka banka;
        public aksuBank()
        {
            banka = new Banka();
            InitializeComponent();
        }



        void MusteriGetir()
        {
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            da = new SqlDataAdapter("Select *From Musteri", baglan);
            DataTable tablo = new DataTable();
            da.Fill(tablo);
            baglan.Open();
            dataGridView3.DataSource = tablo;
            dataGridView2.DataSource = tablo;
            baglan.Close();
        }
        /*
        void IslemGetir()
        {
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            da = new SqlDataAdapter("SELECT * FROM Islem", baglan);
            DataTable tablo2 = new DataTable();
            da.Fill(tablo2);
            baglan.Open();
            dataGridView1.DataSource = tablo2;
            baglan.Close();
        }

    */
        void ıslemOlustur(string kayanakHesapNo, string heddefHesapNo, string islem,
            string kaynakBakiye, string hedefBakiye, decimal tutar, DateTime IslemTarihi)
        {
            try
            {
                if (baglan.State == ConnectionState.Closed)//eger baglantı kapalıysa
                    baglan.Open();
                /////Yeni işlem oluşturup databaseye kaydetme kodu////////////////////
                string hesapKayit = "INSERT INTO Islem (kaynak, hedef, islem, kaynak_bakiye,hedef_bakiye,tutar,tarih) " +
                "values(@kaynak, @hedef, @islem, @kaynak_bakiye, @hedef_bakiye, @tutar, @tarih)";
                komut = new SqlCommand(hesapKayit, baglan);  //komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                komut.Parameters.AddWithValue("@kaynak", kayanakHesapNo);
                komut.Parameters.AddWithValue("@hedef", heddefHesapNo);
                komut.Parameters.AddWithValue("@islem", islem);
                komut.Parameters.AddWithValue("@kaynak_bakiye", kaynakBakiye);
                komut.Parameters.AddWithValue("@hedef_bakiye", hedefBakiye);
                komut.Parameters.AddWithValue("@tutar", tutar);
                komut.Parameters.AddWithValue("@tarih", IslemTarihi);

                komut.ExecuteNonQuery(); //sonuçları çalıştır.
                baglan.Close();
                MessageBox.Show("İşlem kaydedildi");
                //////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////dataBase kodu sonu////////////////////////////////////////////////////
            }
            catch (Exception hata)
            {
                MessageBox.Show("İşlem kaydetme esnasında hata oluştu:" + hata.Message);

            }

           // IslemGetir();
        }



        void bakiyeGetir(string tc, string HesapNo, Hesap h)
        {
            /////veritabanından gönderici hesaptaki bakiyeyi çekme kodu/////////////
            string BAKİYE;
            string kayit = "SELECT * FROM Hesap where tc_no=@tc_no and hesap_no=@hesap_no";//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz
            komut = new SqlCommand(kayit, baglan);
            komut.Parameters.AddWithValue("@tc_no", tc);
            komut.Parameters.AddWithValue("@hesap_no", HesapNo);
            baglan.Open();
            dr = komut.ExecuteReader();
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

 
        public void BuyukHarfeCevir(Musteri m)
        {
            m.kimlikBilgisi.AdSoyad = m.kimlikBilgisi.Ad.ToUpper() + " " + m.kimlikBilgisi.Soyad.ToUpper();
        }


        public decimal ToplamKasa = 0;



        private void aksuBank_Load(object sender, EventArgs e)
        {

            // TODO: Bu kod satırı 'musterilerDataSet.Hesap' tablosuna veri yükler. Bunu gerektiği şekilde taşıyabilir, veya kaldırabilirsiniz.
           //this.hesapTableAdapter.Fill(this.musterilerDataSet.Hesap);
            MusteriGetir();
            //IslemGetir();
        }

   

        private void btnMusteriKaydet_Click(object sender, EventArgs e)
        {
            BireyselMusteri bireysel = new BireyselMusteri();
            bireysel.kimlikBilgisi.TCKimlikNo = Convert.ToInt64(txtTCKimlikNo.Text);
            bireysel.kimlikBilgisi.Ad = txtAd.Text;
            bireysel.kimlikBilgisi.Soyad = txtSoyad.Text;
            bireysel.kimlikBilgisi.telefon = txtTelefon.Text;
            bireysel.kimlikBilgisi.adres = txtAdres.Text;
            bireysel.MusteriValidasyon();
            if (bireysel.validasyonKontrol == 0)
            {
                ////////db deki en büyük müşteri numarasını alma ve bir artırarak farklı müşteri no lar ürertme kodu//////
                string dbEnBuyukMNo;
                string sorgu3 = "SELECT MAX(musteri_no) FROM Musteri";
                baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
                komut = new SqlCommand(sorgu3, baglan);//sorguyu komuta bağla
                baglan.Open();
                dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    dbEnBuyukMNo = dr[0].ToString();
                    if (dr[0].ToString() == "")
                    {
                        bireyselNo = 1000000;
                    }
                    else
                    {
                        bireyselNo = Convert.ToInt64(dbEnBuyukMNo) + 1;
                    }
                    dr.Close();
                    baglan.Close();
                }
                dr.Close();
                baglan.Close();
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                string sorgu = "SELECT * FROM Musteri where tc_no=@tc_no";
                baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
                komut = new SqlCommand(sorgu, baglan);//sorguyu komuta bağla
                komut.Parameters.AddWithValue("@tc_no", txtTCKimlikNo.Text);//komuta bağladığın sorguyu tc no ya göre yap
                baglan.Open();
                dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    baglan.Close();
                    dr.Close();
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("Bu TC Kimlik No Zaten Bir Müşterimize Aittir");
                }
                else
                {
                    dr.Close();
                    baglan.Close();
                    try
                    {
                        if (baglan.State == ConnectionState.Closed)//eger baglantı kapalıysa
                            baglan.Open();
                        //Musteri databaseye kayıt yapma kodu
                        string kayit = "set identity_insert Musteri on INSERT INTO Musteri (musteri_no, tc_no, ad_soyad, telefon, adres) " +
                        "values(@musteri_no, @tc_no, @ad_soyad, @telefon, @adres) set identity_insert Musteri off";
                        komut = new SqlCommand(kayit, baglan);//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                        komut.Parameters.AddWithValue("@musteri_no", bireyselNo);
                        komut.Parameters.AddWithValue("@tc_no", txtTCKimlikNo.Text);
                        komut.Parameters.AddWithValue("@ad_soyad", txtAd.Text + " " + txtSoyad.Text);
                        komut.Parameters.AddWithValue("@telefon", txtTelefon.Text);
                        komut.Parameters.AddWithValue("@adres", txtAdres.Text);

                        komut.ExecuteNonQuery();//sonuçları çalıştır.
                        baglan.Close();

                        MusteriGetir();//Tabloyu anlık güncelleme için

                        MessageBox.Show("Kayıt başarılı");
                        /////////////////////////////////////////dataBase kodu sonu////////////////////////////////////////////////////
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show("Hata oluştu" + hata.Message);

                    }
                    /////Yeni Hesap oluşturup databaseye kaydetme kodu////////////////////
                    Hesap h = new Hesap();
                    h.Bakiye = 0;
                    h.HesapNo = bireyselHesapNo;

                    baglan.Open();//bagalntıyı aç 
                    string hesapKayit = "INSERT INTO Hesap (tc_no, hesap_no, bakiye, detay) " +
                     "values(@tc_no, @hesap_no, @bakiye, @detay)";
                    komut = new SqlCommand(hesapKayit, baglan);  //komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                    komut.Parameters.AddWithValue("@tc_no", txtTCKimlikNo.Text);
                    komut.Parameters.AddWithValue("@hesap_no", h.HesapNo);
                    komut.Parameters.AddWithValue("@bakiye", h.Bakiye);
                    komut.Parameters.AddWithValue("@detay", "Yeni Müşteri ekleme(Müdür)");

                    komut.ExecuteNonQuery(); //sonuçları çalıştır.
                    baglan.Close();
                    MessageBox.Show("Hesap oluşturuldu");
                    //////////////////////////////////////////////////////////////////////////////////


                    ///tabloyu temzilemek için
                    txtAd.Clear();
                    txtSoyad.Clear();
                    txtTelefon.Clear();
                    txtTCKimlikNo.Text = "0";
                    txtAdres.Text = "";

                }
            }
            else
            {
                MessageBox.Show(bireysel.MusteriValidasyon());
            }
        }



        private void btnHesapEkle_Click(object sender, EventArgs e)
        {
            q = 0;
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtHesapEkleTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();

                Hesap h = new Hesap();
                h.Bakiye = 0;
                h.HesapNo = bireyselHesapNo;


                List<string> hesapNolar = new List<string>();
                string hesapNO;
                SqlCommand komut2;
                //komut2 = new SqlCommand("SELECT * FROM Hesap where hesap_no='" + h.HesapNo + "'", baglan);
                komut2 = new SqlCommand("SELECT * FROM Hesap where tc_no='" + txtHesapEkleTC.Text + "'", baglan);
                baglan.Open();
                dr = komut2.ExecuteReader(); 
                while (dr.Read())
                {
                    hesapNO = dr["hesap_no"].ToString();
                    hesapNolar.Add(hesapNO);
                    q++;
                    ///Hesap no varsa farklı bir hesap no üret kodu
                    while (hesapNolar.Contains(Convert.ToString(bireyselHesapNo)))
                    {
                        bireyselHesapNo++;
                        h.HesapNo = bireyselHesapNo;
                    }
                    ///Hesap no yoksa database e kaydet
                                
                    
                }
                baglan.Close();
                dr.Close();
                try
                {
                    if (baglan.State == ConnectionState.Closed)    //eger baglantı kapalıysa

                        baglan.Open();//bagalntıyı aç 
                    string kayit = "INSERT INTO Hesap (tc_no, hesap_no, bakiye, detay) " +
                     "values(@tc_no, @hesap_no, @bakiye, @detay)";
                    komut = new SqlCommand(kayit, baglan);  //komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                    komut.Parameters.AddWithValue("@tc_no", txtHesapEkleTC.Text);
                    komut.Parameters.AddWithValue("@hesap_no", h.HesapNo);
                    komut.Parameters.AddWithValue("@bakiye", h.Bakiye);
                    komut.Parameters.AddWithValue("@detay", "Hesap ekleme");

                    komut.ExecuteNonQuery(); //sonuçları çalıştır.
                    baglan.Close();
                    MessageBox.Show("Hesap No: " + h.HesapNo + "\n\nHesap Başarıyla Eklendi");
                    /////////////////////////////////////////dataBase kodu sonu//////////////////////////////
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata oluştu" + hata.Message);

                }
            }
             if (q == 0)
             {
                    baglan.Close();
                    dr.Close();
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("Müşteri Bulunamadı");
             }
                txtHesapEkleTC.Text = "0";

                baglan.Close();
                dr.Close();
        }



        private void btnParaYatirma_Click(object sender, EventArgs e)
        {
            IslemBilgisi isb = new IslemBilgisi();
            isb.Detay = "Para Yatırma";
            q = 0;
            Hesap h= new Hesap();
                q++;
                if (Convert.ToDecimal(txtParaYatirmaTutar.Text) > 0)
                {
                    isb.IslemTarihi = DateTime.Now.Date;
                    isb.Tutar = Convert.ToDecimal(txtParaYatirmaTutar.Text);

                    h.ParaYatir(txtParaYatirmaTC.Text, cmbParaYatırmaHesapNo.Text, txtParaYatirmaTutar.Text, isb);

                    MessageBox.Show("Para Yatırma Başarıyla Gerçekleşmiştir" + "\n\nGüncel Bakiye: " + h.Bakiye);
                //işlemleri db ye kaydetme kodu///////////////////////////////////////////////
                    ıslemOlustur(cmbParaYatırmaHesapNo.Text, "Banka", isb.Detay.ToString(),
                        "null", h.Bakiye.ToString(), isb.Tutar, isb.IslemTarihi);
                ////////////////////////////////////////////////////////////////////////////////////////////


                txtParaYatirmaTC.Text = "0";
                    cmbParaYatırmaHesapNo.Text = "0";
                    txtParaYatirmaTutar.Text = "0";
                    ToplamKasa += isb.Tutar;
                    lblKasa.Text = ToplamKasa.ToString();
                }
                else
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("Hatta!!!!\nGeçerli Tutar Giriniz");
                }
            

    
            
        }


        private void btnParaCekmeCek_Click(object sender, EventArgs e)
        {
            ToplamTutar = 0;
            IslemBilgisi ib = new IslemBilgisi();
            ib.Detay = "Para Çekme";
            q = 0;
            Hesap h = new Hesap();
            
            //////veritabanından bakiye çekme fonksiyonu///////////////////////
            bakiyeGetir(txtParaCekmeTC.Text,cmbParaCekmeHesapNo.Text, h);
            
            ///////////////////////////////////////////////////////////////////////////////////
            if (Convert.ToDecimal(txtParaCekmeTutar.Text) > 0 && Convert.ToDecimal(txtParaCekmeTutar.Text) <= h.Bakiye)
                {
                    q++;
                    ib.IslemTarihi = DateTime.Now.Date;
                    ib.Tutar = 0;
                    h.ParaCek(txtParaCekmeTC.Text, cmbParaCekmeHesapNo.Text, txtParaCekmeTutar.Text, ib);
                   
                        if (ib.Detay == "Para Çekme" && DateTime.Compare(ib.IslemTarihi, ib.IslemTarihi) == 0)
                        {
                            h.Iptal(ib);
                            if (ib.Tutar == 0)
                            {
                                ib.GunlukParaCekme = 0;
                            }
                           // ToplamTutar += ib.GunlukParaCekme;
                            //ToplamTutar += Convert.ToDecimal(txtParaCekmeTutar.Text);
                           // if (ToplamTutar <= 750)
                           // {
                                ib.IslemTarihi = DateTime.Now.Date;
                                ib.Tutar = Convert.ToDecimal(txtParaCekmeTutar.Text);
                                ib.GunlukParaCekme += Convert.ToDecimal(txtParaCekmeTutar.Text);
                               
                    //işlem blgisini db ye yazdırma kodu///////////////////////////////////////////////////
                                ıslemOlustur(cmbParaCekmeHesapNo.Text, "Banka", ib.Detay.ToString(),
                                    "null", h.Bakiye.ToString(), ib.Tutar, ib.IslemTarihi);
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    MessageBox.Show(ib.Tutar + " TL Para Çekme İşlemi Başarıyla Gerçekleşmiştir\nGüncel Bakiye:" + h.Bakiye);
                                
                                ToplamKasa -= ib.Tutar;
                                lblKasa.Text = ToplamKasa.ToString();
                                //cmbParaCekmeHesapNo.Items.Clear();
                                txtParaCekmeTC.Text = "0";
                                cmbParaCekmeHesapNo.Text = "0";
                                txtParaCekmeTutar.Text = "0";

                        }
                    
                }
                else
                {
                    q++;
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("Bakiyeniz yetersiz. Lütfen geçerli Tutar Giriniz");
                }
                if (q == 0)
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("TC Kimlik No veya Hesap Numaranız Yanlış");
                }

                //////////////////////////////////////////////////////////////////////////////

            
        }


      


        private void btnParaTransferGonder_Click(object sender, EventArgs e)
        {

            IslemBilgisi ibG = new IslemBilgisi();
            IslemBilgisi ibA= new IslemBilgisi();
            string Detay = "Para Transferi";
            ibG.Tutar = Convert.ToDecimal(txtParaTransferTutar.Text);
            
            /////veritabanından gondericinin hesabındaki bakiyeyi çekme kodu/////////////
            Hesap gonderici = new Hesap();
            bakiyeGetir(txtParaTransferGTC.Text, cmbParaTransferGHesapNo.Text, gonderici);
            ///////////////////////////////////////////////////////////////////////////////
           
            /////veritabanından alıcının hesabındaki bakiyeyi çekme kodu/////////////
            Hesap alici = new Hesap();
            bakiyeGetir(txtParaTransferiATC.Text, cmbParaTransferAHesapNo.Text, alici);
            ///////////////////////////////////////////////////////////////////////////////
           
            int q = 0;
            
            if (gonderici.Bakiye >= (ibG.Tutar))
            {
                q++;

                ibG.Tutar = Convert.ToDecimal(txtParaTransferTutar.Text);
                ibG.kaynakHesapNo = Convert.ToInt64(cmbParaTransferGHesapNo.Text);
                ibG.IslemTarihi = DateTime.Now.Date;
               
                ibA.Tutar = Convert.ToDecimal(txtParaTransferTutar.Text);
                ibA.hedefHesapNo = Convert.ToInt64(cmbParaTransferAHesapNo.Text);
                ibA.IslemTarihi = DateTime.Now.Date;

                /////databaseden hesap numarasına göre para transferinden sonra hesaptaki bakiyeyi guncelleme kodu/////////////
                gonderici.GondericiBakiyeGuncelle(txtParaTransferGTC.Text, cmbParaTransferGHesapNo.Text, cmbParaTransferAHesapNo.Text, txtParaTransferTutar.Text, ibG);
                ////////////////////////////////////////////////////////////////////
                /////databaseden hesap numarasına göre para transferinden sonra hesaptaki bakiyeyi guncelleme kodu/////////////
                alici.AliciBakiyeGuncelle(txtParaTransferiATC.Text, cmbParaTransferGHesapNo.Text, cmbParaTransferAHesapNo.Text, txtParaTransferTutar.Text, ibA);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                
                
                //işlmeler db ye işlmei kaydetme kodu///////////////////////////////////////////////
                ıslemOlustur(cmbParaTransferGHesapNo.Text, cmbParaTransferAHesapNo.Text, Detay,
                                    gonderici.Bakiye.ToString(), alici.Bakiye.ToString(), ibG.Tutar, ibG.IslemTarihi);
                //işlmeler db ye işlmei kaydetme kodu///////////////////////////////////////////////

                MessageBox.Show(ibG.Tutar + " TL Para Transferi Başarıyla Tamamlanmıştır\nGüncel Bakiyeniz: " + gonderici.Bakiye);
                
               
                //cmbParaTransferAHesapNo.Items.Clear();
                //cmbParaTransferGHesapNo.Items.Clear();
                // ToplamKasa -= ib2.TransferUcreti;
                //lblKasa.Text = ToplamKasa.ToString();
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("Yetersiz Bakiye");
            }
          
            if (q == 0)
                    {
                        System.Media.SystemSounds.Beep.Play();
                        MessageBox.Show("Girilen Bilgiler Hatalı Lütfen Tekrar Deneyiniz");
                    }

            cmbParaTransferGHesapNo.Text = "0";
            cmbParaTransferAHesapNo.Text = "0";
            txtParaTransferGTC.Text = "0";
            txtParaTransferiATC.Text = "0";
            txtParaTransferTutar.Text= "0";
        }





        private void btnHesapKapatmaKapat_Click_1(object sender, EventArgs e)
        {
            int kontrol = 0;
            IslemBilgisi ib = new IslemBilgisi();
            Hesap h = new Hesap();
            bakiyeGetir(txtHesapKapatmaTC.Text, cmbHesapKapatmaHesapNo.Text, h);
            

        /////////////////musteri var mı kontrolü için////////////////////////////////////////////
            string sorgu = "SELECT * FROM Hesap where tc_no=@tc_no and hesap_no=@hesap_no";
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            komut = new SqlCommand(sorgu, baglan);//sorguyu komuta bağla
            komut.Parameters.AddWithValue("@tc_no", txtHesapKapatmaTC.Text);//komuta bağladığın sorguyu tc no ya göre yap
            komut.Parameters.AddWithValue("@hesap_no", cmbHesapKapatmaHesapNo.Text);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                //dr.Close();
        /////////////////musteri var mı kontrolü için////////////////////////////////////////////
                try
                {
                    if (h.Bakiye == 0)
                    {
                        dr.Close();
                        baglan.Close();

                        string kayit = "DELETE FROM Hesap WHERE tc_no=@tc_no and hesap_no=@hesap_no";
                        komut = new SqlCommand(kayit, baglan);//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz
                        komut.Parameters.AddWithValue("@tc_no", txtHesapKapatmaTC.Text);
                        komut.Parameters.AddWithValue("@hesap_no", cmbHesapKapatmaHesapNo.Text);
                        baglan.Open();
                        komut.ExecuteNonQuery();//sonuçları çalıştır.
                        baglan.Close();
                        MessageBox.Show("Hesap Başarıyla Silindi");

                        /////////////////////////////////////////dataBase kodu sonu/////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                        MessageBox.Show("Hesap Kapatılamaz!\nBakiyeniz 0 TL Olmalıdır\nMevcut Bakiye: " + h.Bakiye);
                    }
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata oluştu " + hata.Message);

                }

            }
            else
            {
                MessageBox.Show("Müşteri yok");
            }

            txtHesapEkleTC.Text = "0";
            baglan.Close();
           
        }



        private void timer1_Tick(object sender, EventArgs e)
        {//Labellere Gün ay ve yıl olarak tarih, saat dakika ve saniye olarak saat alma kodu
            lblTarih.Text = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
            lblSaat.Text = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;


        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtTCKimlikNo.Text = "0";
            txtAd.Text = "";
            txtSoyad.Text = "";
            txtTelefon.Text = "";
            txtAdres.Text = "";
        }

        private void btnParaYatirmaTemizle_Click(object sender, EventArgs e)
        {
            txtParaYatirmaTC.Text = "0";
            txtParaYatirmaTutar.Text = "0";
            cmbParaYatırmaHesapNo.Items.Clear();
            cmbParaYatırmaHesapNo.Text = "0";
        }

        private void btnParaCekTemizle_Click(object sender, EventArgs e)
        {
            txtParaCekmeTC.Text = "0";
            txtParaCekmeTutar.Text = "0";
            //cmbParaCekmeHesapNo.Items.Clear();
            cmbParaCekmeHesapNo.Text = "0";
        }

        private void btnParaTransferTemizle_Click(object sender, EventArgs e)
        {
            txtParaTransferGTC.Text = "0";
            txtParaTransferiATC.Text = "0";
            txtParaTransferTutar.Text = "0";
           // cmbParaTransferAHesapNo.Items.Clear();
           //cmbParaTransferGHesapNo.Items.Clear();
            cmbParaTransferGHesapNo.Text = "0";
            cmbParaTransferAHesapNo.Text = "0";

        }

        private void btnHesapKapatmaTemizle_Click(object sender, EventArgs e)
        {
            txtHesapKapatmaTC.Text = "0";
            cmbHesapKapatmaHesapNo.Items.Clear();
            cmbHesapKapatmaHesapNo.Text = "0";
        }

  
     


        private void btnMtKaydet_Click(object sender, EventArgs e)
        {
            BireyselMusteri bireysel = new BireyselMusteri();
            bireysel.kimlikBilgisi.TCKimlikNo = Convert.ToInt64(txtMtTC.Text);
            bireysel.kimlikBilgisi.Ad = txtMtAd.Text;
            bireysel.kimlikBilgisi.Soyad = txtMtSoyad.Text;
            bireysel.kimlikBilgisi.telefon = txtMtTelefon.Text;
            bireysel.kimlikBilgisi.adres = txtMtAdres.Text;
            bireysel.MusteriValidasyon();
            if (bireysel.validasyonKontrol==0) {
                ////////db deki en büyük müşteri numarasını alma ve bir artırarak farklı müşteri no lar ürertme kodu//////
                string dbEnBuyukMNo;
                string sorgu3 = "SELECT MAX(musteri_no) FROM Musteri";
                baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
                komut = new SqlCommand(sorgu3, baglan);//sorguyu komuta bağla
                baglan.Open();
                dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    dbEnBuyukMNo = dr[0].ToString();
                    if (dr[0].ToString() == "")
                    {
                        bireyselNo = 1000000;
                    }
                    else {
                        bireyselNo = Convert.ToInt64(dbEnBuyukMNo) + 1;
                    }
                    dr.Close();
                    baglan.Close();
                }
                dr.Close();
                baglan.Close();
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                string sorgu = "SELECT * FROM Musteri where tc_no=@tc_no";
                baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
                komut = new SqlCommand(sorgu, baglan);//sorguyu komuta bağla
                komut.Parameters.AddWithValue("@tc_no", txtMtTC.Text);//komuta bağladığın sorguyu tc no ya göre yap
                baglan.Open();
                dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    baglan.Close();
                    dr.Close();
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("Bu TC Kimlik No Zaten Bir Müşterimize Aittir");
                }
                else
                {
                    dr.Close();
                    baglan.Close();
                    try
                    {
                        if (baglan.State == ConnectionState.Closed)//eger baglantı kapalıysa
                            baglan.Open();
                        //Musteri databaseye kayıt yapma kodu
                        string kayit = "set identity_insert Musteri on INSERT INTO Musteri (musteri_no, tc_no, ad_soyad, telefon, adres) " +
                        "values(@musteri_no, @tc_no, @ad_soyad, @telefon, @adres) set identity_insert Musteri off";
                        komut = new SqlCommand(kayit, baglan);//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                        komut.Parameters.AddWithValue("@musteri_no", bireyselNo);
                        komut.Parameters.AddWithValue("@tc_no", txtMtTC.Text);
                        komut.Parameters.AddWithValue("@ad_soyad", txtMtAd.Text + " " + txtMtSoyad.Text);
                        komut.Parameters.AddWithValue("@telefon", txtMtTelefon.Text);
                        komut.Parameters.AddWithValue("@adres", txtMtAdres.Text);

                        komut.ExecuteNonQuery();//sonuçları çalıştır.
                        baglan.Close();

                        MusteriGetir();//Tabloyu anlık güncelleme için

                        MessageBox.Show("Kayıt başarılı");
                        /////////////////////////////////////////dataBase kodu sonu////////////////////////////////////////////////////
                    }
                    catch (Exception hata)
                    {
                        MessageBox.Show("Hata oluştu" + hata.Message);
                        baglan.Close();

                    }
                    /////Yeni Hesap oluşturup databaseye kaydetme kodu////////////////////
                    Hesap h = new Hesap();
                    h.Bakiye = 0;
                    h.HesapNo = bireyselHesapNo;

                    baglan.Open();//bagalntıyı aç 
                    string hesapKayit = "INSERT INTO Hesap (tc_no, hesap_no, bakiye, detay) " +
                     "values(@tc_no, @hesap_no, @bakiye, @detay)";
                    komut = new SqlCommand(hesapKayit, baglan);  //komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                    komut.Parameters.AddWithValue("@tc_no", txtMtTC.Text);
                    komut.Parameters.AddWithValue("@hesap_no", h.HesapNo);
                    komut.Parameters.AddWithValue("@bakiye", h.Bakiye);
                    komut.Parameters.AddWithValue("@detay", "Yeni Müşteri Ekleme(Müşteri Temsilcisi)");

                    komut.ExecuteNonQuery(); //sonuçları çalıştır.
                    baglan.Close();
                    MessageBox.Show("Hesap oluşturuldu");
                    //////////////////////////////////////////////////////////////////////////////////


                    ///tabloyu temzilemek için
                    txtMtAd.Clear();
                    txtMtSoyad.Clear();
                    txtMtTelefon.Clear();
                    txtMtTC.Text = "0";
                    txtMtAdres.Text = "";

                }
            }
            else
            {
                MessageBox.Show(bireysel.MusteriValidasyon());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtMtTC.Text = "0";
            txtMtAd.Text = "";
            txtMtSoyad.Text = "";
            txtMtTelefon.Text = "";
            txtAdres.Text = "";
        }

        private void btnMusteriSil_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM Musteri where tc_no=@tc_no";
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            komut = new SqlCommand(sorgu, baglan);//sorguyu komuta bağla
            komut.Parameters.AddWithValue("@tc_no", txtSilTC.Text);//komuta bağladığın sorguyu tc no ya göre yap
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                dr.Close();
                baglan.Close();
                try
                {
                    string kayit = "DELETE FROM Musteri WHERE tc_no=@tc_no";
                    komut = new SqlCommand(kayit, baglan);//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz

                    komut.Parameters.AddWithValue("@tc_no", txtSilTC.Text);
                    baglan.Open();
                    komut.ExecuteNonQuery();//sonuçları çalıştır.
                    baglan.Close();

                    MusteriGetir();//Tabloyu anlık güncelleme için
                    MessageBox.Show("Müşteri Başarıyla Silindi");
                    /////////////////////////////////////////dataBase kodu sonu/////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata oluştu " + hata.Message);

                }
                try
                {
                    string kayit = "DELETE FROM Hesap WHERE tc_no=@tc_no";
                    komut = new SqlCommand(kayit, baglan);//komut ile kayıt stringine baglandan gelen değerleri bağlıyoruz
                    komut.Parameters.AddWithValue("@tc_no", txtSilTC.Text);
                    //komut.Parameters.AddWithValue("@hesap_no", cmbHesapKapatmaHesapNo.Text);
                    baglan.Open();
                    komut.ExecuteNonQuery();//sonuçları çalıştır.
                    baglan.Close();
                    MessageBox.Show("Hesap Başarıyla Silindi");

                    /////////////////////////////////////////dataBase kodu sonu/////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                catch (Exception hata)
                {
                    MessageBox.Show("Hata oluştu " + hata.Message);

                }

            }
            else
            {
                MessageBox.Show("Müşteri yok");
            }

        }



     
        private void btnBilgiGuncelle_Click(object sender, EventArgs e)
        {
            string sorgu = "SELECT * FROM Musteri where tc_no=@tc_no";
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            komut = new SqlCommand(sorgu, baglan);//sorguyu komuta bağla
            komut.Parameters.AddWithValue("@tc_no", txtGuncelleTC.Text);//komuta bağladığın sorguyu tc no ya göre yap
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                dr.Close();
                baglan.Close();
                pnlGuncelle.Visible = true;


            }
            else
            {
                MessageBox.Show("Müşteri Bulunamadi");
            }

        }

        private void txtAdres_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtGuncelleTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();

                komut = new SqlCommand(
                    "UPDATE Musteri SET ad_soyad='" + txtGuncelleAd.Text + " " + txtGuncelSoyad.Text + "'" +
                    "where tc_no = '" + txtGuncelleTC.Text + "'" +
                    "UPDATE Musteri SET telefon='" + txtGuncelleTelefon.Text + "'" +
                    "where tc_no = '" + txtGuncelleTC.Text + "'" +
                    "UPDATE Musteri SET adres='" + txtGuncelleAdres.Text + "'" +
                    "where tc_no = '" + txtGuncelleTC.Text + "'", baglan);
               
                baglan.Open();
                komut.ExecuteNonQuery();//sonuçları çalıştır.
                baglan.Close();

                MusteriGetir();//Tabloyu anlık güncelleme için
                MessageBox.Show("Guncelleme başarılı");

            }
            pnlGuncelle.Visible = false;

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (Musteri m in banka.Musteriler)
            {
                dataGridView3.Rows.Add(m.MusteriNo, m.kimlikBilgisi.TCKimlikNo, m.kimlikBilgisi.AdSoyad,
                    m.kimlikBilgisi.telefon, m.kimlikBilgisi.adres, m.MusteriTipi);
            }
        }

        private void btnHSPGOR_Click(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtHSPGORtc.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");

                da = new SqlDataAdapter("Select *From Hesap where tc_no='" + txtHSPGORtc.Text + "'", baglan);
                DataTable tablo3 = new DataTable();
                da.Fill(tablo3);
                baglan.Open();
                dataGridView4.DataSource = tablo3;
                baglan.Close();
            }
            else
            {
                baglan.Close();
                MessageBox.Show("Müşteri Bulunamadı");
                dataGridView4.DataSource = "";
            }


        }


        private void txtParaYatirmaTC_TextChanged(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtParaYatirmaTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                DataTable dataTable = new DataTable();
                baglan.ConnectionString = "Data Source=.;Initial Catalog=Musteriler;Integrated Security=SSPI";

                SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Hesap where tc_no='" + txtParaYatirmaTC.Text + "'", baglan);
                da.Fill(dataTable);

                cmbParaYatırmaHesapNo.DisplayMember = "hesap_no";
                cmbParaYatırmaHesapNo.DataSource = dataTable;

            }
            baglan.Close();
        }

        private void cmbParaYatırmaHesapNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void txtHesapKapatmaTC_TextChanged(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtHesapKapatmaTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                DataTable dt = new DataTable();
                baglan.ConnectionString = "Data Source=.;Initial Catalog=Musteriler;Integrated Security=SSPI";

                SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Hesap where tc_no='" + txtHesapKapatmaTC.Text + "'", baglan);
                da.Fill(dt);

                cmbHesapKapatmaHesapNo.DisplayMember = "hesap_no";
                cmbHesapKapatmaHesapNo.DataSource = dt;

            }
            baglan.Close();
        }

        private void txtParaCekmeTC_TextChanged(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtParaCekmeTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                DataTable dataTable = new DataTable();
                baglan.ConnectionString = "Data Source=.;Initial Catalog=Musteriler;Integrated Security=SSPI";

                SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Hesap where tc_no='" + txtParaCekmeTC.Text + "'", baglan);
                da.Fill(dataTable);

                cmbParaCekmeHesapNo.DisplayMember = "hesap_no";
                cmbParaCekmeHesapNo.DataSource = dataTable;

            }
            baglan.Close();
        }

        private void txtParaTransferGTC_TextChanged(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtParaTransferGTC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                DataTable dataTable = new DataTable();
                baglan.ConnectionString = "Data Source=.;Initial Catalog=Musteriler;Integrated Security=SSPI";

                SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Hesap where tc_no='" + txtParaTransferGTC.Text + "'", baglan);
                da.Fill(dataTable);

                cmbParaTransferGHesapNo.DisplayMember = "hesap_no";
                cmbParaTransferGHesapNo.DataSource = dataTable;

            }
            baglan.Close();
        }

        private void txtParaTransferiATC_TextChanged(object sender, EventArgs e)
        {
            komut = new SqlCommand("SELECT * FROM Musteri where tc_no='" + txtParaTransferiATC.Text + "'", baglan);
            baglan.Open();
            dr = komut.ExecuteReader();
            if (dr.Read())
            {
                baglan.Close();
                dr.Close();
                DataTable dataTable = new DataTable();
                baglan.ConnectionString = "Data Source=.;Initial Catalog=Musteriler;Integrated Security=SSPI";

                SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Hesap where tc_no='" + txtParaTransferiATC.Text + "'", baglan);
                da.Fill(dataTable);

                cmbParaTransferAHesapNo.DisplayMember = "hesap_no";
                cmbParaTransferAHesapNo.DataSource = dataTable;

            }
            baglan.Close();
        }

        private void btnIslem_Click(object sender, EventArgs e)
        {
            baglan = new SqlConnection("server=.; Initial Catalog=Musteriler;Integrated Security=SSPI");
            da = new SqlDataAdapter("SELECT TOP "+ txtIslemAdedi.Text+" * FROM Islem", baglan);
            DataTable tablo2 = new DataTable();
            da.Fill(tablo2);
            baglan.Open();
            dataGridView1.DataSource = tablo2;
            baglan.Close();
        }

        private void txtTCKimlikNo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
