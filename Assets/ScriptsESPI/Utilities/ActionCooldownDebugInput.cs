using UnityEngine;

// Debug input used to test move and upgrade cooldowns
public class ActionCooldownDebugInput : MonoBehaviour
{
    [SerializeField] private CooldownSystem cooldownSystem;

    private void Update()
    {
        if (cooldownSystem == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            cooldownSystem.StartCooldown(CooldownType.MoveStructure);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            cooldownSystem.StartCooldown(CooldownType.UpgradeStructure);
        }
    }
}