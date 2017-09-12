using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager{
	
	public static SkillCaller GetSkillCaller(Animator animator){
		SkillCaller caller = null;
		var go = GameObject.Find (animator.name+"-Skills");
		if (go == null) {
			go = new GameObject (animator.name + "-Skills");
		}
		caller = go.GetComponent<SkillCaller> ();
		if (caller == null) {
			caller = go.AddComponent<SkillCaller> ();
		}
		caller.animator = animator;
		return caller;
	}

	public static Skill CreateSkill(SkillCaller caller,string skillID,Example.Skill.SkillType skillType,params Example.Skill.TargetType[] skillTargets){ 
		var go = CreateGameObject (caller.transform,"Skill-" + skillID); 
		CreateGameObject (go.transform, "Events");
		CreateGameObject (go.transform, "Particles");
		CreateGameObject (go.transform, "Effects");

		var skill = go.AddComponent<Skill> ();

		skill.skillID = skillID;
		skill.skillType = skillType;
		skill.targetTypes = skillTargets;

		return skill;
	}

	public static SkillPlayEffect CreateSkillEffect(Skill skill,GameObject effectPrefab,int startFrame,int duration){
		if (effectPrefab == null)
			return null;

		var root = GetSkillEffectRoot (skill); 
		var go = InstantiateGameObject (root,effectPrefab,effectPrefab.name); 

		var skillEffect = go.AddComponent<SkillPlayEffect> ();

		skillEffect.startFrame = startFrame;
		skillEffect.duration = duration;
		skillEffect.effectName = effectPrefab.name;

		return skillEffect;
	}

	public static SkillParticle CreateSkillParticle(Skill skill,GameObject effectPrefab,int startFrame,int duration,Example.SkillPath.PathType pathType,Example.SkillShape.ShapeType shapeType){ 
		var root = GetSkillParticleRoot (skill); 

		var go = CreateGameObject (root,"Particle"); 

		var path = CreateSkillPath (go,pathType);
		var shape = CreateSkillShape(go,shapeType);

		var skillParticle = go.AddComponent<SkillParticle> ();

		var effect = InstantiateGameObject (go.transform,effectPrefab,"Effect"); 

		skillParticle.path = path;
		skillParticle.hitShape = shape;
		skillParticle.startFrame = startFrame;
		skillParticle.duration = duration; 
		skillParticle.effect = effectPrefab;
		skillParticle.effectName = effectPrefab!=null?effectPrefab.name:"";

		return skillParticle;
	}

	public static T CreateSkillAction<T>(Skill skill,int startFrame,int duration) where T:SkillAction{
		var root = GetSkillEventRoot (skill);
		var go = CreateGameObject (root,"Action");
		var skillEvent =  go.AddComponent<T> (); 
		skillEvent.startFrame = startFrame;
		skillEvent.duration = duration;
		return skillEvent;
	}

	public static SkillPlayAction CreatePlayAction(Skill skill,string actionName,int startFrame){
		var root = GetSkillEventRoot (skill);
		var go = CreateGameObject (root,"Action-" + actionName); 

		var skillEvent = go.AddComponent<SkillPlayAction> ();

		skillEvent.actionName = actionName;
		skillEvent.startFrame = startFrame; 

		return skillEvent;
	}
	 

	public static SkillHit CreateSkillHit(SkillParticle skillParticle,params string[] buffIDs){
		var root = skillParticle.transform;
		var go = CreateGameObject (root,"Actions"); 

		var skillEvent = go.AddComponent<SkillHit> ();

		skillEvent.particle = skillParticle;
		skillEvent.buffIDs = buffIDs; 

		return skillEvent;
	}

	public static SkillAddBuff CreateBuffEvent(Skill skill,string buffID,int startFrame){
		var root = GetSkillEventRoot (skill);
		var go = CreateGameObject (root,"Buff-" + buffID); 

		var skillEvent = go.AddComponent<SkillAddBuff> (); 

		skillEvent.buffID = buffID;
		skillEvent.startFrame = startFrame; 

		return skillEvent;
	}

	public static Transform GetSkillEventRoot(Skill skill){
		var root = skill.transform.Find ("Actions");
		if (root == null) {
			root = CreateGameObject(skill.transform,"Actions").transform; 
		}
		return root;
	}

	static Transform GetSkillParticleRoot(Skill skill){
		var root = skill.transform.Find ("Particles");
		if (root == null) {
			root = CreateGameObject(skill.transform,"Particles").transform; 
		}
		return root;
	}

	static Transform GetSkillEffectRoot(Skill skill){
		var root = skill.transform.Find ("Actions");
		if (root == null) {
			root = CreateGameObject(skill.transform,"Actions").transform; 
		}
		return root;
	}

	static SkillPath CreateSkillPath(GameObject go,Example.SkillPath.PathType type) {
		SkillPath path = null;
		switch (type) {
		case Example.SkillPath.PathType.NONE:
			path = go.AddComponent<NOPSkillPath> ();
			break;
		case Example.SkillPath.PathType.LINE:
			var linePath = go.AddComponent<LineSkillPath> (); 
			linePath.startPos = CreateGameObject (go.transform, "Start").transform;
			linePath.endPos = CreateGameObject (go.transform, "End").transform;
			path = linePath;
			break;
		case Example.SkillPath.PathType.FOLLOW:
			var followPath = go.AddComponent<FollowSkillPath> (); 
			path = followPath;
			break;
		case Example.SkillPath.PathType.HELIX:
			var helixPath = go.AddComponent<HelixSkillPath> (); 
			path = helixPath;
			break;
		}
		path.pathType = type;
		return path;
	}

	static SkillShape CreateSkillShape(GameObject go,Example.SkillShape.ShapeType type){
		SkillShape shape = null;
		switch (type) {
		case Example.SkillShape.ShapeType.BOX:
			shape = go.AddComponent<BoxSkillShape> ();
			break;
		case Example.SkillShape.ShapeType.CIRCLE:
			var circle = go.AddComponent<CircleSkillShape> (); 
			shape = circle;
			break; 
		}
		shape.shapeType = type;
		return shape;
	}

	public static GameObject CreateGameObject(Transform parent,string name){
		var go = new GameObject (name); 
		go.transform.parent = parent;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
		return go;
	}

	public static GameObject InstantiateGameObject(Transform parent,GameObject prefab,string name){
		var go = GameObject.Instantiate (prefab);
		go.name = name;
		go.transform.parent = parent;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
		return go;
	}
}
