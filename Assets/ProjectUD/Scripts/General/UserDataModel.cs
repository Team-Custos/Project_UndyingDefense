using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : MonoBehaviour
{
    [SerializeField] private AudioClip lobbyBgm;

    public static UserDataModel instance;

    public List<string> skillIDs = new List<string>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
       SoundManager.Instance.PlayBGM(lobbyBgm);
    }
}
