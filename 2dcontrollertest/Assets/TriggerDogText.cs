using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDogText : MonoBehaviour
{
    public GameObject text;

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.tag == "Player") {
            text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            text.SetActive(false);
        }
    }
}
