using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.SetActive(false);
        }
    }
}
