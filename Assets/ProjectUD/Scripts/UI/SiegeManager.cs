using UnityEngine;

public class SiegeManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem siegeParticle;
    [SerializeField] private GameObject siegeEffect;

    public void ActivateSiegeMode()
    {
        if (siegeParticle != null)
        {
            siegeParticle.gameObject.SetActive(true);
            siegeParticle.Play();

            Invoke(nameof(EnableSiegeEffect), siegeParticle.main.duration);
        }
    }

    public void EnableSiegeEffect()
    {
        if (siegeParticle != null)
        {
            siegeParticle.Stop();
            siegeParticle.gameObject.SetActive(false);
        }

        if (siegeEffect != null)
        {
            siegeEffect.SetActive(true);
        }
    }
}
