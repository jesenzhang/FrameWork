using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoxSkillShape : SkillShape { 


	public override bool Intersect (SkillShape other)
	{
		return base.Intersect (other);
	}

	public override bool Intersect (Vector3 point)
	{
		return base.Intersect (point);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red; 
		Gizmos.DrawWireCube (transform.position,new Vector3(width * scale,1,height * scale));
	}
	 
}
