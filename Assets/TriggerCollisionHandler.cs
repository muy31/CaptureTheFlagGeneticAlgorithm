using UnityEngine;

public class TriggerCollisionHandler : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Vector3 pushDirection = (other.transform.position - transform.position).normalized;
        if (pushDirection.magnitude == 0)
        {
            pushDirection = Vector3.right;
        }
        other.transform.position += pushDirection * Time.deltaTime * 3f;
    }
}
