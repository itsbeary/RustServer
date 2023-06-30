using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000AA4 RID: 2724
	public sealed class PropertySheetFactory
	{
		// Token: 0x060040C9 RID: 16585 RVA: 0x0017D20F File Offset: 0x0017B40F
		public PropertySheetFactory()
		{
			this.m_Sheets = new Dictionary<Shader, PropertySheet>();
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x0017D224 File Offset: 0x0017B424
		[Obsolete("Use PropertySheet.Get(Shader) with a direct reference to the Shader instead.")]
		public PropertySheet Get(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shaderName));
			}
			return this.Get(shader);
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x0017D25C File Offset: 0x0017B45C
		public PropertySheet Get(Shader shader)
		{
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shader));
			}
			PropertySheet propertySheet;
			if (this.m_Sheets.TryGetValue(shader, out propertySheet))
			{
				return propertySheet;
			}
			string name = shader.name;
			propertySheet = new PropertySheet(new Material(shader)
			{
				name = string.Format("PostProcess - {0}", name.Substring(name.LastIndexOf('/') + 1)),
				hideFlags = HideFlags.DontSave
			});
			this.m_Sheets.Add(shader, propertySheet);
			return propertySheet;
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x0017D2E0 File Offset: 0x0017B4E0
		public void Release()
		{
			foreach (PropertySheet propertySheet in this.m_Sheets.Values)
			{
				propertySheet.Release();
			}
			this.m_Sheets.Clear();
		}

		// Token: 0x04003A16 RID: 14870
		private readonly Dictionary<Shader, PropertySheet> m_Sheets;
	}
}
