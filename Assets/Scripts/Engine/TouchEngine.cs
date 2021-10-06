using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agent;
using Agent.Controllers;
using Agent.Tools;


namespace Engine {

    public class TouchEngine : MonoBehaviour
    {
        //GameObject to spawn for spot touching
        [SerializeField] public GameObject gameObjectSpot;
        [SerializeField] public GameObject gameObjectAgent;
        [SerializeField] public GameObject gameObjectField;
        public int spotSpawnCount = 5;
        private int spotFoundCount = 0;
        private Kempo kempoAgent;
        private (float,float) area;
        private Dictionary<int,GameObject?> spotInstance;

        public TouchEngine() {
            spotInstance = new Dictionary<int,GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Spot Engine Start");
            area = GetFieldArea(gameObjectField.GetComponent<Renderer>());
            SpotSpawn(spotSpawnCount);
            SetupAgent(gameObjectAgent);
        }

        private void SetupAgent(GameObject agent)
        {
            kempoAgent = agent.GetComponent<Kempo>();
        }

        private void SpotSpawn(int count) {
            foreach (int index in Enumerable.Range(0, count))
            {
                var x = RandomLoc(area.Item1);
                var y = RandomLoc(area.Item2);
                spotInstance[index] = Instantiate(gameObjectSpot, new Vector3(x, 10.0f, y), Quaternion.identity);;
                var spotScript = spotInstance[index].GetComponent<Spot>();
                spotScript.SetEngine(this,index);
            }
        }

        private (float,float) GetFieldArea(Renderer field) {
            return ((int)System.Math.Round(field.bounds.size.x)/2,(int)System.Math.Round(field.bounds.size.z)/2);
        }

        private float Invert(float i) {
            return i * -1;
        }

        private float RandomLoc(float i){
            return Random.Range(this.Invert(i), i);
        }

        private void FixedUpdate() {
            if(spotInstance.Count <= 0) {
                kempoAgent.SendMessage("EngineReset");
            }
        }

        public void SpotRemove(int index) {
            Debug.Log("Removing Spot " + index);
            Destroy(spotInstance[index],1);
            spotInstance.Remove(index);
            Debug.Log("Count " + spotInstance.Count);
        }

        public void SpotFound(int index) {
            Debug.Log("FoundSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
                spotFoundCount ++;
                kempoAgent.SendMessage("RewardGoal");
            }
        }

        public void SpotLost(int index) {
            Debug.Log("LostSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
            }
        }

        public void AgentDead() {
            Resetboard();
        }

        private void SpotDeSpawn(){
            foreach(Spot _s in spotInstance) {
                Destroy(_s);
            }
        }

        private void Resetboard() {
             SpotDeSpawn();
             agent.transform.position = new Vector3(0, 0, 0);
             SpotSpawn(spotSpawnCount);

        }
    }
}
