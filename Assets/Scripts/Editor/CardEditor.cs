using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var card = (Card)target;
        if (GUILayout.Button("Switch state"))
        {
            card.SwitchState();
        }
    }
}