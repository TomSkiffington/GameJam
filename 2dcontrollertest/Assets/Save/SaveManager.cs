using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public GameObject player;
    private Vector3 savedPos;

    private void Start() {
        if (PlayerPrefs.GetFloat("xPos") != 0 && PlayerPrefs.GetFloat("yPos") != 0) {
            savedPos.x = PlayerPrefs.GetFloat("xPos");
            savedPos.y = PlayerPrefs.GetFloat("yPos");
            player.transform.position = savedPos;
        }
    }


    private void FixedUpdate() {
        Save();
    }

    public void Save() {
        PlayerPrefs.SetFloat("xPos", gameObject.transform.position.x);
        PlayerPrefs.SetFloat("yPos", gameObject.transform.position.y);
    }
}
