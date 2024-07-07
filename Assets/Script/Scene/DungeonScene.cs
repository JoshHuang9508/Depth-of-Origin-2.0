using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : MonoBehaviour
{
    public bool isRoomGenerateDone;

    [Header("Reference")]
    [SerializeField] private SceneControl sceneControl;

    private void Update()
    {
        sceneControl.isSceneSetup = isRoomGenerateDone;
    }
}
