/* Created by SharpDevelop.
 * User: dao
 * Date: 15.02.2009
 * Time: 9:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace ETRTranslator
{
	/// <summary>
	/// Description of ETRNounSklonenie.
	/// </summary>
	partial class ETRNoun
	{
		protected RuMorf OsoboeCheredovanie(string ishodnaya,RuMorf izmRuMorf,char firstIndex,RuChislo chislo,RuPadezh padezh,Odushevlennost odush)
		{
			ETRNoun.RuMorf newRuMorf =izmRuMorf;
			switch(firstIndex)
			{
				case '1':
					if(chislo == RuChislo.Mnozhestvennoe)
					{
						newRuMorf.osnova = newRuMorf.osnova.Substring(0,newRuMorf.osnova.Length-2);
						switch(padezh)
						{
							case RuPadezh.Imenitelniy:
								newRuMorf.okonchanie = "е";
								break;
							case RuPadezh.Roditelniy:
								newRuMorf.okonchanie = "";
								break;
							case RuPadezh.Vinitelniy:
								if(odush == Odushevlennost.Odushevlennoe)
									newRuMorf.okonchanie = "";
								else newRuMorf.okonchanie = "е";
								break;
						}
					}
					break;
				case '3':
					switch(chislo)
					{
						case RuChislo.Edinstvennoe:
							newRuMorf.osnova = BeglayaGlasnaya(newRuMorf.osnova,newRuMorf.okonchanie,ishodnaya, RuRod.Muzhskoj,'3',false,false);
							break;
						case RuChislo.Mnozhestvennoe:
							newRuMorf.osnova = newRuMorf.osnova.Replace("ёнок","ят");
							newRuMorf.osnova = newRuMorf.osnova.Replace("енок","ят");
							newRuMorf.osnova = newRuMorf.osnova.Replace("онок","ат");
							newRuMorf.osnova = newRuMorf.osnova.Replace("ёночек","ятк");
							newRuMorf.osnova = newRuMorf.osnova.Replace("еночек","ятк");
							newRuMorf.osnova = newRuMorf.osnova.Replace("оночек","атк");
							if(newRuMorf.osnova.IndexOf("ятк")!=-1||newRuMorf.osnova.IndexOf("атк")!=-1)
							{
								newRuMorf.okonchanie = OkonchanieSklonenia("",'3',padezh,RuRod.Zhenskij,odush,false,false, RuChislo.Mnozhestvennoe,false,false);
							}
							else
							{
								newRuMorf.okonchanie = OkonchanieSklonenia("",'3',padezh,RuRod.Srednij,odush,false,false, RuChislo.Mnozhestvennoe,false,false);
							}
							break;
					}
					break;
				case '8':
					switch(chislo)
					{
						case RuChislo.Edinstvennoe:
							switch(padezh)
							{
								case RuPadezh.Imenitelniy:
								case RuPadezh.Vinitelniy:
									newRuMorf.okonchanie = "я";
									break;
								case RuPadezh.Roditelniy:
								case RuPadezh.Datelniy:
								case RuPadezh.Predlozhniy:
									newRuMorf.okonchanie = "и";
									break;
								case RuPadezh.Tvoritelniy:
									newRuMorf.okonchanie = "ем";
									break;
							}
							break;
						case RuChislo.Mnozhestvennoe:
							switch(padezh)
							{
								case RuPadezh.Imenitelniy:
								case RuPadezh.Vinitelniy:
									newRuMorf.okonchanie = "а";
									break;
								case RuPadezh.Roditelniy:
									newRuMorf.okonchanie = "";
									break;
								case RuPadezh.Datelniy:
									newRuMorf.okonchanie = "ам";
									break;
								case RuPadezh.Predlozhniy:
									newRuMorf.okonchanie = "ах";
									break;
								case RuPadezh.Tvoritelniy:
									newRuMorf.okonchanie = "ами";
									break;
							}
							break;
					}
					if(!(chislo == RuChislo.Edinstvennoe && (padezh == RuPadezh.Imenitelniy || padezh == RuPadezh.Vinitelniy)))
						newRuMorf.osnova = newRuMorf.osnova.Insert(newRuMorf.osnova.Length,"ен");
					break;
			}
			return newRuMorf;
		}
		
		protected string BeglayaGlasnaya(string osnova,string ok,string ishodnaya,RuRod rod,char firstIndex,bool rod_padezh_mn_chisla, bool udarnoe)
		{
			string ruGlas = "ёуеаояиюэы";
			string ship = "шжчщц";
			string kgh = "кгх";
			int mesto = osnova.Length-2;
			if(ishodnaya[ishodnaya.Length-1]=='й')
				mesto = osnova.Length-1;
						
			if(ruGlas.IndexOf(osnova[mesto])!=-1)
			{
				if(osnova+ok != ishodnaya && ok != "ью")
				{
					if(osnova[mesto] == 'о')
						osnova = osnova.Remove(mesto,1);
					else if(osnova[mesto] == 'е' || osnova[mesto] == 'ё')
					{
						
						if(ruGlas.IndexOf(osnova[mesto-1])!=-1)
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"й");
						}
						else if(rod == RuRod.Muzhskoj && firstIndex == '6')
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"ь");
						}
						else if(rod == RuRod.Muzhskoj && firstIndex == '3' && ruGlas.IndexOf(osnova[mesto-1])==-1 && ship.IndexOf(osnova[mesto-1])==-1 )
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"ь");
						}
						else if(osnova[mesto-1] == 'л')
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"ь");
						}
						else osnova = osnova.Remove(mesto,1);
					}
				}
					
			}
			else
			{
				if(rod_padezh_mn_chisla)
				{
					if(rod != RuRod.Muzhskoj && firstIndex == '6')
					{
						osnova = osnova.Remove(osnova.Length-1,1);
						if(udarnoe)
							osnova = osnova.Insert(osnova.Length,"е");
						else osnova = osnova.Insert(osnova.Length,"и");
					}
					else if(rod != RuRod.Muzhskoj && (osnova[mesto] == 'ь' || osnova[mesto] == 'й'))
					{
						if(udarnoe && osnova[mesto+1] != 'ц')
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"ё");
						}
						else
						{
							osnova = osnova.Remove(mesto,1);
							osnova = osnova.Insert(mesto,"е");
						}						
					}
					else if(kgh.IndexOf(osnova[mesto]) != -1)
					{
						osnova = osnova.Insert(mesto+1,"о");
					}
					else if(kgh.IndexOf(osnova[mesto+1]) != -1 && ship.IndexOf(osnova[mesto]) == -1)
					{
						osnova = osnova.Insert(mesto+1,"о");
					}
					else if(udarnoe)
					{
						if(ship.IndexOf(osnova[mesto]) == -1)
							osnova = osnova.Insert(mesto+1,"ё");
						else osnova = osnova.Insert(mesto+1,"о");
					}
					else osnova = osnova.Insert(mesto+1,"е");
				}
			}
			return osnova;
		}
		
		protected string OkonchanieSklonenia(string nac_ok,char firstIndex,RuPadezh padezh,RuRod rod,Odushevlennost odush,bool udarnoe,bool posl_is_ship,RuChislo chislo,bool odin_v_kruge,bool dva_v_kruge)
		{
			string ok = "";
			
			switch(chislo)
			{
				case RuChislo.Edinstvennoe:
					switch(padezh)
					{
						case RuPadezh.Imenitelniy:
							ok = nac_ok;
							break;
						case RuPadezh.Roditelniy:					
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "а";											
											break;
										case '2':
											ok = "я";											
											break;
										case '3':
											ok = "а";
											break;
										case '4':
											ok = "а";
											break;
										case '5':
											ok = "а";
											break;
										case '6':
											ok = "и";
											break;
										case '7':
											ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "ы";											
											break;
										case '2':
											ok = "и";											
											break;
										case '3':
											ok = "и";
											break;
										case '4':
											ok = "и";
											break;
										case '5':
											ok = "ы";
											break;
										case '6':
											ok = "и";
											break;
										case '7':
											ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "а";											
											break;
										case '2':
											ok = "я";											
											break;
										case '3':
											ok = "а";										
											break;
										case '4':
											ok = "а";											
											break;
										case '5':
											ok = "а";											
											break;
										case '6':
											ok = "я";											
											break;
										case '7':
											ok = "я";											
											break;
										case '8':
											ok = "и";											
											break;
									}		
									break;						
							}
							break;
						case RuPadezh.Vinitelniy:
							switch(rod)
							{
								case RuRod.Muzhskoj:
									if(odush == Odushevlennost.Odushevlennoe)
									{
										switch(firstIndex)
										{
											case '1':
												ok = "а";											
												break;
											case '2':
												ok = "я";											
												break;
											case '3':
												ok = "а";
												break;
											case '4':
												ok = "а";
												break;
											case '5':
												ok = "а";
												break;
											case '6':
												ok = "и";
												break;
											case '7':
												ok = "и";
												break;
											case '8':
												ok = "ь";
												break;
										}	
									}
									else
									{
										ok = nac_ok;
									}
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "у";											
											break;
										case '2':
											ok = "ю";											
											break;
										case '3':
											ok = "у";
											break;
										case '4':
											ok = "у";
											break;
										case '5':
											ok = "у";
											break;
										case '6':
											ok = "ю";
											break;
										case '7':
											ok = "ю";
											break;
										case '8':
											ok = "ь";
											break;
									}					
									break;					
								case RuRod.Srednij:
									ok = nac_ok;
									if(firstIndex == '8')
										ok = "ь";
									break;						
							}							
							break;
						case RuPadezh.Datelniy:
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "у";											
											break;
										case '2':
											ok = "ю";											
											break;
										case '3':
											ok = "у";
											break;
										case '4':
											ok = "у";
											break;
										case '5':
											ok = "у";
											break;
										case '6':
											ok = "ю";
											break;
										case '7':
											ok = "ю";
											break;
										case '8':
											ok = "и";
											break;
									}									
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "е";											
											break;
										case '2':
											ok = "е";											
											break;
										case '3':
											ok = "е";
											break;
										case '4':
											ok = "е";
											break;
										case '5':
											ok = "е";
											break;
										case '6':
											ok = "е";
											break;
										case '7':
											ok = "е";// ? ударение (-и в безударном)
											if(!udarnoe)
												ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "у";											
											break;
										case '2':
											ok = "ю";											
											break;
										case '3':
											ok = "у";
											break;
										case '4':
											ok = "у";
											break;
										case '5':
											ok = "у";
											break;
										case '6':
											ok = "ю";
											break;
										case '7':
											ok = "ю";
											break;
										case '8':
											ok = "и";
											break;
									}	
									break;						
							}	
							break;
						case RuPadezh.Tvoritelniy:
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "ом";											
											break;
										case '2':
											ok = "ем";											
											break;
										case '3':
											ok = "ом";											
											break;
										case '4':
											ok = "ом";
											if(!udarnoe)
												ok = "ем";
											break;
										case '5':
											ok = "ом";
											if(!udarnoe)
												ok = "ем";
											break;
										case '6':
											ok = "ем";
											break;
										case '7':
											ok = "ем";
											break;
										case '8':
											ok = "ем";
											break;
									}									
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "ой";											
											break;
										case '2':
											ok = "ей";											
											break;
										case '3':
											ok = "ой"; // ? ударение
											break;
										case '4':
											ok = "ой"; // ? ударение
											break;
										case '5':
											ok = "ой"; // ? ударение
											break;
										case '6':
											ok = "ей"; 
											break;
										case '7':
											ok = "ей";
											break;
										case '8':
											ok = "ью";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "ом";											
											break;
										case '2':
											ok = "ем";											
											break;
										case '3':
											ok = "ом";// ? ударение
											break;
										case '4':
											ok = "ом";// ? ударение
											break;
										case '5':
											ok = "ом";// ? ударение
											break;
										case '6':
											ok = "ем";
											break;
										case '7':
											ok = "ем";
											break;
										case '8':
											ok = "ем";
											break;
									}		
									break;						
							}	
							break;
						case RuPadezh.Predlozhniy:
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "е";											
											break;
										case '2':
											ok = "е";											
											break;
										case '3':
											ok = "е";
											break;
										case '4':
											ok = "е";
											break;
										case '5':
											ok = "е";
											break;
										case '6':
											ok = "е";
											break;
										case '7':
											ok = "е";// ? ударение в безуданом -и
											break;
										case '8':
											ok = "и";
											break;
									}									
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "е";											
											break;
										case '2':
											ok = "е";											
											break;
										case '3':
											ok = "е"; 
											break;
										case '4':
											ok = "е";
											break;
										case '5':
											ok = "е"; 
											break;
										case '6':
											ok = "е"; 
											break;
										case '7':
											ok = "е";// ? ударение в безударном -и
											if(!udarnoe)
												ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "е";											
											break;
										case '2':
											ok = "е";											
											break;
										case '3':
											ok = "е";
											break;
										case '4':
											ok = "е";
											break;
										case '5':
											ok = "е";
											break;
										case '6':
											ok = "е";
											break;
										case '7':
											ok = "и";// ? ударение в безударном -и
											break;
										case '8':
											ok = "и";
											break;
									}		
									break;						
							}	
							break;					
					}
					break;
				case RuChislo.Mnozhestvennoe:
					switch(padezh)
					{
						case RuPadezh.Imenitelniy:
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "ы";
											if(odin_v_kruge)
												ok = "а";
											break;
										case '2':
											ok = "и";
											if(odin_v_kruge)
												ok = "я";
											break;
										case '3':
											ok = "и";
											break;
										case '4':
											ok = "и";
											break;
										case '5':
											ok = "ы";
											break;
										case '6':
											ok = "и";
											break;
										case '7':
											ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "ы";
											if(odin_v_kruge)
												ok = "а";
											break;
										case '2':
											ok = "и";
											if(odin_v_kruge)
												ok = "я";
											break;
										case '3':
											ok = "и";
											break;
										case '4':
											ok = "и";
											break;
										case '5':
											ok = "ы";
											break;
										case '6':
											ok = "и";
											break;
										case '7':
											ok = "и";
											break;
										case '8':
											ok = "и";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "а";
											if(odin_v_kruge)
												ok = "ы";
											break;
										case '2':
											ok = "я";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '3':
											ok = "а";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '4':
											ok = "а";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '5':
											ok = "а";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '6':
											ok = "я";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '7':
											ok = "я";
											if(odin_v_kruge)
												ok = "и";
											break;
										case '8':
											ok = "я";
											if(odin_v_kruge)
												ok = "и";
											break;
									}		
									break;						
							}
							break;
						case RuPadezh.Roditelniy:					
							switch(rod)
							{
								case RuRod.Muzhskoj:
									switch(firstIndex)
									{
										case '1':
											ok = "ов";
											if(dva_v_kruge)
												ok = "";
											break;
										case '2':
											ok = "ей";
											if(dva_v_kruge)
												ok = "ь";											
											break;
										case '3':
											ok = "ов";
											if(dva_v_kruge)
												ok = "";
											break;
										case '4':
											ok = "ей";
											if(dva_v_kruge)
												ok = "";
											break;
										case '5':
											ok = "ов";
											if(dva_v_kruge)
												ok = "";
											break;
										case '6':
											ok = "ев";
											if(dva_v_kruge)
												ok = "ь";
											break;
										case '7':
											ok = "ев";
											if(dva_v_kruge)
												ok = "ь";
											break;
										case '8':
											ok = "ей";
											break;
									}					
									break;
								case RuRod.Zhenskij:
									switch(firstIndex)
									{
										case '1':
											ok = "";
											if(dva_v_kruge)
												ok = "ей";
											break;
										case '2':
											ok = "ей"; // ? ударение в безударном -ь
											if(dva_v_kruge)
												ok = "ей";
											if(!udarnoe)
												ok = "ь";
											break;
										case '3':
											ok = "";
											if(dva_v_kruge)
												ok = "ей";
											break;
										case '4':
											ok = "ей"; // ? ударение в безударном -ь
											if(dva_v_kruge)
												ok = "ей";
											if(!udarnoe)
												ok = "ь";
											break;
										case '5':
											ok = "";
											if(dva_v_kruge)
												ok = "ей";
											break;
										case '6':
											ok = "й";
											if(dva_v_kruge)
												ok = "ей";
											break;
										case '7':
											ok = "й";
											if(dva_v_kruge)
												ok = "ей";
											break;
										case '8':
											ok = "ей";
											if(dva_v_kruge)
												ok = "ей";
											break;
									}					
									break;					
								case RuRod.Srednij:
									switch(firstIndex)
									{
										case '1':
											ok = "";
											if(dva_v_kruge)
												ok = "ов";
											break;
										case '2':
											ok = "ей"; // ? ударение в безударном -ь
											if(dva_v_kruge)
												ok = "ев";
											if(!udarnoe)
												ok = "ь";
											break;
										case '3':
											ok = "";
											if(dva_v_kruge)
												ok = "ов";
											break;
										case '4':
											ok = "ей"; // ? ударение в безударном -ь
											if(dva_v_kruge)
											{
												ok = "ов"; // ? ударение
												if(!udarnoe)
													ok = "ев";
											}
											if(!udarnoe)
												ok = "ь";
											break;
										case '5':
											ok = "";
											if(dva_v_kruge)
											{
												ok = "ов"; // ? ударение
												if(!udarnoe)
													ok = "ев";
											}
											break;
										case '6':
											ok = "й";
											if(dva_v_kruge)
												ok = "ев";
											break;
										case '7':
											ok = "й";
											if(dva_v_kruge)
												ok = "ев";
											break;
										case '8':
											ok = "ей";
											if(dva_v_kruge)
												ok = "ев";
											break;
									}			
									break;						
							}
							break;
						case RuPadezh.Vinitelniy:
							if(odush == Odushevlennost.Odushevlennoe)
							{
								switch(rod)
								{
									case RuRod.Muzhskoj:
										switch(firstIndex)
										{
											case '1':
												ok = "ов";
												if(dva_v_kruge)
													ok = "";
												break;
											case '2':
												ok = "ей";
												if(dva_v_kruge)
													ok = "ь";											
												break;
											case '3':
												ok = "ов";
												if(dva_v_kruge)
													ok = "";
												break;
											case '4':
												ok = "ей";
												if(dva_v_kruge)
													ok = "";
												break;
											case '5':
												ok = "ов";
												if(dva_v_kruge)
													ok = "";
												break;
											case '6':
												ok = "ев";
												if(dva_v_kruge)
													ok = "ь";
												break;
											case '7':
												ok = "ев";
												if(dva_v_kruge)
													ok = "ь";
												break;
											case '8':
												ok = "ей";
												break;
										}					
										break;
									case RuRod.Zhenskij:
										switch(firstIndex)
										{
											case '1':
												ok = "";
												if(dva_v_kruge)
													ok = "ей";
												break;
											case '2':
												ok = "ей"; // ? ударение в безударном -ь
												if(dva_v_kruge)
													ok = "ей";
												if(!udarnoe)
													ok = "ь";
												break;
											case '3':
												ok = "";
												if(dva_v_kruge)
													ok = "ей";
												break;
											case '4':
												ok = "ей"; // ? ударение в безударном -ь
												if(dva_v_kruge)
													ok = "ей";
												if(!udarnoe)
													ok = "ь";
												break;
											case '5':
												ok = "";
												if(dva_v_kruge)
													ok = "ей";
												break;
											case '6':
												ok = "й";
												if(dva_v_kruge)
													ok = "ей";
												break;
											case '7':
												ok = "й";
												if(dva_v_kruge)
													ok = "ей";
												break;
											case '8':
												ok = "ей";
												if(dva_v_kruge)
													ok = "ей";
												break;
										}					
										break;					
									case RuRod.Srednij:
										switch(firstIndex)
										{
											case '1':
												ok = "";
												if(dva_v_kruge)
													ok = "ов";
												break;
											case '2':
												ok = "ей"; // ? ударение в безударном -ь
												if(dva_v_kruge)
													ok = "ев";
												if(!udarnoe)
													ok = "ь";
												break;
											case '3':
												ok = "";
												if(dva_v_kruge)
													ok = "ов";
												break;
											case '4':
												ok = "ей"; // ? ударение в безударном -ь
												if(dva_v_kruge)
												{
													ok = "ов"; // ? ударение
													if(!udarnoe)
														ok = "ев";
												}
												if(!udarnoe)
													ok = "ь";
												break;
											case '5':
												ok = "";
												if(dva_v_kruge)
												{
													ok = "ов"; // ? ударение
													if(!udarnoe)
														ok = "ев";
												}
												break;
											case '6':
												ok = "й";
												if(dva_v_kruge)
													ok = "ев";
												break;
											case '7':
												ok = "й";
												if(dva_v_kruge)
													ok = "ев";
												break;
											case '8':
												ok = "ей";
												if(dva_v_kruge)
													ok = "ев";
												break;
										}			
										break;
								}
							}
							else
							{
								switch(rod)
								{
									case RuRod.Muzhskoj:
										switch(firstIndex)
										{
											case '1':
												ok = "ы";
												if(odin_v_kruge)
													ok = "а";
												break;
											case '2':
												ok = "и";
												if(odin_v_kruge)
													ok = "я";
												break;
											case '3':
												ok = "и";
												break;
											case '4':
												ok = "и";
												break;
											case '5':
												ok = "ы";
												break;
											case '6':
												ok = "и";
												break;
											case '7':
												ok = "и";
												break;
											case '8':
												ok = "и";
												break;
										}					
										break;
									case RuRod.Zhenskij:
										switch(firstIndex)
										{
											case '1':
												ok = "ы";
												if(odin_v_kruge)
													ok = "а";
												break;
											case '2':
												ok = "и";
												if(odin_v_kruge)
													ok = "я";
												break;
											case '3':
												ok = "и";
												break;
											case '4':
												ok = "и";
												break;
											case '5':
												ok = "ы";
												break;
											case '6':
												ok = "и";
												break;
											case '7':
												ok = "и";
												break;
											case '8':
												ok = "и";
												break;
										}					
										break;					
									case RuRod.Srednij:
										switch(firstIndex)
										{
											case '1':
												ok = "а";
												if(odin_v_kruge)
													ok = "ы";
												break;
											case '2':
												ok = "я";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '3':
												ok = "а";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '4':
												ok = "а";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '5':
												ok = "а";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '6':
												ok = "я";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '7':
												ok = "я";
												if(odin_v_kruge)
													ok = "и";
												break;
											case '8':
												ok = "я";
												if(odin_v_kruge)
													ok = "и";
												break;
										}		
										break;						
								}
							}
							break;
						case RuPadezh.Datelniy:
							switch(firstIndex)
							{
								case '1':
									ok = "ам";
									break;
								case '2':
									ok = "ям";
									if(posl_is_ship)
										ok = "ам";
									break;
								case '3':
									ok = "ам";
									break;
								case '4':
									ok = "ам";
									break;
								case '5':
									ok = "ам";
									break;
								case '6':
									ok = "ям";
									if(posl_is_ship)
										ok = "ам";
									break;
								case '7':
									ok = "ям";
									if(posl_is_ship)
										ok = "ам";
									break;
								case '8':
									ok = "ям";
									if(posl_is_ship)
										ok = "ам";
									break;
							}	
							break;
						case RuPadezh.Tvoritelniy:
							switch(firstIndex)
							{
								case '1':
									ok = "ами";
									break;
								case '2':
									ok = "ями";
									if(posl_is_ship)
										ok = "ами";
									break;
								case '3':
									ok = "ами";
									break;
								case '4':
									ok = "ами";
									break;
								case '5':
									ok = "ами";
									break;
								case '6':
									ok = "ями";
									if(posl_is_ship)
										ok = "ами";
									break;
								case '7':
									ok = "ями";
									if(posl_is_ship)
										ok = "ами";
									break;
								case '8':
									ok = "ями";
									if(posl_is_ship)
										ok = "ами";
									break;
							}	
							break;
						case RuPadezh.Predlozhniy:
							switch(firstIndex)
							{
								case '1':
									ok = "ах";
									break;
								case '2':
									ok = "ях";	
									if(posl_is_ship)
										ok = "ах";
									break;
								case '3':
									ok = "ах";
									break;
								case '4':
									ok = "ах";
									break;
								case '5':
									ok = "ах";
									break;
								case '6':
									ok = "ях";
									if(posl_is_ship)
										ok = "ах";
									break;
								case '7':
									ok = "ях";
									if(posl_is_ship)
										ok = "ах";
									break;
								case '8':
									ok = "ях";
									if(posl_is_ship)
										ok = "ах";
									break;
							}	
							break;					
					}
					break;
			}
			
			return ok;
		}
	}
}
