using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int damages;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player"){
            other.GetComponent<CanTakeDamage>().TakeHit(damages);
        }
    }
}
