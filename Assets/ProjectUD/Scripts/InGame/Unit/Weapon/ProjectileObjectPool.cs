using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    private Dictionary<GameObject, Queue<ProjectileCtrl>> projectileDic = new Dictionary<GameObject, Queue<ProjectileCtrl>>();

    public ProjectileCtrl GetProjectile(ProjectileCtrl projectilePrefab)
    {
        GameObject prefabKey = projectilePrefab.gameObject;
        if (!projectileDic.TryGetValue(prefabKey, out Queue<ProjectileCtrl> projectileQueue))
        {
            projectileQueue = new Queue<ProjectileCtrl>();

            projectileDic.Add(prefabKey, projectileQueue);
        }

        ProjectileCtrl projectile;

        if (projectileQueue.Count > 0)
        {
            projectile = projectileQueue.Dequeue();
        }
        else
        {
            projectile = InstantiateProjectile(projectilePrefab);
        }

        projectile.transform.SetParent(null, true);

        return projectile;
    }

    private ProjectileCtrl InstantiateProjectile(ProjectileCtrl projectilePrefab)
    {
        ProjectileCtrl projectile = Instantiate(projectilePrefab, transform);

        projectile.gameObject.SetActive(false);

        return projectile;
    }

    public void ReturnProjectile(ProjectileCtrl projectile)
    {
        
    }

}
