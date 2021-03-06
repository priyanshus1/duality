﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Duality;
using Duality.Components;
using Duality.Components.Renderers;
using Duality.Resources;
using Duality.Drawing;
using Duality.Editor;


namespace Duality.Editor.Plugins.Base.DataConverters
{
	public class GameObjFromPrefab : DataConverter
	{
		public override Type TargetType
		{
			get { return typeof(GameObject); }
		}
		public override int Priority
		{
			get { return PrioritySpecialized; }
		}

		public override bool CanConvertFrom(ConvertOperation convert)
		{
			return 
				convert.AllowedOperations.HasFlag(ConvertOperation.Operation.CreateObj) && 
				convert.Data.ContainsContentRefs<Prefab>();
		}
		public override bool Convert(ConvertOperation convert)
		{
			if (convert.Data.ContainsContentRefs<Prefab>())
			{
				ContentRef<Prefab>[] dropdata = convert.Data.GetContentRefs<Prefab>();

				// Instantiate Prefabs
				foreach (ContentRef<Prefab> pRef in dropdata)
				{
					if (convert.IsObjectHandled(pRef.Res)) continue;
					if (!pRef.IsAvailable) continue;
					GameObject newObj = pRef.Res.Instantiate();
					if (newObj != null)
					{
						convert.AddResult(newObj);
						convert.MarkObjectHandled(pRef.Res);
					}
				}
			}
			// Don't finish convert operation - other converters miht contribute to the new GameObjects!
			return false; 
		}
	}
}
