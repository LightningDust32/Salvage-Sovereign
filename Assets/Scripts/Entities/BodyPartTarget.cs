using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPartTarget : MonoBehaviour
{
    [SerializeField] private BodyPart bodyPart;
    [SerializeField] private Color highlightColour;

    private Renderer rend;
    private Color originalColor;

    private Enemy parentEnemy;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        parentEnemy = GetComponentInParent<Enemy>();
    }

    private void OnMouseEnter()
    {
        if (!CanInteract()) return;

        SetHighlight(true);
    }

    private void OnMouseExit()
    {
        SetHighlight(false);
    }

    private void OnMouseDown()
    {
        if (!CanInteract()) return;

        Player player = FindFirstObjectByType<Player>();

        if (player != null && parentEnemy != null)
        {
            player.ExecutePowerAttack(bodyPart, parentEnemy);
        }
    }

    private bool CanInteract()
    {
        if (parentEnemy == null) return false;

        return parentEnemy.IsTargetingActive();
    }

    private void SetHighlight(bool state)
    {
        if (rend == null) return;

        rend.material.color = state ? highlightColour : originalColor;
    }
}
