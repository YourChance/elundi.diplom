/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 14.03.2009
 * Time: 16:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of ETRCore.
	/// </summary>
	public class ETRCore
	{
		public ETRCore()
		{
			NounModule = new ETRNoun();
			AdjectiveModule = new ETRAdjective();
			VerbModule = new GlagModule();
			PredlogModule = new ETRPredlog();
			DefaultModule = new ETRDefault();
			OtherModule = new ETROther();
		}
		
		public event CoreTickEventHandler Tick;
		public event CoreEndEventHandler End;
		
		double Count = 0;
		double Scet = 0;
		
		public void OnTick()
		{
			if(this.Tick != null)
				Tick(this,new CoreTickEventArgs(Count!=0?Scet/Count:Count));
		}
		
		IModule NounModule;
		IModule AdjectiveModule;
		IModule VerbModule;
		IModule PredlogModule;
		IModule DefaultModule;
		IModule OtherModule;
		//etc...
		
		public string Translate(string inText)
		{
			Grafemat gr = new Grafemat();
			ArrayList prText = gr.AnalyzeTextEl(inText);
			
			Count = 0;
			Scet = 0;
			
			//Считаем слова (для тиков)
			foreach(Predlozhenie curPred in prText)
				for(int i=0;i<curPred.Count;i++)
					Count++;
			
			Count *=2;
			
			for(int i=0;i<prText.Count;i++)
			{
				Predlozhenie curPred = (Predlozhenie)prText[i];
				
				//
				//	Первый этап - анализ
				//
				
				//Нулевой проход - определение частей речи
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						if(eSlovo.Length == 1)
							curSlovo.chastRechi = ChastRechi.Mestoimenie;
						else
						switch(eSlovo[0])
						{
							case 'Q':
							case 'W':
								curSlovo.chastRechi = ChastRechi.Suschestvitelnoe;
								break;
							case 'F':
								curSlovo.chastRechi = ChastRechi.Predlog;
								break;
							case 'E':
								curSlovo.chastRechi = ChastRechi.Prilagatelnoe;
								break;
							case 'I':
								curSlovo.chastRechi = ChastRechi.Prichastie;
								break;
							case 'A':
								curSlovo.chastRechi = ChastRechi.Mestoimenie;
								break;
							case 'R':
							case 'T':
							case 'Y':
							case 'U':
							case 'O':
								curSlovo.chastRechi = ChastRechi.Glagol;
								break;
							//etc...
							default:
								curSlovo.chastRechi = ChastRechi.Mezhdometie;
								break;
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				// Первый проход - существительные
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						if(eSlovo[0]=='Q' || eSlovo[0]=='W')
						{
							curSlovo = NounModule.Analyze(curPred,j);	
							Scet++;
							OnTick();
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				// Полуторный проход - глаголы
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						if(eSlovo[0]=='R' || eSlovo[0]=='T' || eSlovo[0]=='Y' || eSlovo[0]=='U')
						{
							curSlovo = VerbModule.Analyze(curPred,j);	
							Scet++;
							OnTick();
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}
				
				//Второй проход - все остальное.
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					if(curSlovo.chastRechi != ChastRechi.Znak)
					{
						string eSlovo = curSlovo.eSlovo;
						
						//
						// Заменить на соответствующее для данной части речи.
						//
						if(eSlovo[0]=='E' || eSlovo[0]=='I')
						{
							curSlovo = AdjectiveModule.Analyze(curPred,j);
							Scet++;
							OnTick();
						}
						if(eSlovo[0]=='F')
						{
							curSlovo = PredlogModule.Analyze(curPred,j);
							Scet++;
							OnTick();
						}
						
						if(eSlovo[0]=='A')
						{
							curSlovo = OtherModule.Analyze(curPred,j);
							Scet++;
							OnTick();
						}
					}
					
					curPred.SetSlovo(curSlovo,j);
				}	
				
				//
				//	Второй этап - перевод
				//
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					try
					{
					
						switch(curSlovo.chastRechi)
						{
							case ChastRechi.Suschestvitelnoe:
								curSlovo = NounModule.Translate(curPred,j);
								break;
							case ChastRechi.Prilagatelnoe:
								curSlovo = AdjectiveModule.Translate(curPred,j);
								break;
							case ChastRechi.Prichastie:
								curSlovo = AdjectiveModule.Translate(curPred,j);
								break;
							case ChastRechi.Glagol:
								curSlovo = VerbModule.Translate(curPred,j);
								break;
							case ChastRechi.Predlog:
								curSlovo = PredlogModule.Translate(curPred,j);
								break;
							case ChastRechi.Mestoimenie:
								curSlovo = OtherModule.Translate(curPred,j);
								break;
							default:
								curSlovo = DefaultModule.Translate(curPred,j);
								break;
						}
					}
					catch
					{}
					
					curPred.SetSlovo(curSlovo,j);
					Scet++;
					OnTick();
				}
				
				prText[i] = curPred;
			}
			
			string rezult = "";
			
			Scet=Count;
			OnTick();
			
			for(int i=0;i<prText.Count;i++)
			{
				rezult += ((Predlozhenie)prText[i]).ToRString();
			}
			
			if(this.End != null)
				End(this,new CoreEndEventArgs(rezult));
			
			return rezult;
		}
	}
}
