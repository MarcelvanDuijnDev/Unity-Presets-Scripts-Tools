using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class AudioZoneBox : MonoBehaviour
{
    private enum Options { SetVolume, VolumeOnDistance };
    [Header("Type")]
    [SerializeField] private Options _Option = Options.SetVolume;

    [Header("Target")]
    [SerializeField] private Transform _ZoneEffector = null;

    [Header("Settings - Zone")]
    [SerializeField] private string _AudioTrack = "";
    [SerializeField] private float _Volume = 1;

    [Tooltip("1 = volume from 0 to max based on how close the effector is to the center.")]
    [SerializeField] private float _IncreaseMultiplier = 1;

    // Check effector leaving bounds
    private bool _EffectorInBounds;

    //Bounds
    public Bounds Bounds { get { return _Bounds; } set { _Bounds = value; } }
    [SerializeField] private Bounds _Bounds = new Bounds(Vector3.zero, new Vector3(5,5,5));

    // Optimization (This way the AudioHandler doesn't have too loop trough the available audiotracks)
    private int _AudioTrackID;

    // AudioHandler Ref
    private AudioHandler AudioHandler;

    // Max distance
    private float _MaxDistance;

    void Start()
    {
        AudioHandler = AudioHandler.AUDIO;

        if (_ZoneEffector == null)
        {
            try { _ZoneEffector = GameObject.Find("Player").transform; }
            catch { Debug.Log("No Effector Assigned!"); }
        }
        
        // Get TrackID
        _AudioTrackID = AudioHandler.AUDIO.Get_Track_ID(_AudioTrack);

        // Set max distance
        _MaxDistance = Vector3.Distance(Vector3.zero, _Bounds.size) *.25f;
    }

    void Update()
    {
        if (Bounds.Contains(_ZoneEffector.position))
        {
            switch(_Option)
            {
                case Options.SetVolume:
                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, _Volume, true);
                    break;
                case Options.VolumeOnDistance:
                    float distance = Vector3.Distance(_Bounds.center, _ZoneEffector.position);
                    float newvolume = (1 - (distance / _MaxDistance)) * _Volume * _IncreaseMultiplier;
                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, newvolume, true);
                    break;
            }

            // Check Effector OnExit
            if (!_EffectorInBounds)
                _EffectorInBounds = true;
        }
        else
        {
            // Effector OnExit
            if (_EffectorInBounds)
            {
                AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, 0, true);
                _EffectorInBounds = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(0, 1f, 0, 0.1f);
        Gizmos.DrawCube(transform.position, Bounds.size);
    }
}

//Editor Bounds
[CustomEditor(typeof(AudioZoneBox)), CanEditMultipleObjects]
public class AudioZoneBoxEditor : Editor
{
    private BoxBoundsHandle _BoundsHandle = new BoxBoundsHandle();

    protected virtual void OnSceneGUI()
    {
        AudioZoneBox audiozonebox = (AudioZoneBox)target;

        _BoundsHandle.center = audiozonebox.transform.position;
        _BoundsHandle.size = audiozonebox.Bounds.size;

        EditorGUI.BeginChangeCheck();
        _BoundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(audiozonebox, "Change Bounds");

            Bounds newBounds = new Bounds();
            newBounds.center = audiozonebox.transform.position;
            newBounds.size = _BoundsHandle.size;
            audiozonebox.Bounds = newBounds;
        }
    }
}