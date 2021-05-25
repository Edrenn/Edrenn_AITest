using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CanTakeDamage {
    void TakeHit(int damage);
}

public class Player : MonoBehaviour, CanTakeDamage
{
    [SerializeField] private int healthPoints = 3;
    private bool canTakeDamage = true;
    public void TakeHit(int damage){
        if (canTakeDamage){
            StartCoroutine("InvincibilityFrame");
            healthPoints -= damage;
        }
    }
    IEnumerator InvincibilityFrame(){
        canTakeDamage = false;
        yield return new WaitForSeconds(2);
        canTakeDamage = true;
    }
}

