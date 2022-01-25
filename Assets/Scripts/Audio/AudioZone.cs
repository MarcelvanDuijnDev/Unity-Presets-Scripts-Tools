using UnityEngine;

public class AudioZone : MonoBehaviour
{
    private enum Options { SetVolume, VolumeOnDistance };
    [Header("Type")]
    [SerializeField] private Options _Option = Options.SetVolume;

    [Header("Target")]
    [SerializeField] private Transform _ZoneEffector = null;

    [Header("Settings - Zone")]
    [SerializeField] private string _AudioTrackName = "";
    [SerializeField] private float _Volume = 1;
    public float Range = 10;
    [Tooltip("1 = volume from 0 to max based on how close the effector is to the center.")]
    [SerializeField] private float _IncreaseMultiplier = 1;

    [Header("3D Audio")]
    [SerializeField] private bool _Use3DAudio = true;
    [SerializeField] private bool _UseThisPos = true;
    [SerializeField] private bool _CreateNewAudioSource = true;

    // Check effector leaving bounds
    private bool _EffectorInBounds;

    // Optimization (This way the AudioHandler doesn't have too loop trough the available audiotracks)
    private int _AudioTrackID;

    // Max distance
    private float _MaxDistance;

    void Start()
    {
        if (AudioHandler.AUDIO != null)
        {
            //3D Audio
            if (_Use3DAudio)
            {
                if (_CreateNewAudioSource)
                    _AudioTrackName = AudioHandler.AUDIO.DuplicateAudioTrack(_AudioTrackName);

                if (_UseThisPos)
                    AudioHandler.AUDIO.ChangeAudioPosition(_AudioTrackName, transform.position);
            }

            if (_ZoneEffector == null)
            {
                try { 
                    _ZoneEffector = GameObject.FindObjectsOfType<AudioListener>()[0].gameObject.transform; 
                }
                catch { Debug.Log("No AudioListener Found In The Scene"); }
            }

            // Get TrackID
            _AudioTrackID = AudioHandler.AUDIO.Get_Track_ID(_AudioTrackName);
            if (_AudioTrackID == -1)
                Debug.Log("AudioZone: Track(" + _AudioTrackName + ") Does not Exist");

            // Set max distance
            _MaxDistance = Range;
        }
        else
        {
            _AudioTrackID = -1;
            Debug.Log("AudioZone: AudioHandler does not exist in scene");
        }
    }

    void Update()
    {
        if (_AudioTrackID == -1)
            return;
        if (Vector3.Distance(transform.position,_ZoneEffector.position) <= _MaxDistance)
        {
            switch (_Option)
            {
                case Options.SetVolume:
                    AudioHandler.AUDIO.SetTrackVolume(_AudioTrackID, _Volume, true);
                    break;
                case Options.VolumeOnDistance:
                    float distance = Vector3.Distance(transform.position, _ZoneEffector.position);
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

    public void PlayTrack()
    {
        AudioHandler.AUDIO.PlayTrack(_AudioTrackName);
    }
    public void StartTrack()
    {
        AudioHandler.AUDIO.StartTrack(_AudioTrackName);
    }
    public void StopTrack()
    {
        AudioHandler.AUDIO.StopTrack(_AudioTrackName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Vector4(0, 1f, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, Range);
    }
}