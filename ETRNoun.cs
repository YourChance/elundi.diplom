/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 10.12.2008
 * Time: 20:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of ETRNoun.
	/// </summary>
	public partial class ETRNoun : IModule
	{
		public ETRNoun()
		{
		}
		
		public struct RuMorf
		{
			public string osnova;
			public string okonchanie;
			public RuMorf(string os, string ok)
			{
				osnova = os;
				okonchanie = ok;
			}
		}
		
		protected string osnova;
		
		public Slovo Analyze(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			PreAnalyze(pr,place,ref analyzed);
			PostAnalyze(pr,place,ref analyzed);			
			
			return analyzed;
		}
		
		public Slovo Translate(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			Sklonenie(ref analyzed);
			
			return analyzed;
		}	
		
		protected void ShiftPadezh(ref Slovo slovo)
		{
			switch(slovo.padezh)
			{
				case Padezh.Vinitelnij:
					slovo.ruSlovo.ruPadezh = RuPadezh.Vinitelniy;
					break;
				case Padezh.Prityazhatelnij:
					slovo.ruSlovo.ruPadezh = RuPadezh.Roditelniy;
					break;
				case Padezh.Datelnij:
					slovo.ruSlovo.ruPadezh = RuPadezh.Datelniy;
					break;
				case Padezh.Tvoritelnij:
					slovo.ruSlovo.ruPadezh = RuPadezh.Tvoritelniy;
					break;
				case Padezh.Imenitelnij:
					slovo.ruSlovo.ruPadezh = RuPadezh.Imenitelniy;
					break;
				default:
					slovo.ruSlovo.ruPadezh = RuPadezh.Predlozhniy;
					break;
			}		
		}
		
		protected void PreAnalyze(Predlozhenie pr,int place,ref Slovo slovo)
		{
			AnalyzePadezh(pr,place,ref slovo);
			AnalyzeChislo(ref slovo);
			AnalyzeRod(ref slovo);
			FindOsnova(ref slovo);
			TranslateOsnova(ref slovo);
		}
		
		public void PostAnalyze(Predlozhenie pr,int place,ref Slovo slovo)
		{
			PostAnalyzeRod(ref slovo);
			PostAnalyzePadezh(pr,place,ref slovo);			
		}
		
		protected void AnalyzePadezh(Predlozhenie pr,int place,ref Slovo slovo)
		{
			int i = place-1;
			
			while(i>0 && (pr[i].chastRechi == ChastRechi.Prilagatelnoe || pr[i].chastRechi == ChastRechi.Prichastie || pr[i].chastRechi == ChastRechi.Mestoimenie))
			{
				i = i-1;
			}
			
			if(pr[i]!=null && pr[i].chastRechi == ChastRechi.Predlog)
			{
				Slovo predlog = pr[i];
				switch(predlog.eSlovo)
				{
					case "FF":
						slovo.padezh = Padezh.Vinitelnij;
						break;
					case "FQV":
					case "FJB":
						slovo.padezh = Padezh.Prityazhatelnij;
						break;
					case "FZP":
						slovo.padezh = Padezh.Datelnij;
						break;
					case "FT":
						slovo.padezh = Padezh.Tvoritelnij;						
						break;
					case "FR":
						slovo.padezh = Padezh.Instrumentalnij;
						break;
					case "FZJ":
						slovo.padezh = Padezh.Prisvyazochnij;
						break;
					case "FOB":
						slovo.padezh = Padezh.Dejstviya;
						break;
					case "FZO":
						slovo.padezh = Padezh.Predmeta;
						break;
					case "FZE":
						slovo.padezh = Padezh.Napravleniya;
						break;
					case "FA":
						slovo.padezh = Padezh.Mestoprebivaniya;
						break;
					case "FWV":
						slovo.padezh = Padezh.Obrasheniya;
						break;
					case "FSVE":
						slovo.padezh = Padezh.Avtora;
						break;
					case "FRZF":
						slovo.padezh = Padezh.Nazvaniya;
						break;
					case "FVA":
						slovo.padezh = Padezh.Celi;
						break;
					case "FFVY":
						slovo.padezh = Padezh.Prichini;
						break;
					case "FFV":
						slovo.padezh = Padezh.Sledstviya;
						break;					
				}
			}
			else
			{
				slovo.padezh = Padezh.Imenitelnij;
			}				
		}
		
		protected void AnalyzeChislo(ref Slovo slovo)
		{
			switch(slovo.eSlovo[0])
			{
				case 'Q':
					slovo.chislo = Chislo.Edinstvennoe;
					slovo.ruSlovo.ruChislo = RuChislo.Edinstvennoe;
					if(slovo.eSlovo.IndexOf("QBA")!=-1)
						slovo.chislo = Chislo.Odinochnoe;
					if(slovo.eSlovo.IndexOf("WBA")!=-1)
						slovo.chislo = Chislo.Malochislennoe;
					break;
				case 'W':
					slovo.chislo = Chislo.Mnozhestvennoe;
					slovo.ruSlovo.ruChislo = RuChislo.Mnozhestvennoe;
					if(slovo.eSlovo.IndexOf("QBA")!=-1)
						slovo.chislo = Chislo.Neopredelennoe;
					if(slovo.eSlovo.IndexOf("WBA")!=-1)
						slovo.chislo = Chislo.Mnogoobraznoe;
					break;
				default:
					// здесь будет обработка ошибок
					break;
			}
		}
		
		protected void AnalyzeRod(ref Slovo slovo)
		{
			slovo.rod = Rod.Obshij;
			if(slovo.eSlovo.IndexOf("]") != -1)
			{
				slovo.rod = Rod.Muzhskoj;
				slovo.ruSlovo.ruRod = RuRod.Muzhskoj;
			}
			if(slovo.eSlovo.IndexOf("[") != -1)
			{
				slovo.rod = Rod.Zhenskij;
				slovo.ruSlovo.ruRod = RuRod.Zhenskij;
			}
			if(slovo.eSlovo.IndexOf("EVA") != -1)
			{
				slovo.rod = Rod.Muzhskoj;
				slovo.ruSlovo.ruRod = RuRod.Muzhskoj;
			}
			if(slovo.eSlovo.IndexOf("RVA") != -1)
			{
				slovo.rod = Rod.Zhenskij;
				slovo.ruSlovo.ruRod = RuRod.Zhenskij;
			}
		}
		
		protected void FindOsnova(ref Slovo slovo)
		{
			osnova = slovo.eSlovo;
			osnova = osnova.Replace("EVA","");
			osnova = osnova.Replace("RVA","");
			osnova = osnova.Replace("QBA","");
			osnova = osnova.Replace("WBA","");
			
			if(osnova.Length>3 && osnova.Substring(0,2)==osnova.Substring(2,2))	
				osnova = osnova.Remove(0,2);
			if(osnova[0] == 'W')
				osnova = "Q"+osnova.Substring(1);
			while(osnova[osnova.Length-1]=='-')
			{
				osnova = osnova.Remove(osnova.Length-1,1);
			}
		}
		
		protected void TranslateOsnova(ref Slovo slovo)
		{
			Dictionary dict = new Dictionary();
			ArrayList al = dict.GetStrict(osnova);
			if(slovo.eSlovo == "Q-")
				slovo.rSlovo = "я";
			else if(slovo.eSlovo == "W-")
				slovo.rSlovo = "ты";
			else if(al.Count > 0)
			{
				slovo.rSlovo = ((DictSlovo)al[0]).Rus;
			}
			else
			{
				slovo.rSlovo = "";
				//throw new ETRTranslatorException(ETRError.SlovaNetVSlovare,"Слово не найдено в словаре");
			}
		}
		
		protected void PostAnalyzeRod(ref Slovo slovo)
		{
			if(slovo.rod == Rod.Obshij)
			{
				Zaliznyak z = new Zaliznyak();
				string s = (string)(z.GetStrict(slovo.rSlovo.Replace("ё","е")))[0];
				string paradigma = s.Split(';')[1];
				string digits = "0123456789";
				string osnBukvSimvol = s.Substring(s.IndexOfAny(digits.ToCharArray()));
				osnBukvSimvol = osnBukvSimvol.Substring(osnBukvSimvol.IndexOf(' '));
				if(osnBukvSimvol.IndexOfAny(digits.ToCharArray()) != -1)
					osnBukvSimvol = osnBukvSimvol.Substring(0,osnBukvSimvol.IndexOfAny(digits.ToCharArray()));
				
				if(osnBukvSimvol.IndexOf("жо")!=-1)
				{
					slovo.ruSlovo.ruRod = RuRod.Zhenskij;
					slovo.odushevlennost = Odushevlennost.Odushevlennoe;
				}
				else if(osnBukvSimvol.IndexOf("мо")!=-1)
				{
					slovo.ruSlovo.ruRod = RuRod.Muzhskoj;
					slovo.odushevlennost = Odushevlennost.Odushevlennoe;
				}
				else if(osnBukvSimvol.IndexOf("со")!=-1)
				{
					slovo.ruSlovo.ruRod = RuRod.Srednij;
					slovo.odushevlennost = Odushevlennost.Odushevlennoe;
				}
				else if(osnBukvSimvol.IndexOf("ж")!=-1)
				{
					slovo.ruSlovo.ruRod = RuRod.Zhenskij;					
					slovo.odushevlennost = Odushevlennost.Neodushevlennoe;
				}
				else if(osnBukvSimvol.IndexOf("м")!=-1)
				{
					slovo.ruSlovo.ruRod = RuRod.Muzhskoj;					
					slovo.odushevlennost = Odushevlennost.Neodushevlennoe;
				}
				else 
				{
					slovo.ruSlovo.ruRod = RuRod.Srednij;
					slovo.odushevlennost = Odushevlennost.Neodushevlennoe;
				}
			}
		}
		
		public void PostAnalyzePadezh(Predlozhenie pr,int place,ref Slovo slovo)
		{
			if(pr[place-1]!=null && pr[place-1].chastRechi == ChastRechi.Predlog)
			{
				Slovo predlog = pr[place-1];
				//Здесь будет анализ падежа на основе падежа эльюнди и русского предлога.
			}
			
			ShiftPadezh(ref slovo);
		}
		
		protected RuMorf AnalyzeRuMorf(Slovo slovo)
		{
			string rus = slovo.rSlovo;
			
			string ruGlas = "ёуеаояиюэыйь";
			string ok = "";
			string osn = rus;
			
			if(ruGlas.IndexOf(rus[rus.Length-1])!=-1)
			{
				ok = rus.Substring(rus.Length-1);
				osn = rus.Substring(0,rus.Length-1);
			}
						
			return new RuMorf(osn,ok);
		}
		
		protected void Sklonenie(ref Slovo slovo)
		{
			RuMorf ruMorf = AnalyzeRuMorf(slovo);
			
			RuMorf izmRuMorf = new RuMorf(ruMorf.osnova,"");
			
			Zaliznyak z = new Zaliznyak();
			string s = (string)(z.GetStrict(slovo.rSlovo.Replace("ё","е")))[0];
			string paradigma = s.Split(';')[1];
			string digits = "0123456789";
			string osnBukvSimvol = s.Substring(s.IndexOfAny(digits.ToCharArray()));
			osnBukvSimvol = osnBukvSimvol.Substring(osnBukvSimvol.IndexOf(' '));
			string indexes = (osnBukvSimvol.IndexOfAny(digits.ToCharArray()) != -1)?osnBukvSimvol.Substring(osnBukvSimvol.IndexOfAny(digits.ToCharArray())):"1";
			if(osnBukvSimvol.IndexOfAny(digits.ToCharArray())!= -1)
				osnBukvSimvol = osnBukvSimvol.Substring(0,osnBukvSimvol.IndexOfAny(digits.ToCharArray()));
			
			char firstIndex = indexes[0];
			char secondIndex = ' ';
			if(indexes.Length>1)
				secondIndex = indexes[1];
			if(secondIndex == '*')
				secondIndex = indexes[2];
			
			char posl = ruMorf.osnova[ruMorf.osnova.Length-1];
			bool posl_is_ship = (posl=='ч')||(posl=='ш')||(posl=='щ')||(posl=='ж');
			
			
			bool udarnoe = false;
			
			if(slovo.ruSlovo.ruChislo == RuChislo.Edinstvennoe)
				udarnoe = (secondIndex == 'в' || secondIndex == 'D' || secondIndex == 'f');
			else {
				if(slovo.ruSlovo.ruPadezh == RuPadezh.Imenitelniy)
					udarnoe = (secondIndex == 'в' || secondIndex == 'с');
				else udarnoe = (secondIndex == 'в' || secondIndex == 'с' || secondIndex == 'е' || secondIndex == 'f');
			}
			
			izmRuMorf.okonchanie = OkonchanieSklonenia(ruMorf.okonchanie,firstIndex,slovo.ruSlovo.ruPadezh,slovo.ruSlovo.ruRod,slovo.odushevlennost,udarnoe,posl_is_ship,slovo.ruSlovo.ruChislo,(indexes.IndexOf("\"1\"")!=-1),(indexes.IndexOf("\"2\"")!=-1));
			
			bool zvezdochka = (indexes.IndexOf("**")==-1&&indexes.IndexOf("*")!=-1);
			
			bool osoboe_cheredovanie = (indexes.IndexOf("**")!=-1);
			
			bool rod_padezh_mn_chisla = (slovo.ruSlovo.ruPadezh == RuPadezh.Roditelniy && slovo.ruSlovo.ruChislo == RuChislo.Mnozhestvennoe) || (slovo.ruSlovo.ruPadezh == RuPadezh.Vinitelniy && slovo.ruSlovo.ruChislo == RuChislo.Mnozhestvennoe && slovo.odushevlennost == Odushevlennost.Odushevlennoe);
			
			if(zvezdochka)
				izmRuMorf.osnova = BeglayaGlasnaya(izmRuMorf.osnova,izmRuMorf.okonchanie,slovo.rSlovo,slovo.ruSlovo.ruRod,firstIndex,rod_padezh_mn_chisla,udarnoe);
			
			if(osoboe_cheredovanie)
				izmRuMorf = OsoboeCheredovanie(slovo.rSlovo,izmRuMorf,firstIndex,slovo.ruSlovo.ruChislo,slovo.ruSlovo.ruPadezh,slovo.odushevlennost);
									
			slovo.rSlovo = izmRuMorf.osnova+izmRuMorf.okonchanie;
		}
		
	}
}
