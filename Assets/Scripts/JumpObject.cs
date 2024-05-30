using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{   
    public float jumpPower;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
    }
}
