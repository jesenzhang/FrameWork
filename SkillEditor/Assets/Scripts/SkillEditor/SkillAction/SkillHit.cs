using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillHit : SkillAction { 

	public SkillParticle particle;

	public string[] buffIDs;  

	public GameObject hitEffect;

	[HideInInspector]
	public string hitEffectName;

	public int hitEffectTime = 100;
	 

	protected override void OnUpdate (SkillCaller caller, Skill skill, int time)
	{
		base.OnUpdate (caller, skill, time);
		particle.CheckHit (caller, skill, time);
	}

	public override string displayName{
		get { 
			return "Hit-" +hitEffectName+"-"+string.Join(",",buffIDs);
		}
	}

	public override Example.ContentValue[] arguments{
		get { 
			Example.ContentValue[] args = new Example.ContentValue[buffIDs.Length + 3];
			args [0].IntValue = particle.id;
			args [1].StrValue = hitEffectName;
			args [2].IntValue = hitEffectTime;
			for (int i = 0; i < buffIDs.Length; ++i) {
				args [3 + i].StrValue = buffIDs [i];
			}
			return args;
		}
	}

	protected override void OnEditorUpdate ()
	{
		if (hitEffect != null) {
			hitEffectName = hitEffect.name;
		}
	}
}
