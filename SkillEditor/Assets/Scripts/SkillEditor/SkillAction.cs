using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ActionState{
	IDLE,
	STARTED,
	STOPED,
	CANCELED,
}

[ExecuteInEditMode]
public abstract class SkillAction : SkillTimeTween {	 
	public abstract Example.ContentValue[] arguments {
		get;
	}
}
