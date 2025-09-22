using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardView))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var card = (CardView)target;
        if (GUILayout.Button("Switch state"))
        {
            card.SwitchState();
        }
    }
}