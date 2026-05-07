using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyBodyPart : MonoBehaviour
{
    private Enemy owner;
    private BodyPart bodyPart;
    private bool isActive = false;

    public void Initialize(Enemy enemy, BodyPart part)
    {
        owner = enemy;
        bodyPart = part;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
