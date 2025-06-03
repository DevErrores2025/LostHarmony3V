using UnityEngine;

public class ToxicShooter : MonoBehaviour
{
    public GameObject skullPrefab;
    public float shootInterval = 3f;
    public float projectileSpeed = 5f;

    [Range(1, 3)]
    public int numberOfProjectiles = 3; // 1, 2 o 3 proyectiles

    private Vector2[] allDirections = new Vector2[] {
        new Vector2(-1, 1), // izquierda + arriba
        new Vector2(0, 1),  // arriba
        new Vector2(1, 1)   // derecha + arriba
    };

    void Start()
    {
        InvokeRepeating(nameof(ShootProjectiles), 1f, shootInterval);
    }

    void ShootProjectiles()
    {
        int middleIndex = allDirections.Length / 2;

        // Esto asegura que si numberOfProjectiles == 1 se dispare solo el del centro, si 2, los extremos, si 3, todos.
        if (numberOfProjectiles == 1)
        {
            SpawnProjectile(allDirections[1]);
        }
        else if (numberOfProjectiles == 2)
        {
            SpawnProjectile(allDirections[0]);
            SpawnProjectile(allDirections[2]);
        }
        else if (numberOfProjectiles >= 3)
        {
            foreach (Vector2 dir in allDirections)
            {
                SpawnProjectile(dir);
            }
        }
    }

    void SpawnProjectile(Vector2 direction)
    {
        GameObject skull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
        var projectile = skull.GetComponent<SkullProjectile>();
        projectile.direction = direction.normalized;
        projectile.speed = projectileSpeed;
    }
}