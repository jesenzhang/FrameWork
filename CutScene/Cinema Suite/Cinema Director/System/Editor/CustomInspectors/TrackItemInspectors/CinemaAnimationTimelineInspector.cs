using UnityEditor;
using UnityEngine;
using CinemaDirector;
using System.Collections.Generic;

[CustomEditor(typeof(PlayAnimatorTimeline))]
public class CinemaAnimationTimelineInspector : Editor {
    // Properties
    private SerializedObject playanimation;

    private SerializedProperty firetime;
    private SerializedProperty duration;
    private SerializedProperty StateName;
    private SerializedProperty Layer;
    private const string ERROR_MSG = "CinemaAnimation requires an Animator component with an assigned RuntimeAnimatorController.";
    public void OnEnable()
    {
        
    }
 
    public override void OnInspectorGUI()
    {
        playanimation = new SerializedObject(this.target);
        this.firetime = playanimation.FindProperty("firetime");
        this.duration = playanimation.FindProperty("duration");
        this.StateName = playanimation.FindProperty("StateName");
        this.Layer = playanimation.FindProperty("Layer");

        PlayAnimatorTimeline animation = (target as PlayAnimatorTimeline);

        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);
        playanimation.ApplyModifiedProperties();

        CharacterTrackGroup ctg = animation.transform.parent.parent.gameObject.GetComponent<CharacterTrackGroup>();
        ActorTrackGroup atg = animation.transform.parent.parent.gameObject.GetComponent<ActorTrackGroup>();
        if (ctg == null && atg==null)
        {
            EditorGUILayout.HelpBox("Has No CharacterTrackGroup ", MessageType.Error);
            return;
        }
        Animator amr = null;
        if (ctg != null)
        {
            amr = ctg.Actor.GetComponentInChildren<Animator>();
        }
        else if (atg != null)
        {
            amr = atg.Actor.GetComponentInChildren<Animator>();
        }

        if (amr == null)
        {
            EditorGUILayout.HelpBox("Character Has No Animator ", MessageType.Error);
            return;
        }
        amr.Rebind();
        RuntimeAnimatorController rt = amr.runtimeAnimatorController;
        if (rt == null)
        {
            EditorGUILayout.HelpBox(ERROR_MSG, MessageType.Error);
            return;
        }
        List<string> layers = MecanimAnimationUtility.GetAllLayerNamesWithAnimator(amr);
        
        var newlayer = EditorGUILayout.Popup("Layers", Layer.intValue, layers.ToArray());
        if (newlayer != Layer.intValue)
        { 
            Layer.intValue = newlayer;
        }

        //List<string> availableStateNames = MecanimAnimationUtility.GetAllStateNamesWithController(rt);
        List<string> availableStateNames = MecanimAnimationUtility.GetAllStateNamesWithControllerInLayer(rt as UnityEditor.Animations.AnimatorController, Layer.intValue);
        int existingState = availableStateNames.IndexOf(StateName.stringValue);
        var newState = EditorGUILayout.Popup("State", existingState, availableStateNames.ToArray());

        if (newState != existingState)
        {
            existingState = newState;
            StateName.stringValue = availableStateNames[newState];
        }
        EditorGUILayout.PropertyField(StateName);
        /* this.duration.floatValue = MecanimAnimationUtility.GetStateDurationWithAnimatorController(StateName.stringValue, rt);
         if (this.duration.floatValue <= 0)
         {
             this.duration.floatValue = 1;
         }*/


        playanimation.ApplyModifiedProperties();
    }
}
