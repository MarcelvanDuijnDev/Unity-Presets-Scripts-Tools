using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(AudioZone)), CanEditMultipleObjects]
public class AudioZoneEditor : Editor
{
    private SphereBoundsHandle _BoundsHandle = new SphereBoundsHandle();

    protected virtual void OnSceneGUI()
    {
        AudioZone audiozone = (AudioZone)target;

        _BoundsHandle.center = audiozone.transform.position;
        _BoundsHandle.radius = audiozone.Range;

        EditorGUI.BeginChangeCheck();
        _BoundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(audiozone, "Change Bounds");

            BoundingSphere newBounds = new BoundingSphere();
            newBounds.position = audiozone.transform.position;
            newBounds.radius = _BoundsHandle.radius;

            audiozone.Range = _BoundsHandle.radius;
        }

    }
}