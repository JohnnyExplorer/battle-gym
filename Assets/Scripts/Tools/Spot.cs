using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent;

namespace Agent.Tools {
    public class Spot : MonoBehaviour
    {
        Rigidbody rBody;
        public int found = 0;
        private TouchAgent engine;
        private int index;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Collision Detection Started " + found);
            rBody = GetComponent<Rigidbody>();
        }

        public void SetEngine(TouchAgent gameEngine, int i) {
            engine = gameEngine;
            index = i;
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
            Debug.Log(collision.gameObject.name);
            if(collision.gameObject.name == "Agent")
            {
                Debug.Log("GOT A HIT");
                engine.SendMessage("spotFound",index);
            }

        }        
    }
}
