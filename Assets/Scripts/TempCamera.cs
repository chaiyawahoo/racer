using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    private void Update() {
        if (GameObject.FindWithTag("MainCamera")) {
            gameObject.SetActive(false);
        }
    }
}
