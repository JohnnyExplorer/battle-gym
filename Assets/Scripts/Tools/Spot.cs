using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agent;
using Engine;

namespace Agent.Tools {
    public class Spot : MonoBehaviour
    {
        public bool found = false;
        private TouchEngine engine;
        private int index;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("SPOT - Collision Detection Started " + found);
        }

        public void SetEngine(TouchEngine gameEngine, int i) {
            engine = gameEngine;
            index = i;
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate() {
            if(found) {
                Debug.Log("SPOT - GOT A HIT " + index);
                engine.SendMessage("SpotFound",index);
            }
            
            if(GetComponent<Transform>().position.y < 0) {
                Debug.Log("SPOT - Lost a spot HIT " + index);
                engine.SendMessage("SpotLost",index);
            }
        }

        private void OnTriggerEnter(Collider collision) {
            Debug.Log("SPOT - Trigger Detected");
        }

        private void OnCollisionEnter(Collision collision) {
            Debug.Log("SPOT - OnCollisionEnter" + collision.gameObject.name);
            if(collision.gameObject.name == "Agent")
            {
                found = true;
            }
        }        
    }
}
