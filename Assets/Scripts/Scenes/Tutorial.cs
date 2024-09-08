using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //declare editor parameters
    [SerializeField] private List<Stage> _stages = new List<Stage>();

    //declare script variables
    private int _currentStageIdx = -1; //start index on -1 because it is increased in the enableStage function
    private bool _isLastStage = false;

    private void Start()
    {
        //make sure the first stage is properly enabled
        EnableStage();
    }

    private void Update()
    {
        //ignore updates when on last stage, win condition is handled in GameOver script
        if (_isLastStage)
            return;

        //check if the current stage was completed
        if (IsStageCompleted())
        {
            //handle opening the exits for the current stage
            OpenExits();

            //handle enabling everything that needs to be enabled for the next stage
            EnableStage();
        }
    }

    private bool IsStageCompleted()
    {
        //get the current stage
        Stage currentStage = _stages[_currentStageIdx];

        if (currentStage != null)
        {
            //check if every target for the current stage is destroyed
            foreach (GameObject target in currentStage.Targets)
            {
                if (target != null)
                {
                    //check if the target is a projectile defence
                    ProjectileDefence projectileDefence = target.GetComponent<ProjectileDefence>();

                    //if so, check if it is destroyed
                    if (projectileDefence != null)
                    {
                        if (!projectileDefence.IsDestroyed)
                            return false;

                        continue;
                    }

                    //check if the target is a npc defence
                    NPCDefence npcDefence = target.GetComponent<NPCDefence>();

                    //if so, check if it is destroyed
                    if (npcDefence != null)
                    {
                        if (!npcDefence.IsDestroyed)
                            return false;

                        continue;
                    }

                    //check if the target is a barrack
                    Barrack barrack = target.GetComponent<Barrack>();

                    //if so, check if it is destroyed
                    if (barrack != null)
                    {
                        if (!barrack.IsDestroyed)
                            return false;

                        continue;
                    }

                    //if the target is none of the above types it is a wall or enemy which are not eliminated yet
                    return false;
                }
            }

            //check if all spawned enemies are eliminated as well
            EnemyCharacter enemy = currentStage.gameObject.GetComponentInChildren<EnemyCharacter>();

            if (enemy != null)
                return false;

            return true;
        }

        return false;
    }

    private void OpenExits()
    {
        //get stage that needs to be opened
        Stage stageToOpen = _stages[_currentStageIdx];

        if (stageToOpen != null)
        {
            //destroy every object that is seen as an exit
            foreach (GameObject exit in stageToOpen.Exits)
            {
                Destroy(exit);
            }
        }
    }

    private void EnableStage()
    {
        //increase stage index and check if it is the last stage
        if (++_currentStageIdx == _stages.Count - 1)
            _isLastStage = true;

        //get stage that needs to be enabled
        Stage stageToEnable = _stages[_currentStageIdx];

        if (stageToEnable != null)
        {
            //enable every object that needs to have something enabled in that stage
            foreach (GameObject objectToEnable in stageToEnable.Enables)
            {
                if (objectToEnable != null)
                {
                    //check if object is the town hall
                    TownHall townHall = objectToEnable.GetComponent<TownHall>();

                    if (townHall != null)
                    {
                        EnableTownHall(townHall);

                        continue;
                    }

                    //check if object is a barrack
                    Barrack barrack = objectToEnable.GetComponent<Barrack>();

                    if (barrack != null)
                    {
                        EnableBarrack(barrack);

                        continue;
                    }

                    //check if object is a projectile defence
                    ProjectileDefence projectileDefence = objectToEnable.GetComponent<ProjectileDefence>();

                    //if so, check if it is destroyed
                    if (projectileDefence != null)
                    {
                        EnableProjectileDefence(projectileDefence);

                        continue;
                    }

                    //check if object is a npc defence
                    NPCDefence npcDefence = objectToEnable.GetComponent<NPCDefence>();

                    //if so, check if it is destroyed
                    if (npcDefence != null)
                    {
                        EnableNPCDefence(npcDefence);

                        continue;
                    }

                    //check if object is a wall
                    Walls walls = objectToEnable.GetComponent<Walls>();

                    if (walls != null)
                    {
                        EnableWalls(walls);

                        continue;
                    }
                }
            }
        }
    }
    private void EnableTownHall(TownHall townHall)
    {
        //enable town hall health
        Health health = townHall.GetComponent<Health>();

        if (health != null)
            health.enabled = true;
    }
    private void EnableBarrack(Barrack barrack)
    {
        //enable barrack spawning enemies
        barrack.enabled = true;

        //enable barrack health
        Health health = barrack.GetComponent<Health>();

        if (health != null)
            health.enabled = true;
    }
    private void EnableProjectileDefence(ProjectileDefence projectileDefence)
    {
        //enable projectile defence
        projectileDefence.enabled = true;

        //enable projectile defence health
        Health health = projectileDefence.GetComponent<Health>();

        if (health != null)
            health.enabled = true;
    }
    private void EnableNPCDefence(NPCDefence npcDefence)
    {
        //enable NPC defence
        npcDefence.enabled = true;

        //enable NPC defence health
        Health health = npcDefence.GetComponent<Health>();

        if (health != null)
            health.enabled = true;
    }
    private void EnableWalls(Walls walls)
    {
        //enable wall health
        Health health = walls.GetComponent<Health>();

        if (health != null)
            health.enabled = true;
    }
}
