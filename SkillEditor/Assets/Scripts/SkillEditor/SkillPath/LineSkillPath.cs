using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineSkillPath : SkillPath {
	
	public Transform startPos;
	public Transform endPos; 

	private Vector3 dir;
	 
	protected override void OnStartPath (Transform target)
	{
		origPos = startPos.position;
		dir = endPos.localPosition - startPos.localPosition;
	}

	protected override void OnUpdatePath (Transform target,float factor)
	{  
		target.position = origPos + dir * factor;
		if (updateRotation && dir.sqrMagnitude > 0) {
			target.rotation = Quaternion.LookRotation (dir);
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		if (Application.isPlaying) {
			Gizmos.DrawLine (origPos, origPos + dir);
		} else {
			Gizmos.DrawLine (startPos.position, endPos.position);
		}
	}

	public override Example.ContentValue[] arguments {
		get {
			Example.ContentValue[] args = new Example.ContentValue[2];
			args [0].Vector3Value = MathUtil.ToVector3f(startPos.localPosition);
			args [1].Vector3Value = MathUtil.ToVector3f(endPos.localPosition);
			return args;
		}
	}
}
