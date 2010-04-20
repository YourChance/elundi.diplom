using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Data.SQLite;
using System.Data;

    public struct Noun
    {
        public Chislo chislo;
        public Rod rod;
        public Padezh padezh;
        public string osnova;
        public string english;
        public override string ToString()
        {
            return
            "\r\nОснова: " + this.osnova +
            "\r\nПеревод: " + this.english +
            "\r\nРод: " + this.rod +
            "\r\nПадеж: " + this.padezh +
            "\r\nЧисло: " + this.chislo + "\r\n";
        }
    }

    public struct Glagol
    {
        public string english;
        public Vremya vremya;
        public Zalog zalog;
        public Naklonenie naklonenie;
        public Vid vid;
        public Sostoynie sostoyanie;
        public override string ToString()
        {
            return "\r\nПеревод: " + this.english +
                   "\r\nВремя: " + this.vremya +
                   "\r\nЗалог: " + this.zalog +
                   "\r\nНаклонение: " + this.naklonenie +
                   "\r\nВид: " + this.vid +
                   "\r\nСостояние: " + this.sostoyanie;
        }
    }

    public struct Mestoimenie
    {
        public string english;
        public override string ToString()
        {
            return
                "\r\nПеревод: " + this.english;
        }
    }

    public struct Prilagatelnoe
    {
        public string osnova;
        public string english;
        public Rod rod;
        public Padezh padezh;
        public StepenSravneniya stepenSravneniya;
        public Chislo chislo;
        public override string ToString()
        {
            return
                   "\r\nОснова: " + this.osnova +
                   "\r\nПеревод: " + this.english +
                   "\r\nРод: " + this.rod +
                   "\r\nПадеж: " + this.padezh +
                   "\r\nСтепень сравнения: " + this.stepenSravneniya +
                   "\r\nЧисло: " + this.chislo + "\r\n";
        }
    }
    public struct Predlog
    {
        public string english;
        public override string ToString()
        {
            return

            "\r\nПеревод: " + this.english + "\r\n";
        }
    }

	public enum ChastRechi
	{
		Neopredelennaya,
		Suschestvitelnoe,
		Chislitelnoe,
		ChislitelnoePoryadkovoe,
		Prilagatelnoe,
		Glagol,
		Prichastie,
		Deeprichastie,
		Narechie,
		Mestoimenie,
		Predlog,
		Souz,
		Chastica,
		Sluzhebnoe,
		Mezhdometie,
		Znak
	}
	
	//
	// Добавлено Ромой
	//
	public enum Sostoynie
    {
        Обычное,
        Безличное
    }
	
	public enum Lico
	{
		Первое,
		Второе,
		Третье
	}

	public enum Rod
	{
		Muzhskoj,
		Zhenskij,
		Obshij
	}
	
	public enum Odushevlennost
	{
		Odushevlennoe,
		Neodushevlennoe
	}
	
	public enum Chislo
	{
		Edinstvennoe,
		Mnozhestvennoe,
		Odinochnoe,
		Malochislennoe,
		Neopredelennoe,
		Mnogoobraznoe
	}
	
	public enum Padezh
	{
		Imenitelnij,
		Vinitelnij,
		Prityazhatelnij,
		Datelnij,
		Tvoritelnij,
		Instrumentalnij,
		Prisvyazochnij,
		Dejstviya,
		Predmeta,
		Napravleniya,
		Mestoprebivaniya,
		Obrasheniya,
		Avtora,
		Nazvaniya,
		Celi,
		Prichini,
		Sledstviya
	}
	
	public enum Vid
	{
		Zavershennost,
		NevozvratnayaZavershennost,
		Mgnovennost,
		NachaloDejstviya,
		OgranichenieDlitelnosti,
		NeopredelennayaDlitelnost,
		PostoyannayaDlitelnost,
		NezavershennostDejstviya
	}
	
	public enum Zalog
	{
		Vzaimniy,
		Vozvratniy,
		Stradatelniy,
		Dejstvitekniy
	}
	
	public enum Vremya
	{
		Nastoyaschee,
		Proshedshee,
		Buduschee,
		Postoyannoe,
		NastoyascheeDlitelnoe,
		NastoyascheeVNastoyaschem,
		NastoyascheeSProshedshim,
		NastoyascheeSBuduschim,
		ProshedsheeDlitelnoe,
		ProshedsheeSNastoyaschim,
		Davnoproshedshee,
		ProshedsheeBuduscheeBezNastoyaschego,
		BuduscheeDlitelnoe,
		BuduscheeVNastoyaschem,
		BuduscheeSProshedshim,
		BuduscheeDalekoe
	}
	
	public enum Naklonenie
	{
		Izjavitelnoe,
		Povelitelnoe,
		Soslagatelnoe,		
		Zaochnoe,
		Infinitiv
	}
	
	public enum StepenSravneniya
	{
		Polozhitelnaya,
		Bolee,
		Menee,
		Naibolee,
		Naimenee
	}
	
	/// <summary>
	/// Description of Slovo.
	/// </summary>
	public class Slovo
	{
		public string eSlovo;
		public string rSlovo;	
		public ChastRechi chastRechi;
		public Rod rod;
		public Odushevlennost odushevlennost;
		public Chislo chislo;
		public Padezh padezh;
		public Vid vid;
		public Zalog zalog;
		public Vremya vremya;
		public Naklonenie naklonenie;
		public StepenSravneniya stepenSravneniya;
		public Sostoynie sostoynie; // Добавлено Ромой
		public Lico lico;
		public RuSlovo ruSlovo;
        public EnSlovo enSlovo;

        public object ExtraData;

		public Slovo()
		{
			ruSlovo = new RuSlovo();
            enSlovo = new EnSlovo();
		}
		public Slovo(string eSlovo)
		{
			this.eSlovo = eSlovo;
			ruSlovo = new RuSlovo();
            enSlovo = new EnSlovo();
		}
		public Slovo(string rSlovo,ChastRechi chastRechi)
		{
			this.rSlovo = rSlovo;
			this.chastRechi = chastRechi;
			ruSlovo = new RuSlovo();
            enSlovo = new EnSlovo();
		}
		public Slovo(string eSlovo,ChastRechi chastRechi,bool b)
		{
			this.eSlovo = eSlovo;
			this.chastRechi = chastRechi;
			ruSlovo = new RuSlovo();
            enSlovo = new EnSlovo();
		}
	}

    public class EnSlovo
    {
        public string slovo;
    }
	
	public class RuSlovo
	{
		public RuPadezh ruPadezh;
		public RuRod ruRod;	
		public RuChislo ruChislo;
		public RuForma ruForma;
		public RuVid ruVid;
		public RuLico ruLico;
		public RuPerehodnost ruPerehodnost;
	}
	
	public enum RuPadezh
	{
		Imenitelniy,
		Roditelniy,
		Vinitelniy,
		Datelniy,
		Tvoritelniy,
		Predlozhniy
	}
	
	public enum RuRod
	{
		Muzhskoj,
		Zhenskij,
		Srednij
	}
	
	public enum RuChislo
	{
		Edinstvennoe,
		Mnozhestvennoe
	}
	
	public enum RuForma
	{
		Polnaya,
		Kratkaya
	}
	
	public enum RuVid
	{
		Sovershenniy,
		Nesovershenniy
	}
	
	public enum RuPerehodnost
	{
		Perehodniy,
		Neperehodniy
	}
	
	public enum RuLico
	{
		Pervoe,
		Vtoroe,
		Tretie,
		Bezlichnoe
	}
	
	public class Predlozhenie
	{
		ArrayList slova;
		public Predlozhenie()
		{
			slova = new ArrayList();
			Dict = new Dictionary();
		}
		public Slovo this[int n]
		{
			get
			{
				if(n>=0&&n<slova.Count)
					return (Slovo)slova[n];
				else return null;
			}
            set
            {
                if (n >= 0 && n < slova.Count)
                    slova[n] = value;
            }
		}
		public void SetSlovo(Slovo s,int i)
		{
			slova[i] = s;
		}
		public void AddSlovo(Slovo s)
		{
			slova.Add(s);
		}
		public int Count
		{
			get
			{
				return slova.Count;
			}
		}

        public string ToEnString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Slovo s in slova)
                sb.Append(s.enSlovo.slovo + " "/*+"("+s.chastRechi.ToString()+") "*/);
            return sb.ToString();
        }

		public string ToRString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(Slovo s in slova)
				sb.Append(s.rSlovo+" "/*+"("+s.chastRechi.ToString()+") "*/);
			return sb.ToString();
		}
		public string ToEString()
		{
			StringBuilder sb = new StringBuilder();
			foreach(Slovo s in slova)
				if(s.chastRechi != ChastRechi.Znak)
					sb.Append(" "+s.eSlovo);
			else {
				try
				{
					if(s.eSlovo.IndexOf(' ')==-1)
						int.Parse(s.eSlovo);
					else int.Parse(s.eSlovo.Substring(0,s.eSlovo.IndexOf(' ')));
					sb.Append(" "+s.eSlovo);
				}
				catch(Exception /*e*/)
				{
					/*System.Windows.Forms.MessageBox.Show(e.ToString());
					System.Windows.Forms.MessageBox.Show(s.eSlovo);*/
					sb.Append(s.eSlovo);
				}
			}
			return sb.ToString();
		}
		public Dictionary Dict;
	}
	
	public interface IModule
	{
		Slovo Analyze(Predlozhenie p, int poziciya);
		Slovo Translate(Predlozhenie p, int poziciya);
	}
	
	public class Dictionary
	{
		public void Create()
		{
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "CREATE TABLE dict (n INTEGER PRIMARY KEY,  rus TEXT, el TEXT, ex TEXT, proizn TEXT);";
			command.ExecuteNonQuery();
			conn.Close();
		}
		public void Insert(DictSlovo ds)
		{
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "INSERT INTO dict (rus, el, ex, proizn) VALUES ('"+ds.Rus+"','"+ds.El+"','"+ds.Ex+"','"+ds.Proizn+"');";
			command.ExecuteNonQuery();
			conn.Close();
		}
		
		public void InsertEx(DictSlovo ds)
		{
			SQLiteCommand command = new SQLiteCommand(connect);
			command.CommandText = "INSERT INTO dict (rus, el, ex, proizn) VALUES ('"+ds.Rus+"','"+ds.El+"','"+ds.Ex+"','"+ds.Proizn+"');";
			command.ExecuteNonQuery();			
		}
		
		public string CreateNewSlovo(Slovo slovo)
		{
			int i = 1;
			try
			{
				StreamReader sr = new StreamReader("newslovo.ini");
				i = int.Parse(sr.ReadLine());
				sr.Close();
			}
			catch
			{
				i=1;
			}
			int j = i+1;
			string bukvi = "QWERTYUIOPASDFGHJKLZXCVBN";
			string newslovo = "";
			
			switch(slovo.chastRechi)
			{
				case ChastRechi.Suschestvitelnoe:
					newslovo += "Q-";
					break;
				case ChastRechi.Prilagatelnoe:
					newslovo += "E-";
					break;
				case ChastRechi.Glagol:
					newslovo += "R-";
					break;
				case ChastRechi.Prichastie:
					newslovo += "I-";
					break;
				case ChastRechi.Deeprichastie:
					newslovo += "O-";
					break;
				case ChastRechi.Narechie:
					newslovo += "P-";
					break;
				case ChastRechi.Mestoimenie:
					newslovo += "A-";
					break;
				case ChastRechi.Predlog:
					newslovo += "F-";
					break;
				case ChastRechi.Souz:
					newslovo += "S-";
					break;
				case ChastRechi.Chastica:
					newslovo += "J-";
					break;	
				case ChastRechi.Mezhdometie:
					newslovo += "G-";
					break;
				default:
					newslovo += "M-";
					break;
			}		
			newslovo += "M";
			while(i>0)
			{
				newslovo += bukvi[i%bukvi.Length];
				i = i/bukvi.Length;
			}
			newslovo += "M";
			DictSlovo ds = new DictSlovo(slovo.eSlovo,newslovo,"","бред");
			this.Insert(ds);
			FileStream fs = new FileStream("newslovo.ini", FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine(j.ToString());
			sw.Close();

			return newslovo;
		}
		
		public string CreateNewSlovoEx(Slovo slovo)
		{
			int i = 1;
			try
			{
				StreamReader sr = new StreamReader("newslovo.ini");
				i = int.Parse(sr.ReadLine());
				sr.Close();
			}
			catch
			{
				i=1;
			}
			int j = i+1;
			string bukvi = "QWERTYUIOPASDFGHJKLZXCVBN";
			string newslovo = "";
			
			switch(slovo.chastRechi)
			{
				case ChastRechi.Suschestvitelnoe:
					newslovo += "Q-";
					break;
				case ChastRechi.Prilagatelnoe:
					newslovo += "E-";
					break;
				case ChastRechi.Glagol:
					newslovo += "R-";
					break;
				case ChastRechi.Prichastie:
					newslovo += "R-";
					break;
				case ChastRechi.Deeprichastie:
					newslovo += "O-";
					break;
				case ChastRechi.Narechie:
					newslovo += "P-";
					break;
				case ChastRechi.Mestoimenie:
					newslovo += "A-";
					break;
				case ChastRechi.Predlog:
					newslovo += "F-";
					break;
				case ChastRechi.Souz:
					newslovo += "S-";
					break;
				case ChastRechi.Chastica:
					newslovo += "J-";
					break;	
				case ChastRechi.Mezhdometie:
					newslovo += "G-";
					break;
				default:
					newslovo += "M-";
					break;
			}		
			newslovo += "M";
			while(i>0)
			{
				newslovo += bukvi[i%bukvi.Length];
				i = i/bukvi.Length;
			}
			newslovo += "M";
			DictSlovo ds = new DictSlovo(slovo.eSlovo,newslovo,"","бред");
			this.InsertEx(ds);
			FileStream fs = new FileStream("newslovo.ini", FileMode.Create);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine(j.ToString());
			sw.Close();

			return newslovo;
		}
		
		public ArrayList Get(string el)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE el LIKE '%"+el+"%';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			conn.Close();
			return results;
		}
		
		private System.Data.SQLite.SQLiteConnection connect;
		
		public void Open()
		{
			string connectionString = "Data Source=dict.sqlitedb";
			connect = new SQLiteConnection(connectionString);
			connect.Open();			
		}
		
		public void Close()
		{
			connect.Close();
		}
		
		public ArrayList GetStrictEx(string el)
		{
			ArrayList results = new ArrayList();
			SQLiteCommand command = new SQLiteCommand(connect);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE el='"+el+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}			
			return results;
		}
		
		public ArrayList GetStrict(string el)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE el='"+el+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			conn.Close();
			return results;
		}
		public ArrayList GetRus(string ru)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE rus LIKE '%"+ru+"%';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			conn.Close();
			return results;
		}
		public ArrayList GetStrictRus(string ru)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE rus='"+ru+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			conn.Close();
			return results;
		}
		public ArrayList GetStrictRusEx(string ru)
		{
			ArrayList results = new ArrayList();
			SQLiteCommand command = new SQLiteCommand(connect);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict WHERE rus='"+ru+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			return results;
		}
		public ArrayList GetAll()
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=dict.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT rus, el, ex, proizn FROM dict;";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(new DictSlovo(reader.GetString(0),reader.GetString(1),reader.GetString(2),reader.GetString(3)));
			}
			conn.Close();
			return results;
		}		
	}
	
	public struct DictSlovo
	{
		public string Rus;
		public string El;
		public string Ex;
		public string Proizn;
		public DictSlovo(string rus,string el,string ex,string proizn)
		{
			Rus = rus;
			El = el;
			Ex = ex;
			Proizn = proizn;
		}
	}
	
	public class ETRTranslatorException : System.Exception
	{
		public ETRError Error;
		public string ETRMessage;
		public ETRTranslatorException(ETRError error,string message)
		{
			Error = error;
			ETRMessage = message;
		}
	}
	
	public enum ETRError
	{
		SlovaNetVSlovare
	}
	
	public class Zaliznyak
	{
		public ArrayList Get(string slovo)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=zaliznyak.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT slovo,paradigma FROM dict WHERE slovo LIKE '%"+slovo+"%';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(reader.GetString(0)+";"+reader.GetString(1));
			}
			conn.Close();
			return results;
		}
		public ArrayList GetStrict(string slovo)
		{
			ArrayList results = new ArrayList();
			string connectionString = "Data Source=zaliznyak.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT slovo,paradigma FROM dict WHERE slovo = '"+slovo+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(reader.GetString(0)+";"+reader.GetString(1));
			}
			conn.Close();
			if(results.Count==0)
				results.Add(";1 1");
			return results;
		}
	}