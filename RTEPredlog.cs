/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 05.04.2009
 * Time: 21:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace ETRTranslator
{
	/// <summary>
	/// Description of RTEDefault.
	/// </summary>
	public class RTEPredlog : IModule
	{
		public RTEPredlog()
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
			
			ArrayList al = pr.Dict.GetStrictRusEx(analyzed.eSlovo);
            if(al.Count > 0)
			{
				analyzed.eSlovo = ((DictSlovo)al[0]).El;
			}
			else
            {
				analyzed.eSlovo = pr.Dict.CreateNewSlovoEx(analyzed);
			}
			
			return analyzed;
		}	
	}
}
