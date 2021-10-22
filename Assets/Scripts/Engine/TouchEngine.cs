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
using RPGCharacterAnimsFREE;

namespace Engine {

    public class TouchEngine : MonoBehaviour
    {
        //GameObject to spawn for spot touching
        [SerializeField] public GameObject gameObjectSpot;
        [SerializeField] public GameObject gameObjectAgent;
        [SerializeField] public GameObject gameObjectField;
        //Spot controls
        public int spotSpawnCount;
        public int dropLocationDivider;
        private List<int> spotFound = new List<int>();
        public int activeSpots = 0;
        private Kempo kempoAgent;
        private (float,float) area;
        private Dictionary<int,GameObject?> spotInstance;
        private GameObject agentInstance;
        private bool spawning = false;

        public int frames = 0;
        public int maxframe = 1000;
        
        public TouchEngine() {
            spotInstance = new Dictionary<int,GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Debug.Log("ENGINE - Spot Engine Start");
            HingeJoint hingeInactive = this.GetComponentInChildren(typeof(HingeJoint), true) as HingeJoint;

            area = GetFieldArea(transform.Find("Plane").gameObject.GetComponent<Renderer>());
            InitialSpotSpawn(spotSpawnCount);
            SetupAgent(gameObjectAgent);
            GeneralUI.possible = activeSpots;
        }

        private void SetupAgent(GameObject agent)
        {
            if(agentInstance)
            Destroy(agentInstance);
            agentInstance = Instantiate(agent, new Vector3(
                                                    RandomLoc(area.Item1/dropLocationDivider),
                                                    10.0f,
                                                    RandomLoc(area.Item2/dropLocationDivider)
                                                    ), Quaternion.identity,this.GetComponent<Transform>());
            agentInstance.transform.localPosition = new Vector3(
                                                    RandomLoc(area.Item1/dropLocationDivider),
                                                    10.0f,
                                                    RandomLoc(area.Item2/dropLocationDivider));

            //setup floor
            RPGCharacterController agentRpgController = agentInstance.GetComponentInParent<RPGCharacterController>();
            agentRpgController.floor = gameObjectField.GetComponent<Transform>();
            agentRpgController.Init();
            kempoAgent = agentInstance.GetComponent<Kempo>();

            
            //spotInstance[index] 
            UpdateAgent(activeSpots);
           
        }

        private void InitialSpotSpawn(int count) {
            if(!spawning) {
                spawning = true;
                // Debug.Log("ENGINE SPAWNING!!" + count);
                foreach (int index in Enumerable.Range(0, count))
                {

                    spotInstance[index] = Instantiate(gameObjectSpot, new Vector3(
                                                                                    RandomLoc(area.Item1/dropLocationDivider),
                                                                                    10.0f,
                                                                                    RandomLoc(area.Item2/dropLocationDivider)
                                                                                    ), Quaternion.identity,GetComponent<Transform>());
                    //spotInstance[index].transform.parent = GetComponent<Transform>().root.GetComponent<Transform>();
                    spotInstance[index].transform.localPosition = new Vector3(
                                                    RandomLoc(area.Item1/dropLocationDivider),
                                                    10.0f,
                                                    RandomLoc(area.Item2/dropLocationDivider));
                    var spotScript = spotInstance[index].GetComponent<Spot>();
                    spotScript.SetEngine(this,index);
                    // Debug.Log("ENGINE - Spawning Spot " + index);
                }
                activeSpots = count;
                spawning = false;
            }
        }

        private void SpotSpawn(int count) {
            if(spawning == false) {
                spawning = true;
                // Debug.Log("ENGINE SPAWNING!!" + count);
                foreach (KeyValuePair<int, GameObject> spot in spotInstance)
                {
                    spot.Value.transform.localPosition = new Vector3(RandomLoc(area.Item1/dropLocationDivider), 10.1f, RandomLoc(area.Item2/dropLocationDivider));
                    spot.Value.GetComponent<Rigidbody>().useGravity = true;
                }
                activeSpots = count;
                spawning = false;
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
            if(activeSpots <= 0 && spawning == false) {
                if(spotFound.Count > 1) {
                    RewardAgentRewardFinished();
                }
                Resetboard();
            }
            if(IsAgentDead(agentInstance) && spawning == false) {
                PenalizeAgentDead();
                Resetboard();
            }
            
        }
        private void Update() {
            GeneralUI.ScreenText();
            frames++;

            if(frames >= maxframe) {
                PenalizeAgentTime();
                Resetboard();
            } else {
                PenalizeAgentforiteration();
            }
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

        private void PenalizeAgentTime() {
            GeneralUI.fail += 1;
            SignalAgent("TimeUp");
        }

        private void PenalizeAgentforiteration() {
            SignalAgent("IterationPenalty");
        }
        
        private void UpdateAgent(int val) {
            SignalAgent("UpdateGoalCount",activeSpots);

        }

        private void SignalAgent(string signal, int? payLoad = null) {
            if(payLoad == null) {
               kempoAgent.SendMessage(signal);
               return;
            }
            kempoAgent.SendMessage(signal,payLoad);
            
        }

        public void SpotRemove(int index) {
            // Debug.Log("ENGINE - Removing Spot " + index);
            if(activeSpots > 0)
                activeSpots = activeSpots - 1;
            spotInstance[index].transform.localPosition = new Vector3((index * 5) +35, 10.1f, 55);
            spotInstance[index].GetComponent<Rigidbody>().useGravity = false;
            spotInstance[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
            UpdateAgent(activeSpots);
            // Debug.Log("ENGINE - Count " + activeSpots);
        }

        public void SpotFound(int index) {
            // Debug.Log("ENGINE - FoundSignal From " + index);
            if(!spotFound.Contains(index)) {
                SpotRemove(index);
                spotFound.Add(index);
                RewardAgentGoal();
            }
        }

        public void SpotLost(int index) {
            // Debug.Log("ENGINE - LostSignal From " + index);
            if(spotInstance.ContainsKey(index)) {
                SpotRemove(index);
                UpdateAgent(activeSpots);
            }
        }

        

        private void Resetboard() {
             var agentTotalReward = kempoAgent.getTotalRewards();
             SignalAgentEngineReset();
             agentInstance.transform.localPosition = new Vector3(
                                                    RandomLoc(area.Item1/dropLocationDivider),
                                                    0.1f,
                                                    RandomLoc(area.Item2/dropLocationDivider));
             //SetupAgent(gameObjectAgent);
             SpotSpawn(spotSpawnCount);
             frames = 0;
             GeneralUI.reward = agentTotalReward; 
             spotFound.Clear();

        }
    }
}
