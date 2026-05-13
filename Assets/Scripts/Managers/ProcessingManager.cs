using UnityEngine;
using UnityEngine.Rendering;

public class ProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume sceneVolume;

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

    public void ChangeBloom()
    {
        
    }



}
