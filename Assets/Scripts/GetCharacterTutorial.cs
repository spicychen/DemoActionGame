using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCharacterTutorial : MonoBehaviour
{
    public PlayerProfile playerProfile;

    public GameObject step1;
    public GameObject step2;
    public GameObject step3;
    //public GameObject complete_message;

    public Animator playerProfileAnimator;

    public int current_step = 0;

    // Start is called before the first frame update
    void Start()
    {
        step1.SetActive(false);
        step2.SetActive(false);
        step3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerProfile && current_step<7)
        {
            if (playerProfile.player_characters.Count < 1)
            {
                if (playerProfile.player_inventory.Count > 0)
                {

                    if (current_step == 0)
                    {
                        step2.SetActive(false);
                        current_step = 2;
                        foreach (Item i_i in playerProfile.player_inventory)
                        {
                            if (i_i.item_id == "newbee_gift")
                            {
                                step1.SetActive(true);
                                current_step = 1;
                                //open_gift.gameObject.SetActive(true);
                                break;
                            }
                        }
                    }
                }
                if (current_step == 2)
                {
                    step1.SetActive(false);
                    if (playerProfileAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                    {
                        current_step = 3;
                        step2.SetActive(true);
                    }
                    else
                    {
                        current_step = 4;
                    }
                }
                else if (current_step == 4)
                {
                    step1.SetActive(false);
                    step2.SetActive(false);
                    step3.SetActive(true);
                    current_step = 5;

                } 
            }
            if (current_step == 6)
                {
                step3.SetActive(false);
                FindObjectOfType<MessageWindow>().ShowSuccess("Congraduration! You got a Character, You can now play the game.");
                current_step = 7;
                }

        }
    }

    public void CompleteStep1()
    {
        if(current_step==1)
        current_step = 2;
    }

    public void CompleteStep2()
    {
        if(current_step==3)
        current_step = 4;
    }

    public void CompleteStep3()
    {
        if(current_step==5)
        current_step = 6;
    }
}
