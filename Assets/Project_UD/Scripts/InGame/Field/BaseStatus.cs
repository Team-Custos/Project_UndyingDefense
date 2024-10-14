using System.Collections;
using UnityEngine;

public class BaseStatus : MonoBehaviour
{
    public int BaseHPMax = 0;
    public int BaseHPCur = 0;

    public static BaseStatus instance;

    private void Awake()
    {
        instance = this;
    }
    AudioSource audioSource;
    public AudioClip[] BaseHitSound;


    // Start is called before the first frame update
    void Start()
    {
        BaseHPCur = BaseHPMax;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseHPCur <= 0)
        {
            BaseHPCur = 0;

            Time.timeScale = 0.0f;

            //�й�ó��
            Ingame_UIManager.instance.waveResultPanel.SetActive(true);
        }
        
    }

    public void ReceiveDamage(int Damage)
    {
        IEnumerator HitEffect()
        {
            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 255);

            yield return new WaitForSeconds(0.1f);

            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 255);
        }

        StartCoroutine(HitEffect());


        int HitSoundRandomNum = Random.Range(0, 2);
        audioSource.clip = BaseHitSound[HitSoundRandomNum];
        audioSource.Play();
            
        BaseHPCur -= Damage;

       
    }

   

}
