using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance => instance;

    [Header("■ Singleton Option")]
    public bool IsPrime; // 이 싱글톤 객체가 가장 우선인가?

    // IsPrime : 씬 전환 시 파괴되지 않음. 다른 씬의 싱글톤 객체로 대체되지 않음. 생성 이후 항상 존재하고 교체되지 않는 싱글톤.
    // !IsPrime : 씬 전환 시 파괴. 다른 씬의 싱글톤 객체로 대체됨.

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this.GetComponent<T>();
        }
        else
        {
            if(instance != this) // Instance와 현재 객체와 다를 경우
            {
                // Instance 객체 파괴.
                if(instance.IsPrime && this.IsPrime) // 둘 다 Prime일 경우
                {
                    Debug.LogError("Prime인 싱글톤 객체가 2개 이상 존재합니다.");
                    Destroy(this.gameObject);
                }
                else if(instance.IsPrime && !this.IsPrime) // 이미 로드된 Instance가 Prime일 때
                {
                    // 자신의 객체 파괴.
                    Destroy(this.gameObject);
                }
                else // 자신이 Prime이거나 둘 다 Prime이 아닐 때
                {
                    // Instance 교체
                    Destroy(instance.gameObject);
                    instance = this.GetComponent<T>();
                }
            }
        }

        if (IsPrime)
            DontDestroyOnLoad(this.gameObject);
    }
}
