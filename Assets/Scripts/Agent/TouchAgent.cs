using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Agent.Tools;
namespace Agent {

    public class TouchAgent : Unity.MLAgents.Agent
    {
        [SerializeField] public GameObject spot;

        private bool spotIsFound = false;
        private GameObject spotInstance;
        // Start is called before the first frame update
        void Start()
        {
            spotInstance = Instantiate(spot, new Vector3(-7f, 0.5f, 0.13f), Quaternion.identity);
            // _spot.SendMessage("SetEngine",this);
            var spotScript = spotInstance.GetComponent<Spot>();
            spotScript.SetEngine(this);

        }

        // Update is called once per frame
        void Update()
        {
            // gameObject.Find("NameOfTheGameObjectTarget").GetComponent<NameOfTheScrit>()
            if(spotIsFound) {
                Destroy(spotInstance,1);
            }
        }


        public void spotFound() {
            spotIsFound = true;
        }
    }
}
