using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Camera_CutScenes : MonoBehaviour
{
    [SerializeField] private List<Movement_Camera_CutScene> _CutScenes = new List<Movement_Camera_CutScene>();
    [SerializeField] private LayerMask _TargetLayer;

    private int _Current_CutScene;
    private bool _HasCutScenes;

    [Header("Debug Gizmos")]
    [SerializeField] private int _Gizmos_Current_CutScene;

    private void Start()
    {
        for (int i = 0; i < _CutScenes.Count; i++)
        {
            if (_CutScenes[i].OnTrigger)
            {
                _HasCutScenes = true;
                break;
            }
        }
    }

    void Update()
    {
        //If OnTrigger
        if (_HasCutScenes)
            for (int i = 0; i < _CutScenes.Count; i++)
            {
                if (_CutScenes[i].OnTrigger)
                {
                    if (_CutScenes[i].CurrrentScene < _CutScenes[i].CutScene_Scenes.Count)
                    {
                        int detectplayer = Physics.BoxCastAll(_CutScenes[i].CutSceneTriggerPos.position, _CutScenes[i].Size, transform.forward, Quaternion.identity, 1, _TargetLayer).Length;

                        if (detectplayer > 0)
                            Movement_Camera.CAM.CutScene(true, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutScenePosition, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneTarget, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneRotation, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].FollowTarget);
                        else
                            Movement_Camera.CAM.CutScene(false);
                    }
                }
            }
    }

    public void NextScene()
    {
        _CutScenes[_Current_CutScene].CurrrentScene++;
        if (_CutScenes[_Current_CutScene].CurrrentScene < _CutScenes[_Current_CutScene].CutScene_Scenes.Count)
            Movement_Camera.CAM.CutScene(true, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutScenePosition, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneTarget, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneRotation, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].FollowTarget);
        else
            Movement_Camera.CAM.CutScene(false);
    }

    //Set Cutscene
    public void Set_CutScene(int cutsceneid)
    {
        _Current_CutScene = cutsceneid;
    }
    public void Set_CutScene(string cutscenename)
    {
        for (int i = 0; i < _CutScenes.Count; i++)
        {
            if(cutscenename == _CutScenes[i].CutScene_Name)
            {
                _Current_CutScene = i;
                break;
            }
        }
    }

    //Set Scene
    public void Set_Scene(int sceneid)
    {
        _CutScenes[_Current_CutScene].CurrrentScene = sceneid;
    }
    public void Set_Scene(string scenename)
    {
        for (int i = 0; i < _CutScenes[_Current_CutScene].CutScene_Scenes.Count; i++)
        {
            if(scenename == _CutScenes[_Current_CutScene].CutScene_Scenes[i].Scene_Name)
            {
                _CutScenes[_Current_CutScene].CurrrentScene = i;
                break;
            }
        }
    }
    public void Set_Scene(int sceneid, bool startscene)
    {
        _CutScenes[_Current_CutScene].CurrrentScene = sceneid;
        if (startscene)
            Start_CutScene();
    }
    public void Set_Scene(string scenename, bool startscene)
    {
        for (int i = 0; i < _CutScenes[_Current_CutScene].CutScene_Scenes.Count; i++)
        {
            if (scenename == _CutScenes[_Current_CutScene].CutScene_Scenes[i].Scene_Name)
            {
                _CutScenes[_Current_CutScene].CurrrentScene = i;
                if (startscene)
                    Start_CutScene();
                break;
            }
        }
    }

    //Start / Stop Scene
    public void Start_CutScene()
    {
        Movement_Camera.CAM.CutScene(true, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutScenePosition, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneTarget, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].CutSceneRotation, _CutScenes[_Current_CutScene].CutScene_Scenes[_CutScenes[_Current_CutScene].CurrrentScene].FollowTarget);
    }
    public void Stop_CutScene()
    {
        Movement_Camera.CAM.CutScene(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (_Gizmos_Current_CutScene < _CutScenes.Count)
        {
            //Trigger Position
            Gizmos.color = new Vector4(0, 1, 0, 0.2f);
            if (_CutScenes[_Gizmos_Current_CutScene].OnTrigger)
                Gizmos.DrawCube(_CutScenes[_Gizmos_Current_CutScene].CutSceneTriggerPos.position, _CutScenes[_Gizmos_Current_CutScene].Size);

            //Camera Positions
            for (int i = 0; i < _CutScenes[_Gizmos_Current_CutScene].CutScene_Scenes.Count; i++)
            {
                //OnFollow / Locked
                if (_CutScenes[_Gizmos_Current_CutScene].CutScene_Scenes[i].FollowTarget)
                    Gizmos.color = new Vector4(0, 1, 0, 0.2f);
                else
                    Gizmos.color = new Vector4(1, 0, 0, 0.2f);

                //DrawCamera
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(_CutScenes[_Gizmos_Current_CutScene].CutScene_Scenes[i].CutScenePosition, Quaternion.Euler(_CutScenes[_Gizmos_Current_CutScene].CutScene_Scenes[i].CutSceneRotation), Vector4.one);
                Gizmos.DrawFrustum(Vector4.zero, 60, 10, 0.5f, 1);
                Gizmos.matrix = oldGizmosMatrix;
                Gizmos.DrawSphere(_CutScenes[_Gizmos_Current_CutScene].CutScene_Scenes[i].CutScenePosition, 0.5f);
            }
        }
    }
}

[System.Serializable]
public class Movement_Camera_CutScene
{
    public string CutScene_Name;

    public List<Movement_Camera_CutSceneLocation> CutScene_Scenes;
    public int CurrrentScene;

    [Header("Trigger Area")]
    public bool OnTrigger;
    public Transform CutSceneTriggerPos;
    public Vector3 Size;
}

[System.Serializable]
public class Movement_Camera_CutSceneLocation
{
    public string Scene_Name;

    [Header("CutScene Settings")]
    public Vector3 CutScenePosition;
    public Vector3 CutSceneRotation;

    [Header("Follow Target")]
    public bool FollowTarget;
    public GameObject CutSceneTarget;
}