// DiaQ. Dialogue and Quest Engine for Unity
// www.plyoung.com
// Copyright (c) 2013 by Leslie Young
// ====================================================================================================================

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DiaQ
{
	[System.Serializable]
	public class DiaQDecisionTree
	{
		public List<DiaQDecisionTest> tests = new List<DiaQDecisionTest>(0);
		
		public DiaQDecisionTree Copy()
		{
			DiaQDecisionTree d = new DiaQDecisionTree();
			d.tests = new List<DiaQDecisionTest>(0);
			foreach (DiaQDecisionTest t in this.tests) d.tests.Add(t.Copy());
			return d;
		}
		
		public int Evaluate(DiaQGraph graph)
		{
			int i = 0;
			for( ; i < tests.Count; ++i )
			{
				if( tests[i].Evaluate(graph) )
					return i;
			}

			return i;
		}
		
		// ============================================================================================================
	}
}