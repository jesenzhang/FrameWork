using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FollowSkillPath : SkillPath {

	public Transform followTarget;

	public float speed = 1;

	protected override void OnUpdatePath (Transform target,float factor)
	{
		var dir = followTarget.position - target.position;
		dir.Normalize ();
		target.position = target.position + Time.deltaTime * speed * dir;
		if (updateRotation) {
			target.rotation = Quaternion.LookRotation (dir);
		}
	}

	public override Example.ContentValue[] arguments {
		get {
			Example.ContentValue[] args = new Example.ContentValue[1];
			args [0].FloatValue = speed;
			return args;
		}
	}
}
