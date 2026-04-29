using UnityEngine;
using UnityEngine.InputSystem;

public class DebugBuildingSelector : MonoBehaviour
{
    void Start()
    {
        BuildingManager.Instance.SetConstructionIndex(0); //
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            BuildingManager.Instance.SetConstructionIndex(0); //BARRACA

        if (Input.GetKeyDown(KeyCode.Alpha1))
            BuildingManager.Instance.SetConstructionIndex(1); //DEFENSA

        if (Input.GetKeyDown(KeyCode.Alpha2))
            BuildingManager.Instance.SetConstructionIndex(2); //PLAZA

        if (Input.GetKeyDown(KeyCode.Alpha3))
            BuildingManager.Instance.SetConstructionIndex(3); //MURALLA

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            BuildingManager.Instance.TryEnterMoveMode(); //MOVER
        }
    }
}