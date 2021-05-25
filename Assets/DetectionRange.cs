using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPlayerDetected(GameObject player);
public delegate void OnPlayerExit();

public class DetectionRange : MonoBehaviour
{
    private OnPlayerDetected onPlayerDetected;
    private OnPlayerExit onPlayerExit;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
            onPlayerDetected.Invoke(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player")
            onPlayerExit.Invoke();
    }

    public void AddOnPlayerDetected(OnPlayerDetected value){
        onPlayerDetected += value;
    }

    public void AddOnPlayerNotDetected(OnPlayerExit value){
        onPlayerExit += value;
    }
}
