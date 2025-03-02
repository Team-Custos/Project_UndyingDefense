using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GlobalSoundManager : MonoBehaviour
{
    public static GlobalSoundManager instance;

    public enum lobbySfx
    {
        sfx_click,
        sfx_unableClick,
        sfx_battleStart,
        sfx_commanderSkillSelect,
        sfx_commanderSkillEquip,
        sfx_commanderSkillUnequip,
        sfx_bookOpen,
        sfx_bookClose,
        sfx_exit,
        sfx_gameStart,
    }



    public enum backsfx
    {

    }

    [SerializeField] AudioClip[] lobbySfxClip;
    [SerializeField] AudioSource lobbySfxSource;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void PlayLobbySFX(lobbySfx lobbysfx)
    {
        lobbySfxSource.PlayOneShot(lobbySfxClip[(int)lobbysfx]);
    }
}