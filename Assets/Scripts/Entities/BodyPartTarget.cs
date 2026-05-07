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

    public BodyPart GetBodyPart()
    {
        return bodyPart;
    }

    public void SetHighlight(bool state)
    {
        if (rend == null) return;

        rend.material.color = state ? highlightColour : originalColor;
    }
}
