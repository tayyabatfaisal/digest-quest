using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigestQuest;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DeckManager))]
public class DeckManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeckManager deckManager = (DeckManager)target;
        if (GUILayout.Button("Draw Next Card")){
            HandManager handManager = FindObjectOfType<HandManager>();
            if (handManager != null){
                deckManager.DrawCard(handManager);
            }
        }
    }
}
#endif