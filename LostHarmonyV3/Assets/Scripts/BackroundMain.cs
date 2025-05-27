using System.Collections.Generic;
using UnityEngine;

public class BackroundMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool NineImage;
    public List<BackgroundMovement> BackgroundMovements = new List<BackgroundMovement>();

    public void Construct(Transform player)
    {
        transform.position = player.position;

        foreach (var backgroundMovement in BackgroundMovements)
            backgroundMovement.Construct(player, NineImage);
    }

    public void Update()
    {
        foreach (var backgroundMovement in BackgroundMovements)
            backgroundMovement.UpdateMovement();
    }
}
