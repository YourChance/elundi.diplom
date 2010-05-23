/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 05.04.2009
 * Time: 21:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of RTENoun.
	/// </summary>
	public class RTENoun : IModule
	{
		public RTENoun()
		{
		}
		
		public Slovo Analyze(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			return analyzed;
		}
		
		public Slovo Translate(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			TranslateOsnova(ref pr,ref analyzed);
			Chislo(ref analyzed);
			Rod(ref analyzed);
			Padezh(ref pr,place,ref analyzed);
			
			pr.SetSlovo(analyzed,place);
			
			return analyzed;
		}
		
		public void TranslateOsnova(ref Predlozhenie pr,ref Slovo slovo)
		{
			ArrayList al = pr.Dict.GetStrictRusEx(slovo.eSlovo);
            if(al.Count > 0)
			{
				slovo.eSlovo = ((DictSlovo)al[0]).El;
			}
			else
            {
				slovo.eSlovo = pr.Dict.CreateNewSlovoEx(slovo);
			}
		}
		
		public void Chislo(ref Slovo slovo)
		{
			if(slovo.ruSlovo.ruChislo == RuChislo.Mnozhestvennoe)
				slovo.eSlovo = "W"+slovo.eSlovo.Substring(1);
		}
		
		public void Rod(ref Slovo slovo)
		{
			if(slovo.odushevlennost == Odushevlennost.Odushevlennoe)
			{
			/*	if(slovo.ruSlovo.ruRod == RuRod.Muzhskoj)
					slovo.eSlovo = slovo.eSlovo+"-EVA";
				else if(slovo.ruSlovo.ruRod == RuRod.Muzhskoj)
					slovo.eSlovo = slovo.eSlovo+"-RVA";				*/
			}
		}
		
		public void Padezh(ref Predlozhenie pr,int place, ref Slovo slovo)
		{
			string ppred = "";
			
			switch(slovo.ruSlovo.ruPadezh)
			{
				case RuPadezh.Imenitelniy:
					break;
				case RuPadezh.Roditelniy:
					ppred = "FQV";
					break;
				case RuPadezh.Vinitelniy:
					ppred = "FF";
					break;
				case RuPadezh.Datelniy:
					ppred = "FZP";
					break;
				case RuPadezh.Tvoritelniy:
					ppred = "FT";
					break;
				case RuPadezh.Predlozhniy:
					ppred = "FZJ";
					break;
			}
			
			int i = place-1;
			
			while(i>0 && (pr[i].chastRechi == ChastRechi.Prichastie || pr[i].chastRechi == ChastRechi.Prilagatelnoe || pr[i].chastRechi == ChastRechi.Mestoimenie))
			{
				i= i-1;
			}
			
			if(i>=0 && pr[i].chastRechi != ChastRechi.Predlog)
			{
				Slovo ts = pr[i];
				ts.eSlovo = ts.eSlovo + " "+ppred;
				pr.SetSlovo(ts,i);
			}
			else
			{
				if(i>=0 && pr[i].eSlovo != ppred)
				{
					Slovo ts = pr[i];
					ts.eSlovo = ts.eSlovo + " "+ppred;
					pr.SetSlovo(ts,i);
				}
			}
		}
	}
}
