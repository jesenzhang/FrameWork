using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NOPSkillPath : SkillPath {
	
	protected override void OnUpdatePath (Transform target,float factor)
	{
		
	}

	public override Example.ContentValue[] arguments {
		get {
			Example.ContentValue[] args = new Example.ContentValue[0]; 
			return args;
		}
	}
}
