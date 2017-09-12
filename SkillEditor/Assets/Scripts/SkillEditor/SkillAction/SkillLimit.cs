using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillLimitType{
	NONE,
	MOVE,
	ATTACK
}

[ExecuteInEditMode]
public class SkillLimit : SkillAction {
	public SkillPlayAction syncAction;
	public SkillLimitType limitType = SkillLimitType.MOVE; 
	 

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		base.OnStart (caller, skill, time);
		skill.AddLimit (this);
	}

	protected override void OnClean (SkillCaller caller, Skill skill)
	{
		base.OnClean (caller, skill);
		skill.RemoveLimit (this);
	}

	protected override void OnEditorUpdate ()
	{
		if (syncAction != null && syncAction.clipInfo!=null) {
			startFrame = syncAction.startFrame;
			duration = Mathf.RoundToInt(syncAction.clipInfo.length * 1000);
		}
	}
	public override string displayName {
		get { 
			return "Limit-" + limitType + (syncAction!=null?"-"+syncAction.actionName:"");
		}
	}

	public override Example.ContentValue[] arguments{
		get { 
			Example.ContentValue[] args = new Example.ContentValue[1];
			args [0].IntValue = (int)limitType;
			return args;
		}
	}
}
