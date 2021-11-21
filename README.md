# PAWA COMPETITION
## Your task is to train agents for game levels.
#### All the levels are located in the 'Assets/Scenes' directory.
#### To train the agent for the level you need to implement Unity side Agent part and set up Python ML agents learn part.
### Quickstart guide

#### 1. Download and install Unity

https://unity3d.com/get-unity/download

#### 2. Download repository and open it in the Unity

![image](https://user-images.githubusercontent.com/18628055/142763542-1a115bca-615b-4c16-bfcb-0ad3cee15137.png)

#### 3. Add Behavior Parameters script
 
Select level you want to train for and open it.

Choose Player Prefab in the left Scene Hierarchy and choose “Add Component” down in the prefub Inspector.

Find "Behavior Parameters" and choose it. 

![image](https://user-images.githubusercontent.com/18628055/142763656-2a8794a6-38bb-4ae3-bcc0-d68a8002f0f6.png)

#### 4. Implement Agent script

Go to the Scripts directory and create C# script with any name that you want

![image](https://user-images.githubusercontent.com/18628055/142763813-7e662def-0fa1-417b-80c6-a2cbf0fa98a1.png)

#### 5. Open script and modify it
+ Add following lines on the top
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Platformer.Mechanics;
 ```
 + Implement inheritance from Agent class for your script class (You will have other class name, that the same with script name).
```c#
public class AgentExample : Agent
```
+ Replace all the class content with following lines
```c#
public void Awake()
{
   PlayerControl = FindObjectOfType<PlayerController>();
   PlayerControl.controlEnabled = false;
}

public override void OnActionReceived(ActionBuffers actions)
{   

}
```
+ Implement Agent Actions
Use OnActionReceived method to allow agent perform actions. 
ActionBuffers actions argument contains DiscreteActions and ContinuousActions from the RL model as parameters.
There are two main parameters are available for the PlayerControl:

AgentMoveControl – float(continuous) in range [-1; 1], values that are less than 0 correspond to left move, and values that are above than 0 correspond to right move 
respectively.

AgentJumpControl – int(discrete), player controller will try to jump if value is 1.

All the discrete and continuous actions are divided into branches, it’s this is necessary so that the player can perform several actions in parallel mode. In most cases you will have 1 continuous branch for move and 1 discrete branch for jump. 

Example of the method for move and jump
```c#
public override void OnActionReceived(ActionBuffers actions)
    {   
        int actJump = actions.DiscreteActions[0];
        float actMove = actions.ContinuousActions[0];
        PlayerControl.AgentMoveControl = actMove;
        PlayerControl.AgentJumpControl = actJump;
    }
```

Indexes of discrete and continuous actions are the branches.

+  Implement Agent Rewards
Rewards are using to get feedback for the agent about how good agent’s policy for now. Cumulative reward is often the most important metrics to measure how good the agent deals with the task.

To help agent detect situations with rewards add following method to the class
```c#
void OnCollisionEnter2D(Collision2D collision)
    {   
        var tag = collision.gameObject.tag;

        if(tag == "Victory")
        {
            SetReward(+1f);
        }

        if(tag == "Death")
        {
            SetReward(-1f);
        }
    }
```
So, if Agent will touch some of zones with tags, it will receive reward corresponding to some of tags. 

You can check tags in scene hierarchy 

![image](https://user-images.githubusercontent.com/18628055/142764353-8cb936ae-420e-4172-9ece-500058ed78a5.png)

+ The last thing is the scene reload. 

If collision is end state, you need to reload game to begin from start.

Add following method to the class

```c#
private void Restart()
{   
    Scene scene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(scene.name);
    EndEpisode();
}
```

+ And add the restart to end state reward cases

```c#
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
```

#### The finished script should look like this:
```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Platformer.Mechanics;

public class AgentExample : Agent
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
        EndEpisode();
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
```

#### You can find an example in the "Assets/Scripts/example" directory

#### 6. Specify actions branches corresponding to your agent

![image](https://user-images.githubusercontent.com/18628055/142764634-f3324521-2f33-4f81-9dcd-ab48c446646b.png)

#### 7. Add your script

![image](https://user-images.githubusercontent.com/18628055/142764690-db01053c-711e-4dcb-a439-2026fde59811.png)

#### 8. Add Decision Requester script

![image](https://user-images.githubusercontent.com/18628055/142764732-bef50571-c8c8-4146-ac1c-976443c930ba.png)

#### 9. Add Sensor script

MLAgents provides few sensors for your choice, the most versatile is Camera Sensor. 

Don’t forget to add camera to the Camera Sensor.

![image](https://user-images.githubusercontent.com/18628055/142764815-b9bffada-72c0-4ebc-9e36-2a8c27354ebc.png)

### Done! Unity part is ready!

#### 10. Setup Python ML Agents part

+ Install python https://www.python.org/downloads/

+ Install ML Agents Learn for python https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Installation.md

#### 11. Launch training

Type in console

```
mlagents-learn
```

Then press Play button in the Unity

The folder with results will be created at the execution location

#### 12. Track your model with tensorboard

Type in console

```
tensorboard --logdir=results
```

Now you can see the all the model training history in your browser

![image](https://user-images.githubusercontent.com/18628055/142765343-53858639-fad1-4293-b6aa-06a6386d3361.png)

## Have fun and good luck!
