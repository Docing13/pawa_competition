using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Platformer.Mechanics;

public class AgentCompetitor : Agent
{
    PlayerController PlayerControl;

    public void Awake()
    {
        PlayerControl = FindObjectOfType<PlayerController>();
        PlayerControl.controlEnabled = false;
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {   
        var act = actions.DiscreteActions[0];
        
        PlayerControl.AgentMoveControl = 0.6f;
        PlayerControl.AgentJumpControl = act;
    }

    private void Restart()
    {   
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        // EndEpisode();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {   
        var tag = collision.gameObject.tag;

        if(tag == "Victory")
        {
            SetReward(+1);
            Restart();
        }

        if(tag == "Death")
        {
            SetReward(-1f);
            Restart();
        }
    }
}