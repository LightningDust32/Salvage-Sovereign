using UnityEngine;
using UnityEngine.Rendering;

public class ProcessingManager : MonoBehaviour
{
    [SerializeField] private VolumeProfile screenVolume;

    public static ProcessingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
