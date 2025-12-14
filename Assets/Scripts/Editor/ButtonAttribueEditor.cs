using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonAttributeEditor : Editor
{
    private MonoBehaviour _monoBehaviour;
    private void OnEnable()
    {
        _monoBehaviour = target as MonoBehaviour;
    }
    public override void OnInspectorGUI()
    {
        // 원래 존재하는 Inspector를 그린다
        DrawDefaultInspector();
        //base.OnInspectorGUI(); 를 사용해도 상관 없다
            
        // 모든 매소드를 확인해 본다
        MethodInfo[] methods = _monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (MethodInfo method in methods)
        {
            ButtonAttribute buttonAttribute = Attribute.GetCustomAttribute(method, typeof(ButtonAttribute)) as ButtonAttribute;

            if (buttonAttribute != null)
            {
                // 버튼 이름 설정
                string buttonLabel = string.IsNullOrEmpty(buttonAttribute.ButtonName) ? method.Name : buttonAttribute.ButtonName;

                if (GUILayout.Button(buttonLabel))
                {
                    method.Invoke(_monoBehaviour, null);
                }
            }
        }
    }
}