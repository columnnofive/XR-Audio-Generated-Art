using UnityEngine;

public class DeactivateOnStart : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
