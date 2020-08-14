using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CharacterSelection : MonoBehaviour
{
    public ItemSlot[] character_slot_group;
    public PlayerProfile playerProfile;

    public PlayFabHelper playfab_helper;

    public LoadSceneManager l;

    public void SetItems()
    {
        if(playerProfile.player_characters.Count <= character_slot_group.Length)
        {

            for (int x = 0; x < character_slot_group.Length; x++)
            {
                if (x < playerProfile.player_characters.Count)
                {
                    int i = x;
                    character_slot_group[i].SetSlotItem(playerProfile.player_characters[i].img_path, playerProfile.player_characters[i].character_type, 1);
                    character_slot_group[i].SetClickable("Start Game?", () => {
                        PersistentManagerScript.Instance.selected_character = playerProfile.player_characters[i];
                        playfab_helper.StartGame(
                            PersistentManagerScript.Instance.current_level,
                            PersistentManagerScript.Instance.selected_character.character_id,
                            (ExecuteCloudScriptResult result) =>
                            {

                                CloudScriptResult custom_result = JsonUtility.FromJson<CloudScriptResult>(result.FunctionResult.ToString());

                                if (custom_result.status == "Success")
                                {
                                    if (l != null)
                                    {

                                    l.LoadNextScene(PersistentManagerScript.Instance.current_level);
                                    }
                                }
                                else
                                {
                                    FindObjectOfType<MessageWindow>().ShowSuccess(custom_result.message);
                                }
                            }
                            );
                    });
                }
                else
                {
                    character_slot_group[x].HideSlotItem();
                }
            }
        }
    }
}
