using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillAddBuff : SkillAction { 
	
	public string buffID;  

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		
	}

	public override string displayName{
		get { 
			return "AddBuff-" + buffID;
		}
	}

	public override Example.ContentValue[] arguments{
		get { 
			Example.ContentValue[] args = new Example.ContentValue[1];
			args [0].StrValue = buffID;
			return args;
		}
	}

}
