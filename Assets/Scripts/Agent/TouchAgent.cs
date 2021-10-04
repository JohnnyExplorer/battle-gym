using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Agent.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Agent {

    public class TouchAgent : Unity.MLAgents.Agent
    {

        [SerializeField] public GameObject spot;

        private  List<GameObject> spotInstance;
        

        public TouchAgent() {
            spotInstance = new List<GameObject>();
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach (int index in Enumerable.Range(0, 10))
            {
                Debug.Log("Index " + index);
                float offset = index + 2  * -7f;
                var _spot = Instantiate(spot, new Vector3(offset, 0.01f, 0), Quaternion.identity);
                spotInstance.Add(_spot);
                var spotScript = spotInstance[index].GetComponent<Spot>();
                spotScript.SetEngine(this,index);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // gameObject.Find("NameOfTheGameObjectTarget").GetComponent<NameOfTheScrit>()
            Debug.Log(spotInstance.Count);
            
        }


        public void spotFound(int index) {
            Destroy(spotInstance[index],1);
            spotInstance.RemoveAt(index);

        }
    }
}
