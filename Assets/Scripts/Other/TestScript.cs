using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVD;

public class TestScript : MonoBehaviour
{

    void Update()
    {
        mvd.Movement_FreeCamera(5, 10, transform);
        
    }
}
