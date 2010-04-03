/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 23.02.2009
 * Time: 13:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data.SQLite;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of MorfologRus.
	/// </summary>
	public partial class MorfologRus
	{
		public MorfologRus()
		{
			InitializeGramTab();
		}
		
		public event EventHandler Tick;
		
		public Predlozhenie MorfAnalyze(Predlozhenie pr)
		{
			Open();
			Predlozhenie pred = pr;
			
			for(int j=0;j<pred.Count;j++)
			{
				Slovo s = (Slovo)pred[j];
				string slovo = s.rSlovo;
				string flextype = "";
				string grtype = "";
				slovo = slovo.ToUpper();
				
				bool cifra = false;
				
				try
				{
					int i = int.Parse(slovo);
					cifra = true;					
				}
				catch
				{
					cifra = false;
				}
				
				if(!cifra)
				{
				
					for(int i=slovo.Length;i>0;i--)
					{
						string osnova = slovo.Substring(0,i);
						ArrayList ar = GetDict(osnova);
						bool br = false;
						if(ar.Count>0)
						{
							//System.Windows.Forms.MessageBox.Show(osnova);
							foreach(string flexid in ar)
							{
							
							//	Console.WriteLine(osnova);
							string flex = GetFlex(flexid.Split(';')[0]);
						//System.Windows.Forms.MessageBox.Show(flexid.ToString());
								foreach(string f in flex.Split('%'))
								{
									if(f!="")
									{
										if(osnova+f.Split('*')[0]==slovo)
										{
										//	System.Windows.Forms.MessageBox.Show(osnova+flex.Split('%')[1].Split('*')[0],"Начальная форма: ");
											s.eSlovo = (osnova+flex.Split('%')[1].Split('*')[0]).ToLower();
										//	System.Windows.Forms.MessageBox.Show(f.Split('*')[1],"Тип флексии: ");
											flextype = f.Split('*')[1];
											grtype = flexid.Split(';')[1];
											br = true;
											break;
										}
									}
									if(br)
										break;
								}
							}						
						}
						if(br)
							break;
					}
					AnalyzeGramTab(ref s,flextype,grtype);
					pr.SetSlovo(s,j);
					if(s.chastRechi == ChastRechi.Suschestvitelnoe)
						pr.SetSlovo(PostAnalyzePadezh(pr,j),j);
				}
				else 
				{
					s.eSlovo = s.rSlovo;
					s.chastRechi = ChastRechi.Znak;
					pr.SetSlovo(s,j);
				}
				if(this.Tick != null)
					Tick(this,null);
			}
			
			Close();
			
			return pr;
		}
		
		public void Create()
		{
			string connectionString = "Data Source=rusflex.sqlitedb";
			System.Data.SQLite.SQLiteConnection conn = new SQLiteConnection(connectionString);
			conn.Open();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "CREATE TABLE dict (n INTEGER PRIMARY KEY,  slovo TEXT, flexid INTEGER, gr TEXT);";
			command.ExecuteNonQuery();
			command.CommandText = "CREATE TABLE flexions (n INTEGER PRIMARY KEY, flex TEXT);";
			command.ExecuteNonQuery();
			conn.Close();			
		}
		public System.Data.SQLite.SQLiteConnection conn;
		public System.Data.SQLite.SQLiteTransaction trans;
		
		public void Open()
		{
			string connectionString = "Data Source=rusflex.sqlitedb";
			conn = new SQLiteConnection(connectionString);
			conn.Open();	
			trans = conn.BeginTransaction();
		}
		
		public void Close()
		{
			trans.Commit();
			conn.Close();
		}
		
		public void ClearFlex()
		{
			SQLiteCommand command = new SQLiteCommand(conn);
			string ct = "DELETE FROM flexions WHERE 1;";
			command.CommandText = ct;
			command.ExecuteNonQuery();			
		}
		
		public void InsertFlex(string id,string flex)
		{
			SQLiteCommand command = new SQLiteCommand(conn);
			string ct = "INSERT INTO flexions (n,flex) VALUES ('"+id+"','"+flex.Replace("'","''")+"');";
			command.CommandText = ct;
			command.ExecuteNonQuery();
			Console.WriteLine(ct);
		}
		
		public void InsertDict(string id,string slovo,string flexid,string gr)
		{
			SQLiteCommand command = new SQLiteCommand(conn);
			string ct = "INSERT INTO dict (n,slovo,flexid,gr) VALUES ('"+id+"','"+slovo+"','"+flexid+"','"+gr+"');";
			command.CommandText = ct;
			command.ExecuteNonQuery();
			if(int.Parse(id)%1000==0)
				Console.WriteLine("{0:0.00}",(double.Parse(id)/(double)173545));
		}
		
		public ArrayList GetDict(string slovo)
		{
			ArrayList results = new ArrayList();
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT slovo,flexid,gr FROM dict WHERE slovo = '"+slovo+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				results.Add(reader.GetInt64(1).ToString()+";"+reader.GetString(2).ToString());
			}
			return results;
		}
		
		public string GetFlex(string flexid)
		{
			string result = "";
			SQLiteCommand command = new SQLiteCommand(conn);
			command.CommandText = "SELECT flex FROM flexions WHERE n = '"+flexid+"';";
			SQLiteDataReader reader = command.ExecuteReader();
			while(reader.Read())
			{
				result = reader.GetString(0);
			}
			return result;
		}
	}
}
