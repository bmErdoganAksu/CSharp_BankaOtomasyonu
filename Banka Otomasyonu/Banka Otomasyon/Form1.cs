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
    public partial class Form1: Form
    {
        static string con = "Data Source = LAPTOP - 9IQ5NO3T; Initial Catalog = Giris; Integrated Security = True";
        SqlConnection baglan;
        SqlCommand komut;
        SqlDataReader dR;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            
            string sorgu = "SELECT * FROM giris where kullanici_adi=@kullanici_adi AND sifre=@sifre";
            baglan = new SqlConnection("server=.; Initial Catalog=Giris;Integrated Security=SSPI");
            komut = new SqlCommand(sorgu, baglan);
            komut.Parameters.AddWithValue("@kullanici_adi", txtKullaniciAdi.Text);
            komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
            baglan.Open();
            dR = komut.ExecuteReader();
            if (dR.Read())
            {
                MessageBox.Show("Tebrikler!\nGiriş Başarılı.");
                txtKullaniciAdi.Clear();
                txtSifre.Clear();
                aksuBank aksuBank = new aksuBank();
                aksuBank.ShowDialog();
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show("Hatalı Giriş\nKullanıcı adınızı ve şifrenizi kontrol ediniz.");
                txtKullaniciAdi.Clear();
                txtSifre.Clear();
            }
            dR.Close();
            baglan.Close();

        }

        

        private void txtSifre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtKullaniciAdi_TextChanged(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
