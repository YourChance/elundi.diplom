using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SQLite;
using System.Text;
using System.IO;
using System.Windows.Forms;
namespace ETEnTranslator
{
    public partial class Add_to_db : Form
    {
        public Add_to_db()
        {
            InitializeComponent();
        }
        private void Add_to_db_Load(object sender, EventArgs e)
        {
            add_button.Enabled = false;
        }
        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void Enable_btn()
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "")
                add_button.Enabled = true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           //PostMessage(handle,WM_INPUTLANGCHANGEREQUEST,0,LoadKeyboardLayout( StrCopy(Layout,'00000419'),KLF_ACTIVATE));




            Enable_btn();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Enable_btn();
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Enable_btn();
        }
        private void add_button_Click(object sender, EventArgs e)
        {
          try
           {
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=|DataDirectory|dict.sqlitedb");
            connection.Open();
            SQLiteCommand cmd1 = new SQLiteCommand("INSERT INTO dict (rus,el,eng) VALUES(@rus,@el,@eng)", connection);
            cmd1.Parameters.AddWithValue("@rus", textBox1.Text);
            cmd1.Parameters.AddWithValue("@el", textBox2.Text);
            cmd1.Parameters.AddWithValue("@eng", textBox3.Text);
            cmd1.ExecuteNonQuery();
            connection.Close();
            add_button.Text = "";
               add_button.Image = Image.FromFile("img/ok.gif");
                      
           }
           catch
           {
               add_button.Text = "";
               add_button.Image = Image.FromFile("img/error.gif");
           }
          timer1.Start();
          
       }

       int i = 0;
       private void timer1_Tick(object sender, EventArgs e)
       {
           i++;
           if (i == 2)
           {
               Change_img();
               timer1.Stop();
               i = 0;
           }
       }
       private void Change_img()
       {

           add_button.Text = "OK";
           add_button.Image = null;
           add_button.Enabled = false;
           textBox1.Text = textBox2.Text = textBox3.Text = "";
       }

       private void textBox1_Enter(object sender, EventArgs e)
       {
          
  //SetKeyboardLayout(eng);

  //SetKeyboardLayout(ru);
 

       }

     
      
    }
}
