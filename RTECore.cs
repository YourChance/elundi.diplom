/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 05.04.2009
 * Time: 21:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	public class CoreTickEventArgs : EventArgs
	{
		public double Tick;
		public CoreTickEventArgs(double t)
		{
			Tick = t;
		}
	}
	
	public class CoreEndEventArgs : EventArgs
	{
		public string Text;
		public CoreEndEventArgs(string t)
		{
			Text = t;
		}
	}
	
	public delegate void CoreTickEventHandler(object sender, CoreTickEventArgs ea);
	public delegate void CoreEndEventHandler(object sender, CoreEndEventArgs ea);
	
	/// <summary>
	/// Description of RTECore.
	/// </summary>
	public class RTECore
	{
		public RTECore()
		{
			NounModule = new RTENoun();
			VerbModule = new RTEVerb();
			DefaultModule = new RTEDefault();
			PredlogModule = new RTEPredlog();
			//AdjectiveModule = new ETRAdjective();
		}
		
		public event CoreTickEventHandler Tick;
		public event CoreEndEventHandler End;
		public event CoreEndEventHandler Mess;
		
		IModule NounModule;
		IModule DefaultModule;
		IModule VerbModule;
		IModule PredlogModule;
		//etc...
		
		double Count = 0;
		double Scet = 0;
		
		public void OnTick()
		{
			if(this.Tick != null)
				Tick(this,new CoreTickEventArgs(Scet/Count));
		}
		
		public void MorfTickEventHandler(object o, EventArgs ea)
		{
			Scet ++;
			OnTick();
		}
		
		public string Translate(string inText)
		{
			Grafemat grafemat = new Grafemat();
			
			Mess(this,new CoreEndEventArgs("GrafematStart"));
			ArrayList prText = grafemat.AnalyzeText(inText);
			Mess(this,new CoreEndEventArgs("GrafematEnd"));
			
			Count = 0;
			Scet = 0;
			
			//Считаем слова (для тиков)
			foreach(Predlozhenie curPred in prText)
				for(int i=0;i<curPred.Count;i++)
					Count++;
			
			Count *=2;
						
			MorfologRus morfolog = new MorfologRus();
			
			morfolog.Tick += MorfTickEventHandler;
			
			for(int i=0;i<prText.Count;i++)
			{
				Mess(this,new CoreEndEventArgs("MorfAnalyze"));
				prText[i] = morfolog.MorfAnalyze((Predlozhenie)prText[i]);
				Mess(this,new CoreEndEventArgs("MorfAnalyzeEnd"));
			}
			
			for(int i=0;i<prText.Count;i++)
			{
				Predlozhenie curPred = (Predlozhenie)prText[i];
				Mess(this,new CoreEndEventArgs("DictOpen"));
				curPred.Dict.Open();
				Mess(this,new CoreEndEventArgs("DictOpened"));
				
				//
				//	Второй этап - перевод
				//
				
				for(int j=0;j<curPred.Count;j++)
				{
					Slovo curSlovo = curPred[j];
					
					Mess(this,new CoreEndEventArgs(curSlovo.eSlovo));
					
					switch(curSlovo.chastRechi)
					{
						case ChastRechi.Prichastie:
							curSlovo = VerbModule.Translate(curPred,j);
							curSlovo.eSlovo = "I"+curSlovo.eSlovo.Substring(1);
							break;
						case ChastRechi.Suschestvitelnoe:
							//System.Windows.Forms.MessageBox.Show(curSlovo.eSlovo);
							curSlovo = NounModule.Translate(curPred,j);
							break;
						case ChastRechi.Glagol:
							//System.Windows.Forms.MessageBox.Show(curSlovo.eSlovo);
							curSlovo = VerbModule.Translate(curPred,j);
							break;
						case ChastRechi.Predlog:
							curSlovo = PredlogModule.Translate(curPred,j);
							break;
						default:
							curSlovo = DefaultModule.Translate(curPred,j);
							break;
					}					
					curPred.SetSlovo(curSlovo,j);
					
					Scet++;
					OnTick();
				}
				
				curPred.Dict.Close();
				
				prText[i] = curPred;
			}
			
			string rezult = "";
			
			for(int i=0;i<prText.Count;i++)
			{
				rezult += ((Predlozhenie)prText[i]).ToEString();
			}
			
			if(this.End != null)
				End(this,new CoreEndEventArgs(rezult));
			
			return rezult;
		}
	}
}
