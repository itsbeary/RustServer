using System;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200056C RID: 1388
[CreateAssetMenu(menuName = "Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x0010483D File Offset: 0x00102A3D
	protected ConsoleSystem.Command Command
	{
		get
		{
			if (this._command == null)
			{
				this._command = ConsoleSystem.Index.Server.Find(this.PopulationConvar);
				Assert.IsNotNull<ConsoleSystem.Command>(this._command, string.Format("{0} has missing convar {1}", this, this.PopulationConvar));
			}
			return this._command;
		}
	}

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x0010487A File Offset: 0x00102A7A
	public override float TargetDensity
	{
		get
		{
			return this.Command.AsFloat;
		}
	}

	// Token: 0x040022E6 RID: 8934
	[Header("Convars")]
	public string PopulationConvar;

	// Token: 0x040022E7 RID: 8935
	private ConsoleSystem.Command _command;
}
