/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 10.06.2009
 * Time: 22:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of ETROther.
	/// </summary>
	public class ETROther : IModule
	{
		public ETROther()
		{
		}
		
		public Slovo Analyze(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			//if(analyzed.chastRechi == ChastRechi.Mestoimenie)
				CopyCharact(pr,place,ref analyzed);
			
			
			
			return analyzed;
		}
		
		private void CopyCharact(Predlozhenie pr, int place, ref Slovo analyzed)
		{
			
			int i=(place-4>=0?place-4:0);
			int max = (place+4>pr.Count?pr.Count:place+4);
			while(i<max)
			{
            	Slovo slovpoisk = pr[i];
            	if (slovpoisk.chastRechi == ChastRechi.Suschestvitelnoe)
            	{
                	
            		break;
            	}
                else i++;
            }
            if (i != pr.Count)
            {
            	analyzed.ruSlovo.ruChislo = pr[i].ruSlovo.ruChislo;
               analyzed.ruSlovo.ruPadezh = pr[i].ruSlovo.ruPadezh;
               analyzed.ruSlovo.ruRod = pr[i].ruSlovo.ruRod;
               //throw new Exception( pr[i].rSlovo);
              
            }
            else
            {
                analyzed.ruSlovo.ruChislo = RuChislo.Edinstvennoe;
                analyzed.ruSlovo.ruPadezh = RuPadezh.Imenitelniy;
                analyzed.ruSlovo.ruRod = RuRod.Muzhskoj;
            }
		}
		
		public Slovo Translate(Predlozhenie pr,int place)
		{
			Slovo analyzed = pr[place];
			
			Dictionary dict = new Dictionary();
			ArrayList al = dict.GetStrict(analyzed.eSlovo);
            if(al.Count > 0)
			{
				analyzed.rSlovo = ((DictSlovo)al[0]).Rus;
			}
			else
            {
				analyzed.rSlovo = analyzed.eSlovo;
			}
			
		//	if(analyzed.chastRechi == ChastRechi.Mestoimenie)
				analyzed.rSlovo = MestChange(analyzed,analyzed.rSlovo);
			
			return analyzed;
		}	
		
		private string MestChange(Slovo inSlovo, string ruSlovo){

		// 1. Склонение личных местоимений я, ты; мы, вы; он (оно, она), они.
		// Я (1 лицо)
		
		
		
		if (ruSlovo=="я")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "я";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "меня";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "мне";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "меня";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "мной";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "мне";     break; 				
			}   
		}
		
		// **
		
		// ты (1 лицо)
		
		if (ruSlovo=="ты")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "ты";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "тебя";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "тебе";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "тебя";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "тобой";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "тебе";     break; 				
			}   
		}
		
		// **
		
		// мы (2лицо)
		
		if (ruSlovo=="мы")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "мы";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "нас";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "нам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "нас";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "нами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "нас";     break; 				
			}   
		}
		
		// **
		
		// вы (2 лицо)
		
		if (ruSlovo=="вы")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "вы";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "вас";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "вам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "вас";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "вами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "вас";     break; 				
			}   
		}
		
		// **
		
		// он 
		
		if (ruSlovo=="он")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "он";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "его";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "ему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "его";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "им";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "нём";     break; 						
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "оно";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "его";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "ему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "его";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "им";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "нём";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "она";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "её";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "ей";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "её";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "ей";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "ней";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "они";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "их";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "им";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "их";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "ими";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "них";     break; 				
				}   
			break; 
		
			}	
			
		}
		
		// **
		
		
		// 2. Возвратное местоимение себя не имеет формы именительного падежа;
		// себя
		
		if (ruSlovo=="себя")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "себя";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "себе";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "себя";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "собой";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "себе";     break; 				
			}   
		}
		
		// **
		
		// 3. Притяжательные местоимения мой, твой, наш, ваш, свой, указательные тот, этот, такой
		
		// Мой
		
		if (ruSlovo=="мой")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "мой";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "моего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "моему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "мой";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "моим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "моём";     break; 	
					
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "моё";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "моего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "моему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "моё";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "моим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "моём";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "моя";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "моей";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "моей";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "мою";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "моей";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "моей";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "мои";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "моих";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "моим";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "мои";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "моими";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "моих";     break; 				
				}   
			break; 
		
			}	
			
		}
		
		//**
		
		
		
		
		
		// этот
		
		if (ruSlovo=="этот")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "этот";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "этого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "этому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "это";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "этим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "этом";     break; 					
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "это";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "этого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "этому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "это";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "этим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "этом";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "эта";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "этой";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "этой";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "эту";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "этой";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "этой";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "эти";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "этих";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "этим";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "эти";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "этими";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "этих";     break; 				
				}  
			break; 
		
			}	
			
		}
		
		// **
		
		// Следует различать склонение местоимений самый и сам.
		
		// самый
		
		if (ruSlovo=="самый")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "самый";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "самый";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самым";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самом";     break; 					
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "самое";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "самое";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самым";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самом";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "самая";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "самую";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самой";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "самые";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "самых";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "самым";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "самые";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "самыми";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "самых";     break; 				
				}  
			break; 
		
			}	
			
		}
		
		// **
		
		// сам
		if (ruSlovo=="сам")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "сам";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "сам";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самом";     break; 				
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "само";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самого";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самому";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "само";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самом";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "сама";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "саму";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "самой";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "самой";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "сами";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "самих";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "самим";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "сами";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "самими";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "самих";     break; 				
				}   
			break; 
		
			}	
			
		}
		// **
		
		// Местоимение весь (всё, вся, все)
		
		// весь
		
		if (ruSlovo=="весь")
		{
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "весь";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "всего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "всему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "весь";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "всем";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "всём";     break; 				
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "всё";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "всего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "всему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "всё";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "всем";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "всём";     break; 				
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "вся";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "всей";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "всей";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "всю";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "всей";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "всей";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "все";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "всех";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "всем";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "все";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "всеми";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "всех";     break; 				
				}   
			break; 
		
			}	
			
		}
		
		// **
		
		
		//4. Вопросительные и относительные местоимения кто и что и отрицательные местоимения никто, ничто
		
		// кто 
		
		if (ruSlovo=="кто")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "кто";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "кого";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "кому";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "кого";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "кем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "ком";     break; 				
			}   
		}
		
		// **
		
		// что
		
		if (ruSlovo=="что")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "что";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "чего";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "чему";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "что";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "чем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "чём";     break; 				
			}   
		}
		
		// **
		
		// никто
		
		if (ruSlovo=="никто")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "никто";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "никого";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "никому";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "никого";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "никем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "ни о ком";     break; 				
			}   
		}
		
		// **
		
		// ничто
		
		if (ruSlovo=="ничто")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "ничто";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "ничего";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "ничему";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "никого";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "ничем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "ни о чём";     break; 				
			}   
		}
		
		// **
		
		// 5. Отрицательные местоимения некого, нечего не имеют форм именительного падежа
		
		// некого
		
		if (ruSlovo=="некого")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "не у кого";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "некому";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "некого";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "некем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "не о ком";     break; 				
			}   
		}
		
		// **
		
		// нечего
		
		if (ruSlovo=="нечего")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "не у чего";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "нечему";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "некого";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "нечем";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "не о чем";     break; 				
			}   
		}
		
		// **
		
		// Неопределённое местоимение некий 
		
		// некий
		
		if (ruSlovo=="некий")
		{
		
			switch(inSlovo.ruSlovo.ruChislo)
			{
			case RuChislo.Edinstvennoe:
				switch(inSlovo.ruSlovo.ruRod)  
				{   case RuRod.Muzhskoj:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "некий";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "некоего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "некоему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "некий";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "некоим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "некоем";     break; 				
						}   break; 
					case RuRod.Srednij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "некое";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "некоего";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "некоему";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "некое";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "некоим";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "некоем";     break; 					
						}   break;  
					case RuRod.Zhenskij:    
						switch(inSlovo.ruSlovo.ruPadezh)    
						{     
							case RuPadezh.Imenitelniy:      ruSlovo = "некая";     break;     
							case RuPadezh.Roditelniy:      ruSlovo = "некоей";     break; 				
							case RuPadezh.Datelniy:      ruSlovo = "некоей";     break; 				
							case RuPadezh.Vinitelniy:      ruSlovo = "некую";     break; 				
							case RuPadezh.Tvoritelniy:      ruSlovo = "некоей";     break; 				
							case RuPadezh.Predlozhniy:      ruSlovo = "некоей";     break; 				
						}   break;  
						
				} 
			break;  
			
			case RuChislo.Mnozhestvennoe:	
				switch(inSlovo.ruSlovo.ruPadezh)    
				{     
					case RuPadezh.Imenitelniy:      ruSlovo = "некие";     break;     
					case RuPadezh.Roditelniy:      ruSlovo = "некоих";     break; 				
					case RuPadezh.Datelniy:      ruSlovo = "некоим";     break; 				
					case RuPadezh.Vinitelniy:      ruSlovo = "некие";     break; 				
					case RuPadezh.Tvoritelniy:      ruSlovo = "некоими";     break; 				
					case RuPadezh.Predlozhniy:      ruSlovo = "некоих";     break; 				
				}   
			break; 
		
			}	
			
		}
		
		// **
		
		// 8. Местоимения таков, каков, некто, нечто не склоняются.
		
		if (ruSlovo=="один")
		{
			switch(inSlovo.ruSlovo.ruRod)  
			{   case RuRod.Muzhskoj:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "один";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "одного";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "одному";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "один";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "одним";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "одном";     break; 				
					}   break; 
				case RuRod.Srednij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "одно";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "одного";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "одному";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "одно";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "одним";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "одном";     break; 				
					}   break;  
				case RuRod.Zhenskij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "одна";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "одной";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "одной";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "одну";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "одной";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "одной";     break; 				
					}   break;  
						
			} 
		}
			
		//**
		
		// Два 
		
		if (ruSlovo=="два")
		{
			switch(inSlovo.ruSlovo.ruRod)  
			{   case RuRod.Muzhskoj:   
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "два";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "две";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "двум";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "два";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "двумя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "две";     break; 				
					}   break; 
				case RuRod.Srednij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "два";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "две";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "двум";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "две";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "двумя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "две";     break; 					
					}   break;  
				case RuRod.Zhenskij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "две";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "две";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "двум";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "две";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "двумя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "две";     break; 						
					}   break;  
						
			} 
		}
		
		
		//	**	
		
		
		// Три
		
		
		if (ruSlovo=="три")
		{
			switch(inSlovo.ruSlovo.ruRod)  
			{   case RuRod.Muzhskoj:
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "три";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "трёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "трём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "три";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "тремя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "трёх";     break; 				
					}   break; 
				case RuRod.Srednij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "три";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "трёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "трём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "трёх";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "тремя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "трёх";     break; 						
					}   break;  
				case RuRod.Zhenskij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "три";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "трёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "трём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "трёх";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "тремя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "трёх";     break; 							
					}   break;  
						
			} 
		}
		
		// **
		
		// Четыре
		
		if (ruSlovo=="четыре")
		{
			switch(inSlovo.ruSlovo.ruRod)  
			{   case RuRod.Muzhskoj:
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "четыре";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "четырёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "четырём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "четыре";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "четырьмя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "четырёх";     break; 				
					}   break; 
				case RuRod.Srednij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "четыре";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "четырёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "четырём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "четырёх";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "четырьмя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "четырёх";     break; 							
					}   break;  
				case RuRod.Zhenskij:    
					switch(inSlovo.ruSlovo.ruPadezh)    
					{     
						case RuPadezh.Imenitelniy:      ruSlovo = "четыре";     break;     
						case RuPadezh.Roditelniy:      ruSlovo = "четырёх";     break; 				
						case RuPadezh.Datelniy:      ruSlovo = "четырём";     break; 				
						case RuPadezh.Vinitelniy:      ruSlovo = "четырёх";     break; 				
						case RuPadezh.Tvoritelniy:      ruSlovo = "четырьмя";     break; 				
						case RuPadezh.Predlozhniy:      ruSlovo = "четырёх";     break; 							
					}   break;  
						
			} 
		}
		
		// **
		
		
		// Пять
		
		if (ruSlovo=="пять")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "пять";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "пяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "пяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "пять";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "пятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "пяти";     break; 				
			}   
		}
		
		// **
		
		// Шесть
		
		if (ruSlovo=="шесть")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "шесть";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "шести";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "шести";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "шесть";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "шестью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "шести";     break; 				
			}   
		}
		
		// **
		
		// семь
		
		if (ruSlovo=="семь")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "семь";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "семи";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "семи";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "семь";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "семью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "семи";     break; 				
			}   
		}
		
		// **
		
		// восемь
		
		if (ruSlovo=="восемь")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "восемь";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "восеми";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "восеми";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "восемь";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "восемью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "восеми";     break; 				
			}   
		}
		
		// **
		
		// девять
		
		if (ruSlovo=="девять")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "девять";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "девяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "девяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "девять";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "девятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "девяти";     break; 				
			}   
		}
		
		// **
		
		// десять
		
		if (ruSlovo=="десять")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "десять";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "десяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "десяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "десять";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "десятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "десяти";     break; 				
			}   
		}
		
		// **
		
		// одиннадцать
		
		if (ruSlovo=="одиннадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "одиннадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "одиннадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "одиннадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "одиннадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "одиннадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "одиннадцати";     break; 				
			}   
		}
		
		// **
		
		// двенадцать
		
		if (ruSlovo=="двенадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "двенадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "двенадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "двенадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "двенадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "двенадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "двенадцати";     break; 				
			}   
		}
		
		// **
		
		// тринадцать
		
		if (ruSlovo=="тринадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "тринадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "тринадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "тринадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "тринадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "тринадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "тринадцати";     break; 				
			}   
		}
		
		// **
		
		// четырнадцать
		
		if (ruSlovo=="четырнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "четырнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "четырнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "четырнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "четырнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "четырнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "четырнадцати";     break; 				
			}   
		}
		
		// **
		
		// пятнадцать
		
		if (ruSlovo=="пятнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "пятнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "пятнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "пятнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "пятнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "пятнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "пятнадцати";     break; 				
			}   
		}
		
		// **
		
		// шестнадцать
		
		if (ruSlovo=="шестнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "шестнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "шестнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "шестнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "шестнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "шестнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "шестнадцати";     break; 				
			}   
		}
		
		// **
		
		// семнадцать
		
		if (ruSlovo=="семнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "семнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "семнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "семнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "семнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "семнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "семнадцати";     break; 				
			}   
		}
		
		// **
		
		// восемнадцать
		
		if (ruSlovo=="восемнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "восемнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "восемнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "восемнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "восемнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "восемнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "восемнадцати";     break; 				
			}   
		}
		
		// **
		
		// девятнадцать
		
		if (ruSlovo=="девятнадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "девятнадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "девятнадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "девятнадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "девятнадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "девятнадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "девятнадцати";     break; 				
			}   
		}
		
		// **
		
		// двадцать
		
		if (ruSlovo=="двадцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "двадцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "двадцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "двадцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "двадцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "двадцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "двадцати";     break; 				
			}   
		}
		
		// **
		
		// тридцать
		
		if (ruSlovo=="тридцать")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "тридцать";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "тридцати";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "тридцати";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "тридцать";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "тридцатью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "тридцати";     break; 				
			}   
		}
		
		// **
		
		// сорок
		
		if (ruSlovo=="сорок")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "сорок";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "сорока";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "сорока";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "сорок";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "сорока";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "сорока";     break; 				
			}   
		}
		
		// **
		
		// пятьдесят
		
		if (ruSlovo=="пятьдесят")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "пятьдесят";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "пятидесяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "пятидесяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "пятьдесят";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "пятьюдесятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "пятидесяти";     break; 				
			}   
		}
		
		// **
		
		// шестьдесят
		
		if (ruSlovo=="шестьдесят")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "шестьдесят";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "шестидесяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "шестидесяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "шестьдесят";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "шестьюдесятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "шестидесяти";     break; 				
			}   
		}
		
		// **
		
		// семьдесят
		
		if (ruSlovo=="семьдесят")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "семьдесят";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "семидесяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "семидесяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "семьдесят";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "семьюдесятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "семидесяти";     break; 				
			}   
		}
		
		// **
		
		// восемьдесят
		
		if (ruSlovo=="восемьдесят")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "восемьдесят";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "восемидесяти";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "восемидесяти";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "восемьдесят";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "восемьюдесятью";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "восемидесяти";     break; 				
			}   
		}
		
		// **
		
		// девяносто
		
		if (ruSlovo=="девяносто")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "девяносто";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "девяноста";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "девяноста";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "девяносто";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "девяноста";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "девяноста";     break; 				
			}   
		}
		
		// **
		
		// сто
		
		if (ruSlovo=="сто")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "сто";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "ста";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "ста";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "сто";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "ста";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "ста";     break; 				
			}   
		}
		
		// **
		
		// двести
		
		if (ruSlovo=="двести")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "двести";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "двесот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "двумстам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "двести";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "двумястами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "двестах";     break; 				
			}   
		}
		
		// **
		
		// триста
		
		if (ruSlovo=="триста")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "триста";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "трёхсот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "трёмстам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "триста";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "трёмстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "трёхстах";     break; 				
			}   
		}
		
		// **
		
		// четыреста
		
		if (ruSlovo=="четыреста")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "четыреста";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "четырёхсот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "четырёмстам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "четыреста";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "четырёмстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "четырёхстах";     break; 				
			}   
		}
		
		// **
		
		// пятьсот
		
		if (ruSlovo=="пятьсот")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "пятьсот";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "пятисот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "пятистам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "пятьсот";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "пятьюстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "пятистах";     break; 				
			}   
		}
		
		// **
		
		// шестьсот
		
		if (ruSlovo=="шестьсот")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "шестьсот";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "шестисот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "шестистам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "шестьсот";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "шестьюстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "шестистах";     break; 				
			}   
		}
		
		// **
		
		// семьсот
		
		if (ruSlovo=="семьсот")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "семьсот";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "семисот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "семистам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "семьсот";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "семьюстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "семистах";     break; 				
			}   
		}
		
		// **
		
		// восемьсот
		
		if (ruSlovo=="восемьсот")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "восемьсот";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "восемисот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "восемистам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "восемьсот";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "восемьюстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "восемистах";     break; 				
			}   
		}
		
		// **
		
		// девятьсот
		
		if (ruSlovo=="девятьсот")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "девятьсот";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "девятисот";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "девятистам";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "девятьсот";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "девятьюстами";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "девятистах";     break; 				
			}   
		}
		
		// **
		
		// тясяча
		
		if (ruSlovo=="тысяча")
		{
			switch(inSlovo.ruSlovo.ruPadezh)    
			{     
				case RuPadezh.Imenitelniy:      ruSlovo = "тысяча";     break;     
				case RuPadezh.Roditelniy:      ruSlovo = "тысячи";     break; 				
				case RuPadezh.Datelniy:      ruSlovo = "тысяче";     break; 				
				case RuPadezh.Vinitelniy:      ruSlovo = "тысячу";     break; 				
				case RuPadezh.Tvoritelniy:      ruSlovo = "тысячей";     break; 				
				case RuPadezh.Predlozhniy:      ruSlovo = "тысяче";     break; 				
			}   
		}

		
		return ruSlovo;
		}
	}
}
