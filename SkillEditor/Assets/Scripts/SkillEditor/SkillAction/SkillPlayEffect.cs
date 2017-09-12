using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[ExecuteInEditMode]
public class SkillPlayEffect : SkillAction {  

	public string effectName;

	 protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		//Debug.LogFormat ("{0} SkillEffect.OnStart",SkillSimulator.playingTime);
	}

	protected override void OnStop (SkillCaller caller, Skill skill, int time)
	{
		//Debug.LogFormat ("{0} SkillEffect.OnStop",SkillSimulator.playingTime);
	}

	public override string displayName{
		get { 
			return "PlayEffect-" + effectName;
		}
	}

	public override Example.ContentValue[] arguments{
		get { 
			Example.ContentValue[] args = new Example.ContentValue[1];
			args [0].StrValue = effectName; 
			return args;
		}
	}
	 
}
