using UnityEngine;
using Engine;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Agent.State
{
    public class AgentState : MonoBehaviour
    {
        public Dictionary<string,dynamic?> seek;
        public TouchEngine engine;
        public AgentState(TouchEngine engine)
        {
            seek = new Dictionary<string,dynamic>();
            seek.active = 0;
            seek.refresh = 100;
            seek.count = 0;
            seek.value = 0;
            Debug.Log("AgentState = init");
            Debug.Log(this);
            Reset();
        }
        public void UseSeek()
        {
            Debug.Log("AgentState = UseSeek");
            Debug.Log(this);
            if (seek.active == 1)
            {
                seek.value = engine.GetSpot();
            }
        }
        public void Tick()
        {
            Debug.Log("AgentState = Tick");
            Debug.Log(this);
            if (seek.active == 0)
            {
                if (seek.count == seek.refresh)
                {
                    seek.active = 1;
                }
                seek.count++;
            }
        }
        public void Reset()
        {
            Debug.Log("AgentState = ()");
            Debug.Log(this);
            seek.count = 0;
            seek.active = 0;
            seek.value = 0;
        }
    }
}