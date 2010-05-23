/*
 * Created by SharpDevelop.
 * User: dao
 * Date: 19.12.2008
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using LEMMATIZERLib;
using AGRAMTABLib;

namespace ETRTranslator
{
	/// <summary>
	/// Description of GrafMat.
	/// </summary>
	public class GrafMat
	{
		public LemmatizerRussianClass RusLemmatizer;
     	public IParadigmCollection ParadigmCollection;
     	public IParadigm Paradigm;
     	public IGramTab RusGramTab;
		string OneAncode;
		string SrcAncodes;
		
		public GrafMat()
		{
		}
		
		public bool GrafmatAnaliseFull(string inTextForAnalise)
		{
			RusLemmatizer = new LemmatizerRussianClass();
			RusLemmatizer.LoadDictionariesRegistry();
			
			return true;
		}
	}
}
