using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int time_limit_in_seconds;
    public Text timerText;

    public int score;
    public Text score_ui;
    public Text level_ui;
    public Text player_health;

    public Text player_health_ui;

    public EnemyController enemy01;

    public GameObject character01;
    public GameObject character02;

    public ItemSlot[] slots;
    public Text[] remaining_use_ui;

    public CinemachineTargetGroup target_group;
    
    private GameObject active_chatacter;
    private CustomCC.CharaterController active_character_controller;

    private void Start()
    {
        StartCoroutine(Timer(time_limit_in_seconds));
        character01.SetActive(false);
        character02.SetActive(false);
        score = 0;

        active_chatacter = character01;

        if (PersistentManagerScript.Instance)
        {

            level_ui.text = PersistentManagerScript.Instance.current_level.ToString();

            PersistentManagerScript.Instance.score = score;
            PersistentManagerScript.Instance.item_used = new Dictionary<string, int>();

            switch (PersistentManagerScript.Instance.selected_character.character_type)
            {
                case "default_man01":
                    active_chatacter = character01;
                    break;
                case "powerful_man01":
                    active_chatacter = character02;
                    break;
                default:
                    active_chatacter = character01;
                    break;
            }


            for (int i = 0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if (PersistentManagerScript.Instance.player_inventory[i].item_id == "attack_up")
                {
                    slots[0].SetSlotItem(
                        PersistentManagerScript.Instance.player_inventory[i].img_path,
                        PersistentManagerScript.Instance.player_inventory[i].name,
                        PersistentManagerScript.Instance.player_inventory[i].remaining_uses
                        );
                }
                else if (PersistentManagerScript.Instance.player_inventory[i].item_id == "defense_up")
                {
                    slots[1].SetSlotItem(
                        PersistentManagerScript.Instance.player_inventory[i].img_path,
                        PersistentManagerScript.Instance.player_inventory[i].name,
                        PersistentManagerScript.Instance.player_inventory[i].remaining_uses
                        );
                }
                else if (PersistentManagerScript.Instance.player_inventory[i].item_id == "speed_up")
                {
                    slots[2].SetSlotItem(
                        PersistentManagerScript.Instance.player_inventory[i].img_path,
                        PersistentManagerScript.Instance.player_inventory[i].name,
                        PersistentManagerScript.Instance.player_inventory[i].remaining_uses
                        );
                }
                else if (PersistentManagerScript.Instance.player_inventory[i].item_id == "health_potion")
                {
                    slots[3].SetSlotItem(
                        PersistentManagerScript.Instance.player_inventory[i].img_path,
                        PersistentManagerScript.Instance.player_inventory[i].name,
                        PersistentManagerScript.Instance.player_inventory[i].remaining_uses
                        );
                }
            }
       }
        active_chatacter.SetActive(true);
        //target_group = new CinemachineTargetGroup();
        target_group.AddMember(active_chatacter.transform, 1, 5);
        target_group.AddMember(enemy01.transform, 1, 5);
        active_character_controller = active_chatacter.GetComponent<CustomCC.CharaterController>();
        enemy01.player = active_chatacter.transform;
        enemy01.player_animator = active_chatacter.GetComponent<Animator>();
    }

    IEnumerator Timer(int time_limit)
    {
        while (time_limit > 0)
        {
            time_limit -= 1;
            timerText.text = time_limit.ToString();
            yield return new WaitForSeconds(1f);
        }

        GameOver();

    }
    void Update()
    {
        score_ui.text = score.ToString();
        if (active_character_controller)
            player_health.text = active_character_controller.health.ToString();
        if (PersistentManagerScript.Instance)
        {

            for (int i = 0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if (PersistentManagerScript.Instance.player_inventory[i].item_id == "attack_up")
                {
                    remaining_use_ui[0].text = PersistentManagerScript.Instance.player_inventory[i].remaining_uses.ToString();
                }
                if (PersistentManagerScript.Instance.player_inventory[i].item_id == "defense_up")
                {
                    remaining_use_ui[1].text = PersistentManagerScript.Instance.player_inventory[i].remaining_uses.ToString();
                }
                if (PersistentManagerScript.Instance.player_inventory[i].item_id == "speed_up")
                {
                    remaining_use_ui[2].text = PersistentManagerScript.Instance.player_inventory[i].remaining_uses.ToString();

                }
                if (PersistentManagerScript.Instance.player_inventory[i].item_id == "health_potion")
                {
                    remaining_use_ui[3].text = PersistentManagerScript.Instance.player_inventory[i].remaining_uses.ToString();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            UseAttackUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            UseDefenseUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            UseSpeedUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            UseHealthPotion();
        }
    }

    public void UseAttackUp()
    {
        if (PersistentManagerScript.Instance)
        {
            for(int i=0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if(PersistentManagerScript.Instance.player_inventory[i].item_id == "attack_up" && PersistentManagerScript.Instance.player_inventory[i].remaining_uses>0)
                {
                    PersistentManagerScript.Instance.player_inventory[i].remaining_uses--;
                    active_character_controller.UseAttackUp();
                    try
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] += 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] = 0;
                    }
                    break;
                }
            }
        }
    }
    
    public void UseDefenseUp()
    {
        if (PersistentManagerScript.Instance)
        {
            for(int i=0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if(PersistentManagerScript.Instance.player_inventory[i].item_id == "defense_up" && PersistentManagerScript.Instance.player_inventory[i].remaining_uses>0)
                {
                    PersistentManagerScript.Instance.player_inventory[i].remaining_uses--;
                    active_character_controller.UseDefenseUp();
                    try
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] += 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] = 0;
                    }
                    break;
                }
            }
        }
    }
    
    public void UseSpeedUp()
    {
        if (PersistentManagerScript.Instance)
        {
            for(int i=0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if(PersistentManagerScript.Instance.player_inventory[i].item_id == "speed_up" && PersistentManagerScript.Instance.player_inventory[i].remaining_uses>0)
                {
                    PersistentManagerScript.Instance.player_inventory[i].remaining_uses--;
                    active_character_controller.UseSpeedUp();
                    try
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] += 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] = 0;
                    }
                    break;
                }
            }
        }
    }
    
    public void UseHealthPotion()
    {
        if (PersistentManagerScript.Instance)
        {
            for(int i=0; i < PersistentManagerScript.Instance.player_inventory.Count; i++)
            {
                if(PersistentManagerScript.Instance.player_inventory[i].item_id == "health_potion" && PersistentManagerScript.Instance.player_inventory[i].remaining_uses>0)
                {
                    PersistentManagerScript.Instance.player_inventory[i].remaining_uses--;
                    active_character_controller.DrinkPotion();
                    try
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] += 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        PersistentManagerScript.Instance.item_used[PersistentManagerScript.Instance.player_inventory[i].item_instance_id] = 0;
                    }
                    break;
                }
            }
        }
    }

    public void AddScore(int s)
    {
        score += s;
    }

    public void GameOver()
    {
        if (PersistentManagerScript.Instance)
        {

            PersistentManagerScript.Instance.score = score;
        }
        LoadSceneManager l = FindObjectOfType<LoadSceneManager>();
        if (l != null)
        {
            l.GoToGameOverScene();
        }
    }
}
