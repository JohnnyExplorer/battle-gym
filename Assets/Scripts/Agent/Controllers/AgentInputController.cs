using UnityEngine;
using RPGCharacterAnimsFREE;
using RPGCharacterAnimsFREE.Actions;
using Agent.Actions;
using Agent.Tools;


namespace Agent.Controllers
{
	public class AgentInputController : RPGCharacterInputController
    {

        private MoveForwardABit agentAction;

        public AgentInputController() {
            agentAction = new MoveForwardABit();
        }
        public override void Inputs()
        {
            try {
                // inputJump = Input.GetButtonDown("Jump");
                // isJumpHeld = Input.GetButton("Jump");
                // inputLightHit = Input.GetButtonDown("LightHit");
                // inputDeath = Input.GetButtonDown("Death");
                // inputAttackL = Input.GetButtonDown("AttackL");
                // inputAttackR = Input.GetButtonDown("AttackR");
                // inputSwitchUpDown = Input.GetAxisRaw("SwitchUpDown");
                // inputAim = Input.GetAxisRaw("Aim");
                // inputAiming = Input.GetButton("Aiming");
                // inputRoll = Input.GetButtonDown("L3");
                //inputVertical = Input.GetAxisRaw("Vertical");
                //inputHorizontal = Input.GetAxisRaw("Horizontal");
                //inputVertical = agentAction.MoveForward();
                //inputHorizontal = agentAction.MoveForward();

            } catch (System.Exception) {  
                //Debug.LogError("Inputs not found!");
            }
        }
    }

}