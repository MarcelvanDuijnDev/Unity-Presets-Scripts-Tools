using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class AudioZoneSphere : MonoBehaviour
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
    public float BoundsRadius;
    public BoundingSphere Bounds { get { return _Bounds; } set { _Bounds = value; } }
    [SerializeField] private BoundingSphere _Bounds = new BoundingSphere(Vector3.zero, 5);

    // Optimization (This way the AudioHandler doesn't have too loop trough the available audiotracks)
    private int _AudioTrackID;

    // AudioHandler Ref
    private AudioHandler AudioHandler;

    // Max distance
    private float _MaxDistance;

    void Start()
    {
        _Bounds = new BoundingSphere(transform.position, BoundsRadius);

        AudioHandler = AudioHandler.AUDIO;

        if (_ZoneEffector == null)
        {
            try { _ZoneEffector = GameObject.Find("Player").transform; }
            catch { Debug.Log("No Effector Assigned!"); }
        }
        
        // Get TrackID
        _AudioTrackID = AudioHandler.AUDIO.Get_Track_ID(_AudioTrack);

        // Set max distance
        _MaxDistance = Bounds.radius;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position,_ZoneEffector.position) <= _MaxDistance)
        {
            switch (_Option)
            {
                case Options.SetVolume:
                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, _Volume, true);
                    break;
                case Options.VolumeOnDistance:
                    float distance = Vector3.Distance(Bounds.position, _ZoneEffector.position);
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
        Gizmos.DrawSphere(transform.position, Bounds.radius);
    }
}

//Editor Bounds
[CustomEditor(typeof(AudioZoneSphere)), CanEditMultipleObjects]
public class AudioZoneSphereEditor : Editor
{
    private SphereBoundsHandle _BoundsHandle = new SphereBoundsHandle();

    protected virtual void OnSceneGUI()
    {
        AudioZoneSphere audiozonesphere = (AudioZoneSphere)target;

        _BoundsHandle.center = audiozonesphere.transform.position;
        _BoundsHandle.radius = audiozonesphere.Bounds.radius;

        EditorGUI.BeginChangeCheck();
        _BoundsHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(audiozonesphere, "Change Bounds");

            BoundingSphere newBounds = new BoundingSphere();
            newBounds.position = audiozonesphere.transform.position;
            newBounds.radius = _BoundsHandle.radius;
            audiozonesphere.Bounds = newBounds;

            audiozonesphere.BoundsRadius = _BoundsHandle.radius;
            audiozonesphere.Bounds = new BoundingSphere(Vector3.zero, audiozonesphere.BoundsRadius);
        }
        
    }
}
