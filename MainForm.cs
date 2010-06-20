/**
 * БУДУЩИМ ПОКОЛЕНИЯМ:
 * 
 * ЕСТЬ ТАКОЕ ПРАВИЛО: ЕСЛИ НЕ ЗНАЕШЬ ЗАЧЕМ ЭТО ЗДЕСЬ - НЕ ТРОЖЬ!
 * ЛУЧШЕ ОСТАВИТЬ ЧУЖОЙ ГОВНОКОД, ЧЕМ ВЫТЕРЕТЬ ЧТО-ТО НУЖНОЕ, И ПОТОМ КРАСНЕТЬ НА ДИПЛОМЕ.
 * 
 * © 05ПОВТ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ETEnTranslator;

namespace ETRTranslator
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		RTECore RuEl;
		ETRCore ElRu;
        ETEnCore ElEn;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			RuEl = new RTECore();
			RuEl.Tick += TickEventHandler;
			RuEl.End += RuElEndEventHandler;
			RuEl.Mess += MessEventHandler;
			
			ElRu = new ETRCore();
			ElRu.Tick += TickEventHandler;
			ElRu.End += ElRuEndEventHandler;

            ElEn = new ETEnCore();
            ElEn.Tick += TickEventHandler;
            ElEn.End += ElRuEndEventHandler; //тот же что и для русского, токо там у нас англ. будет
			
			this.toolStripComboBox1.SelectedIndex  = 0;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public void TickEventHandler(object sender, CoreTickEventArgs ea)
		{
			this.tick = ea.Tick;
			this.Invoke(new MethodInvoker(SetTick));
		}
		
		public void RuElEndEventHandler(object sender, CoreEndEventArgs ea)
		{
			elText = ea.Text;
			this.Invoke(new MethodInvoker(SetElText));
		}
		
		public void MessEventHandler(object sender, CoreEndEventArgs ea)
		{
			mess = ea.Text;
			this.Invoke(new MethodInvoker(Mess));
		}
		
		public void ElRuEndEventHandler(object sender, CoreEndEventArgs ea)
		{
			ruText = ea.Text;
			this.Invoke(new MethodInvoker(SetRuText));
		}
		
		double tick;
		string elText;
		string ruText;
		string mess;
		
		void SetTick()
		{
			this.progressBar1.Value = (int)(tick*1000)%1000;
		}
		
		void Mess()
		{
			//System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt");
			//sw.WriteLine(mess);
			//sw.Close();
		}
		
		void SetElText()
		{
			this.richTextBox3.Text = elText;
			this.progressBar1.Visible = false;
			
			if(this.toolStripComboBox1.SelectedItem.ToString() == "Русский -> Эльюнди -> Русский")
			{
				this.progressBar1.Value = 0;
				this.progressBar1.Visible = true;
				string text2 = this.richTextBox3.Text;
				System.Threading.Thread netThread2 = new System.Threading.Thread(delegate()
				{
					ElRu.Translate(text2);
				});
				netThread2.Start();
			}
		}
		
		void SetRuText()
		{
			this.richTextBox2.Text = ruText.Replace("в  о","в");
			this.progressBar1.Visible = false;			
		}
		
		void Button1Click(object sender, EventArgs e)
		{
		/*	Predlozhenie pr = new Predlozhenie();
			pr.AddSlovo(new Slovo(textBox1.Text));
			ETRNoun noun = new ETRNoun();
			textBox2.Text = (noun.Analyze(pr,0)).rSlovo;			*/
		}
		
		void Button2Click(object sender, EventArgs e)
		{
		/*	listBox1.Items.Clear();
			
			Predlozhenie pr = new Predlozhenie();
			pr.AddSlovo(new Slovo(textBox1.Text));
			pr[0].rSlovo = textBox2.Text;
			ETRNoun noun = new ETRNoun();
			Slovo s = pr[0];
			s.rod = Rod.Obshij;
			noun.PostAnalyze(pr,0,ref s);
			s.ruSlovo.ruChislo = RuChislo.Mnozhestvennoe;	
			pr.SetSlovo(s,0);
			textBox3.Text = (noun.Translate(pr,0)).rSlovo;
			
			pr = new Predlozhenie();
			pr.AddSlovo(new Slovo(textBox1.Text));
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			s.rod = Rod.Obshij;
			
			s.ruSlovo.ruChislo = RuChislo.Edinstvennoe;
			noun.PostAnalyze(pr,0,ref s);
			s.ruSlovo.ruPadezh = RuPadezh.Imenitelniy;
			listBox1.Items.Add(noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Roditelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("нет кого/чего? "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Vinitelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("виню кого/что? "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Datelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("иду к кому/чему? к "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Tvoritelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("клянусь кем/чем?  "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Predlozhniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("говрю о ком/чем? о "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruChislo = RuChislo.Mnozhestvennoe;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			s.ruSlovo.ruPadezh = RuPadezh.Imenitelniy;
			listBox1.Items.Add(noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Roditelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("нет кого/чего? "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Vinitelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("виню кого/что? "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Datelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("иду к кому/чему? к "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Tvoritelniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("клянусь кем/чем?  "+noun.Translate(pr,0).rSlovo);
			
			s.ruSlovo.ruPadezh = RuPadezh.Predlozhniy;
			pr[0].rSlovo = textBox2.Text;
			s = pr[0];
			listBox1.Items.Add("говрю о ком/чем? о "+noun.Translate(pr,0).rSlovo);
			*/
		}
		
		void Button3Click(object sender, EventArgs e)
		{
		/*	Grafemat grafemat = new Grafemat();
			ArrayList slova = grafemat.AnalyzeText(richTextBox1.Text);
			foreach(Predlozhenie p in slova)
			{
				textBox4.AppendText(p.ToRString()+"\n\n");
				//	listBox2.Items.Add(p.ToRString());
			}
			
			MorfologRus morfolog = new MorfologRus();
			for(int i=0;i<slova.Count;i++)
			{
				slova[i] = morfolog.MorfAnalyze((Predlozhenie)slova[i]);
			}
			
			int i_p=0;
			foreach(Predlozhenie p in slova)
			{
				TreeNode rootNode = treeView1.Nodes.Add("Предложение "+i_p.ToString());
				for(int i=0;i<p.Count;i++)
				{
					TreeNode slovoNode = rootNode.Nodes.Add(((Slovo)p[i]).rSlovo);
					slovoNode.Nodes.Add(((Slovo)p[i]).eSlovo);
					slovoNode.Nodes.Add(((Slovo)p[i]).chastRechi.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).ruSlovo.ruChislo.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).naklonenie.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).odushevlennost.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).ruSlovo.ruPadezh.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).ruSlovo.ruRod.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).stepenSravneniya.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).vid.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).vremya.ToString());
					slovoNode.Nodes.Add(((Slovo)p[i]).zalog.ToString());
				}
				i_p++;
			}*/
		}
		
		void Button5Click(object sender, EventArgs e)
		{
		/*	ETRCore core = new ETRCore();
			richTextBox4.Text = core.Translate(richTextBox3.Text);*/
		}
		
		void Button4Click(object sender, EventArgs e)
		{
		/*	RTECore core = new RTECore();
			richTextBox3.Text = core.Translate(richTextBox2.Text);*/
		}
		
		void TabPage3Click(object sender, EventArgs e)
		{
			
		}
		
		void ПанельМенюToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.toolStrip1.Visible = !this.toolStrip1.Visible;
		}		
		
		
		void RichTextBox1TextChanged(object sender, EventArgs e)
		{
			
		}
		
		void RichTextBox3TextChanged(object sender, EventArgs e)
		{
			//MessageBox.Show(richTextBox3.Font.ToString());
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			
		}
		
		void ToolStripButton1Click(object sender, EventArgs e)
		{
			this.richTextBox1.Text = "";
			this.richTextBox2.Text = "";
			this.richTextBox3.Text = "";
		}
		
		void ToolStripButton5Click(object sender, EventArgs e)
		{
			if(this.toolStripComboBox1.SelectedIndex != -1)
			{
				switch(this.toolStripComboBox1.SelectedItem.ToString())
				{
					case "Русский -> Эльюнди":
						this.progressBar1.Value = 0;
						this.progressBar1.Visible = true;
						string text = this.richTextBox1.Text;
						System.Threading.Thread netThread = new System.Threading.Thread(delegate()
						{
							RuEl.Translate(text);
						});
						netThread.Start();
						break;
					case "Эльюнди -> Русский":
						this.progressBar1.Value = 0;
						this.progressBar1.Visible = true;
						string text2 = this.richTextBox3.Text;
						System.Threading.Thread netThread2 = new System.Threading.Thread(delegate()
						{
							ElRu.Translate(text2);
						});
						netThread2.Start();
						break;
					case "Русский -> Эльюнди -> Русский":
						this.progressBar1.Value = 0;
						this.progressBar1.Visible = true;
						string text3 = this.richTextBox1.Text;
						System.Threading.Thread netThread3 = new System.Threading.Thread(delegate()
						{
							RuEl.Translate(text3);
						});
						netThread3.Start();
						break;
                    case "Эльюнди -> Английский":
                        this.progressBar1.Value = 0;
                        this.progressBar1.Visible = true;
                        string text4 = this.richTextBox3.Text;
                        System.Threading.Thread netThread4 = new System.Threading.Thread(delegate()
                        {
                            ElEn.Translate(text4);
                        });
                        netThread4.Start();
                        break;
				}
			}
		}
		
		void RuElToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.progressBar1.Value = 0;
			this.progressBar1.Visible = true;
			string text = this.richTextBox1.Text;
			System.Threading.Thread netThread = new System.Threading.Thread(delegate()
			{
				RuEl.Translate(text);
			});
			netThread.Start();
		}
		
		void ElRuToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.progressBar1.Value = 0;
			this.progressBar1.Visible = true;
			string text2 = this.richTextBox3.Text;
			System.Threading.Thread netThread2 = new System.Threading.Thread(delegate()
			{
				ElRu.Translate(text2);
			});
			netThread2.Start();
		}
		
		void RuElRuToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.progressBar1.Value = 0;
			this.progressBar1.Visible = true;
			string text = this.richTextBox1.Text;
			this.toolStripComboBox1.SelectedIndex = 2;
			System.Threading.Thread netThread = new System.Threading.Thread(delegate()
			{
				RuEl.Translate(text);
			});
			netThread.Start();
		}
		
		void НовыйToolStripMenuItemClick(object sender, EventArgs e)
		{
			richTextBox1.Text = "";
			richTextBox2.Text = "";
			richTextBox3.Text = "";
		}
		
		void ToolStripButton4Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		void ВыходToolStripMenuItemClick(object sender, EventArgs e)
		{
			Application.Exit();
		}	
		
		
		void ToolStripButton2Click(object sender, EventArgs e)
		{
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if(MessageBox.Show("Файл содержит текст на русском языке? ('нет' - файл содержит текст на Эльюнди)","Выбор языка открываемого текста", MessageBoxButtons.YesNo) == DialogResult.Yes)
					richTextBox1.LoadFile(openFileDialog1.FileName);
				else richTextBox3.LoadFile(openFileDialog1.FileName);
			}
		}
		
		void ToolStripButton3Click(object sender, EventArgs e)
		{
			if(MessageBox.Show("Сохранить результат перевода на руский язык? ('нет' - сохранить промежуточный результат на Эльюнди)","Выбор текста для сохранения", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if(saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					richTextBox2.SaveFile(saveFileDialog1.FileName);
				}
			}
			else if(saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					richTextBox3.SaveFile(saveFileDialog1.FileName);
				}
		}
		
		void ПомощьToolStripMenuItemClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("iexplore",System.IO.Path.GetFullPath("help.mht"));
		}
		
		void ВыделитьВсеToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(this.richTextBox1.Focused)
				this.richTextBox1.SelectAll();
			if(this.richTextBox2.Focused)
				this.richTextBox2.SelectAll();
			if(this.richTextBox3.Focused)
				this.richTextBox3.SelectAll();
		}
		
		void КопироватьToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(this.richTextBox1.Focused)
				this.richTextBox1.Copy();
			if(this.richTextBox2.Focused)
				this.richTextBox2.Copy();
			if(this.richTextBox3.Focused)
				this.richTextBox3.Copy();
		}
		
		void ВставитьToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(this.richTextBox1.Focused)
				this.richTextBox1.Cut();
			if(this.richTextBox2.Focused)
				this.richTextBox2.Cut();
			if(this.richTextBox3.Focused)
				this.richTextBox3.Cut();
		}
		
		void ВставитьToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(this.richTextBox1.Focused)
				this.richTextBox1.Paste();
			if(this.richTextBox2.Focused)
				this.richTextBox2.Paste();
			if(this.richTextBox3.Focused)
				this.richTextBox3.Paste();
		}
		
		void RichTextBox3FontChanged(object sender, EventArgs e)
		{
			MessageBox.Show("tr");
		}
		
		void ОПрограммеToolStripMenuItemClick(object sender, EventArgs e)
		{
			About a = new About();
			a.Show();
		}

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            Add_to_db childForm = new Add_to_db();
            childForm.FormClosed += new FormClosedEventHandler(childForm_FormClosed);
            childForm.Show();
            this.Enabled = false;
        }
        void childForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
        }  
	}
}
