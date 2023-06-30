using System;
using System.Collections.Generic;

// Token: 0x0200068D RID: 1677
public class PathSequencePowerline : PathSequence
{
	// Token: 0x06003015 RID: 12309 RVA: 0x00121118 File Offset: 0x0011F318
	public override void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		bool flag = false;
		if (this.Rule == PathSequencePowerline.SequenceRule.Powerline)
		{
			if (pathLength >= 3)
			{
				flag = sequence.Count == 0 || pathIndex == pathLength - 1;
				if (!flag)
				{
					flag = this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform) >= 2;
				}
			}
		}
		else if (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform)
		{
			flag = pathLength < 3;
			if (!flag)
			{
				int indexCountToRule = this.GetIndexCountToRule(sequence, PathSequencePowerline.SequenceRule.PowerlinePlatform);
				flag = indexCountToRule < 2 && indexCountToRule != sequence.Count && pathIndex < pathLength - 1;
			}
		}
		if (flag)
		{
			Prefab prefabOfType = this.GetPrefabOfType(possibleReplacements, (this.Rule == PathSequencePowerline.SequenceRule.PowerlinePlatform) ? PathSequencePowerline.SequenceRule.Powerline : PathSequencePowerline.SequenceRule.PowerlinePlatform);
			if (prefabOfType != null)
			{
				replacement = prefabOfType;
			}
		}
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x001211B0 File Offset: 0x0011F3B0
	private Prefab GetPrefabOfType(Prefab[] options, PathSequencePowerline.SequenceRule ruleToFind)
	{
		for (int i = 0; i < options.Length; i++)
		{
			PathSequencePowerline pathSequencePowerline = options[i].Attribute.Find<PathSequence>(options[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline == null || pathSequencePowerline.Rule == ruleToFind)
			{
				return options[i];
			}
		}
		return null;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x00121200 File Offset: 0x0011F400
	private int GetIndexCountToRule(List<Prefab> sequence, PathSequencePowerline.SequenceRule rule)
	{
		int num = 0;
		for (int i = sequence.Count - 1; i >= 0; i--)
		{
			PathSequencePowerline pathSequencePowerline = sequence[i].Attribute.Find<PathSequence>(sequence[i].ID) as PathSequencePowerline;
			if (pathSequencePowerline != null)
			{
				if (pathSequencePowerline.Rule == rule)
				{
					break;
				}
				num++;
			}
		}
		return num;
	}

	// Token: 0x040027AE RID: 10158
	public PathSequencePowerline.SequenceRule Rule;

	// Token: 0x040027AF RID: 10159
	private const int RegularPowerlineSpacing = 2;

	// Token: 0x02000DD5 RID: 3541
	public enum SequenceRule
	{
		// Token: 0x040049B4 RID: 18868
		PowerlinePlatform,
		// Token: 0x040049B5 RID: 18869
		Powerline
	}
}
