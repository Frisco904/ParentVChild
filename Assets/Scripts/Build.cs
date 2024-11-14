using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    public void ToggleBuildMode()
    {
        TurretBuilder.main.BuildMode();
    }
}
