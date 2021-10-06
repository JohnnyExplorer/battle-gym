using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agent;
using Agent.Controllers;
using Agent.Tools;
using UI;

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
            Debug.Log("ENGINE - Spot Engine Start");
            area = GetFieldArea(gameObjectField.GetComponent<Renderer>());
            SpotSpawn(spotSpawnCount);
            SetupAgent(gameObjectAgent);
            GeneralUI.possible = spotInstance.Count;
        }

        private void SetupAgent(GameObject agent)
        {
            UpdateAgent(spotInstance.Count);
           
        }

        private void SpotSpawn(int count) {
            foreach (int index in Enumerable.Range(0, count))
            {
                spotInstance[index] = Instantiate(gameObjectSpot, new Vector3(RandomLoc(area.Item1)/4, 10.0f, RandomLoc(area.Item2)/4), Quaternion.identity);;
                var spotScript = spotInstance[index].GetComponent<Spot>();
                spotScript.SetEngine(this,index);
                Debug.Log("ENGINE - Spawning Spot " + index);
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
                if(spotFoundCount > 1) {
                    RewardAgentRewardFinished();
                }
                Resetboard();
            }
            if(IsAgentDead(gameObjectAgent)) {
                PenalizeAgentDead();
                Resetboard();
            }
            
        }

        private void Update() {
            GeneralUI.ScreenText();
        }


        private void RewardAgentRewardFinished() {
            GeneralUI.success += 1;
            GeneralUI.points += 0;
            SignalAgent("RewardFinished");
        }
        private void SignalAgentEngineReset() {
            GeneralUI.episode += 1;
            SignalAgent("EngineReset");
        }

        private bool IsAgentDead(GameObject agentObject) {
            return (agentObject.GetComponent<Transform>().position.y < 0);
        }

        private void RewardAgentGoal() {
            GeneralUI.points += 1;
            SignalAgent("RewardGoal");
        }

        private void PenalizeAgentDead() {
            GeneralUI.fail += 1;
            SignalAgent("Dead");
        }

        private void UpdateAgent(int val) {
            SignalAgent("UpdateGoalCount",spotInstance.Count);

        }

        private void SignalAgent(string signal, int? payLoad = null) {
            if(payLoad == null) {
               gameObjectAgent.SendMessage(signal);
               return;
            }
            gameObjectAgent.SendMessage(signal,payLoad);
            
        }

        public void SpotRemove(int index) {
            Debug.Log("ENGINE - Removing Spot " + index);
            Destroy(spotInstance[index],1);
            spotInstance.Remove(index);
            UpdateAgent(spotInstance.Count);
            Debug.Log("ENGINE - Count " + spotInstance.Count);
        }

        public void SpotFound(int index) {
            Debug.Log("ENGINE - FoundSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
                spotFoundCount ++;
                RewardAgentGoal();
            }
        }

        public void SpotLost(int index) {
            Debug.Log("ENGINE - LostSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
                UpdateAgent(spotInstance.Count);
            }
        }

        private void SpotDeSpawn(){
            //spotInstance.map(fun x -> Destroy(x));
            //TODO map?
            foreach(KeyValuePair<int, GameObject> _s in spotInstance) {
                Destroy(_s.Value);
            }
        }

        private void Resetboard() {
             SignalAgentEngineReset();
             SpotDeSpawn();
             gameObjectAgent.transform.position = new Vector3(0, 0.1f, 0);
             SpotSpawn(spotSpawnCount);

        }
    }
}
