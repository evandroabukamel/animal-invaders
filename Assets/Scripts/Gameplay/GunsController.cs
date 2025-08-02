using UnityEngine;

public class GunsController : MonoBehaviour
{
    [SerializeField] ParticleSystem[] shotParticles;

    public void Shoot()
    {
        for (var p = 0; p < shotParticles.Length; p++)
        {
            shotParticles[p].Play();
        }
    }
}
