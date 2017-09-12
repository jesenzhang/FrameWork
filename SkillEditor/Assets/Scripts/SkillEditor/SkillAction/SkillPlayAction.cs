﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillPlayAction : SkillAction {

	public AnimationClip clipInfo;

	public int crossFadeTime = 0;

	public string actionName;  	 

	 protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		Debug.LogFormat ("{0} {1} Play {2}",SkillSimulator.playingTime,caller.name,actionName);
		caller.PlayAction(actionName,crossFadeTime);
	}
	  
	protected override void OnClean (SkillCaller caller, Skill skill)
	{
		base.OnClean (caller, skill);
		caller.PlayAction ("Stand",300);
	}

	public override int GetEndTime (SkillCaller caller, Skill skill)
	{
		if (clipInfo == null) {
			var clips = caller.animator.runtimeAnimatorController.animationClips;
			foreach (var clip in clips) {
				if (clip.name == actionName) {					
					clipInfo = clip;
					break;
				}
			}
		}

		duration = Mathf.RoundToInt(clipInfo.length * 1000);

		return startFrame + duration;
	} 


	public override string displayName{
		get { 
			return "PlayAction-"+actionName;
		}
	}

	public override Example.ContentValue[] arguments{
		get { 
			Example.ContentValue[] args = new Example.ContentValue[2];
			args [0].StrValue = actionName;
			args [1].IntValue = crossFadeTime;
			return args;
		}
	}
}
