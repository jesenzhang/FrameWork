using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaActorClipCurve))]
public class ActorClipCurveInspector : Editor
{
    // Properties
    private SerializedObject actorClipCurve;
    private SerializedProperty editorRevert;
    private SerializedProperty runtimeRevert;

    private int componentSeletion = 0;
    private int propertySelection = 0;
    private bool isDataFolded = true;
    private bool isNewCurveFolded = true;
    private GUIContent dataContent = new GUIContent("Curves");
    private GUIContent addContent = new GUIContent("Add new curves");

    [Multiline]
    private const string ERROR_MSG = "CinemaActorClipCurve requires an \nActorTrack as a parent object \nwith an assigned actor.";
    public void OnEnable()
    {
        actorClipCurve = new SerializedObject(this.target);
        this.editorRevert = actorClipCurve.FindProperty("editorRevertMode");
        this.runtimeRevert = actorClipCurve.FindProperty("runtimeRevertMode");
    }

    public override void OnInspectorGUI()
    {
        actorClipCurve.Update();
        CinemaActorClipCurve clipCurveGameObject = (target as CinemaActorClipCurve);
        
        if (clipCurveGameObject == null || clipCurveGameObject.Actor == null)
        {
            EditorGUILayout.HelpBox(ERROR_MSG, UnityEditor.MessageType.Error);
            return;
        }

        GameObject actor = clipCurveGameObject.Actor.gameObject;

        List<KeyValuePair<string, string>> currentCurves = new List<KeyValuePair<string, string>>();

        EditorGUILayout.PropertyField(editorRevert);
        EditorGUILayout.PropertyField(runtimeRevert);

        SerializedProperty curveData = actorClipCurve.FindProperty("curveData");
        if (curveData.arraySize > 0)
        {
            isDataFolded = EditorGUILayout.Foldout(isDataFolded, dataContent);
            if (isDataFolded)
            {
                for (int i = 0; i < curveData.arraySize; i++)
                {
                    SerializedProperty member = curveData.GetArrayElementAtIndex(i);
                    SerializedProperty typeProperty = member.FindPropertyRelative("Type");
                    SerializedProperty memberProperty = member.FindPropertyRelative("PropertyName");

                    KeyValuePair<string, string> curveStrings = new KeyValuePair<string, string>(typeProperty.stringValue, memberProperty.stringValue);
                    currentCurves.Add(curveStrings);

                    Component c = actor.GetComponent(typeProperty.stringValue);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent(string.Format("{0}.{1}", typeProperty.stringValue, DirectorHelper.GetUserFriendlyName(typeProperty.stringValue, memberProperty.stringValue)), EditorGUIUtility.ObjectContent(c, c.GetType()).image));
                    if (GUILayout.Button("-", GUILayout.Width(24f)))
                    {
                        curveData.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(5);
        }

        isNewCurveFolded = EditorGUILayout.Foldout(isNewCurveFolded, addContent);

        if (isNewCurveFolded)
        {
            List<GUIContent> componentSelectionList = new List<GUIContent>();
            List<GUIContent> propertySelectionList = new List<GUIContent>();

            Component[] components = DirectorHelper.getValidComponents(actor);
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];

                if (component != null)
                    componentSelectionList.Add(new GUIContent(component.GetType().Name));
            }

            componentSeletion = EditorGUILayout.Popup(new GUIContent("Component"), componentSeletion, componentSelectionList.ToArray());
            MemberInfo[] members = DirectorHelper.getValidMembers(components[componentSeletion]);
            List<MemberInfo> newMembers = new List<MemberInfo>();
            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                if (!currentCurves.Contains(new KeyValuePair<string, string>(components[componentSeletion].GetType().Name, memberInfo.Name)))
                {
                    newMembers.Add(memberInfo);
                }
            }
            members = newMembers.ToArray();

            for (int i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];
                string name = DirectorHelper.GetUserFriendlyName(components[componentSeletion], memberInfo);
                propertySelectionList.Add(new GUIContent(name));
            }
            propertySelection = EditorGUILayout.Popup(new GUIContent("Property"), propertySelection, propertySelectionList.ToArray());

            if (GUILayout.Button("Add Curve") && members.Length > 0)
            {
                Type t = null;
                PropertyInfo property = members[propertySelection] as PropertyInfo;
                FieldInfo field = members[propertySelection] as FieldInfo;
                bool isProperty = false;
                if (property != null)
                {
                    t = property.PropertyType;
                    isProperty = true;
                }
                else if (field != null)
                {
                    t = field.FieldType;
                    isProperty = false;
                }
                clipCurveGameObject.AddClipCurveData(components[componentSeletion], members[propertySelection].Name, isProperty, t);
            }
        }



        actorClipCurve.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        CinemaActorClipCurve clipCurveGameObject = (target as CinemaActorClipCurve);
        for (int i = 0; i < clipCurveGameObject.CurveData.Count; i++)
        {
            MemberClipCurveData data = clipCurveGameObject.CurveData[i];
            if (data.Type == "Transform" && data.PropertyName == "localPosition")
            {
                HandlesUtility.DrawCurvePath(data.Curve1, data.Curve2, data.Curve3);
          
               /* for (int j = 0; j < data.Curve1.keys.Length - 1; j++)
                {
                    //AnimationUtility.SetKeyLeftTangentMode(data.Curve1, j, AnimationUtility.TangentMode.Linear);
                    Keyframe key1x = data.Curve1.keys[j];
                    Keyframe key1y = data.Curve2.keys[j];
                    Keyframe key1z = data.Curve3.keys[j];
                    Keyframe key2x = data.Curve1.keys[j + 1];
                    Keyframe key2y = data.Curve2.keys[j + 1];
                    Keyframe key2z = data.Curve3.keys[j + 1];

                    Vector3 position = new Vector3(key1x.value, key1y.value, key1z.value);
                    Vector3 inTangent1 = new Vector3(key1x.inTangent, key1y.inTangent, key1z.inTangent);
                    Vector3 outTangent1 = new Vector3(key1x.outTangent, key1y.outTangent, key1z.outTangent);

                    Vector3 position2 = new Vector3(key2x.value, key2y.value, key2z.value);
                    Vector3 inTangent2 = new Vector3(key2x.inTangent, key2y.inTangent, key2z.inTangent);
                    Vector3 outTangent2 = new Vector3(key2x.outTangent, key2y.outTangent, key2z.outTangent);

                    float handleSize = HandlesUtility.GetHandleSize(position);
                    Handles.Label(position + new Vector3(0.25f * handleSize, 0.0f * handleSize, 0.0f * handleSize), j.ToString());

                    Vector3 nPosition = HandlesUtility.PositionHandle(position, Quaternion.identity);
                    if (position != nPosition)
                    {
                        Keyframe nkey1 = new Keyframe(data.Curve1.keys[j].time, nPosition.x);
                        data.Curve1.MoveKey(j, nkey1);
                        Keyframe nkey2 = new Keyframe(data.Curve2.keys[j].time, nPosition.y);
                        data.Curve2.MoveKey(j, nkey2);
                        Keyframe nkey3 = new Keyframe(data.Curve3.keys[j].time, nPosition.z);
                        data.Curve3.MoveKey(j, nkey3);
                    }
                    if (j == data.Curve1.length - 2)
                    {
                        Vector3 nPosition2 = HandlesUtility.PositionHandle(position2, Quaternion.identity);
                        float handleSize2 = HandlesUtility.GetHandleSize(position2);
                        Handles.Label(position2 + new Vector3(0.25f * handleSize2, 0.0f * handleSize2, 0.0f * handleSize2), (j+1).ToString());

                        if (position2 != nPosition2)
                        {
                            Keyframe nkey1 = new Keyframe(data.Curve1.keys[j+1].time, nPosition2.x);
                            data.Curve1.MoveKey(j + 1, nkey1);
                            Keyframe nkey2 = new Keyframe(data.Curve2.keys[j + 1].time, nPosition2.y);
                            data.Curve2.MoveKey(j + 1, nkey2);
                            Keyframe nkey3 = new Keyframe(data.Curve3.keys[j + 1].time, nPosition2.z);
                            data.Curve3.MoveKey(j + 1, nkey3);
                        }
                    }

                    Vector3 ninTangent1 = HandlesUtility.TangentHandle(inTangent1 + position);
                    if (inTangent1 != ninTangent1)
                    {
                        Keyframe ok1= data.Curve1.keys[j];
                        Keyframe nkey1 = new Keyframe(ok1.time, ok1.value, ninTangent1.x - ok1.value, ok1.outTangent);
                        data.Curve1.MoveKey(j, nkey1);
                        Keyframe ok2 = data.Curve2.keys[j];
                        Keyframe nkey2 = new Keyframe(ok2.time, ok2.value, ninTangent1.y - ok2.value, ok2.outTangent);
                        data.Curve2.MoveKey(j, nkey2);
                        Keyframe ok3 = data.Curve3.keys[j];
                        Keyframe nkey3 = new Keyframe(ok3.time, ok3.value, ninTangent1.z - ok3.value, ok3.outTangent);
                        data.Curve3.MoveKey(j, nkey3);
                    }

                    float time1 = data.Curve1.keys[j].time;
                    float time2 = data.Curve1.keys[j +1].time;
                    int N = 20;
                    float gap = (time2 - time1) / N;
           
                    for (float t = time1; t < time2; t += gap)
                    {
                        Vector3 dposition = new Vector3(data.Curve1.Evaluate(t), data.Curve2.Evaluate(t), data.Curve3.Evaluate(t));
                        Vector3 dposition2 = new Vector3(data.Curve1.Evaluate(t + gap), data.Curve2.Evaluate(t + gap), data.Curve3.Evaluate(t + gap));
                        Handles.color = Color.red;
                        Handles.DrawLine(dposition, dposition2);
                        Handles.color = Color.white;
                    }
                    
                }*/
            }
             
          
        }
    }
}
