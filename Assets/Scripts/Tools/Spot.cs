using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    Rigidbody rBody;
    int found = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Collision Detection Started");
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider collision) {
        Debug.Log("Trigger Detected");
    }

    private void OnCollisionEnter(Collision collision)
    {
        found = 1;

    }

    public int isFound() {
        return found;
    }
}
