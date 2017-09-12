using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HelixSkillPath : SkillPath {

	public float radius = 10;

	protected override void OnUpdatePath (Transform target,float factor)
	{

	}

	public override Example.ContentValue[] arguments {
		get {
			Example.ContentValue[] args = new Example.ContentValue[1];
			args [0].FloatValue = radius;
			return args;
		}
	}
}
