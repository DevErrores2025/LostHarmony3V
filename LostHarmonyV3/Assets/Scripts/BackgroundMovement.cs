using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public Vector2 Speed;  // Controla la velocidad del parallax (X e Y)
    public List<CheckPosition> CheckPosition = new List<CheckPosition>();

    public Vector3 InitialOffset = Vector3.zero;  // 🎯 Offset inicial configurable desde el Inspector

    private Transform player;

    public void Construct(Transform player, bool nineImage)
    {
        this.player = player;

        // Aquí usamos el offset que elijas en Unity (no el transform.position)
        foreach (var checkInBound in CheckPosition)
            checkInBound.Construct(player, nineImage);
    }

    public void UpdateMovement()
    {
        var position = player.position;
        // Posición del fondo = offset inicial + movimiento del jugador (parallax)
        transform.position = InitialOffset + new Vector3(position.x * Speed.x, position.y * Speed.y, 0);

        foreach (var checkPosition in CheckPosition)
            checkPosition.UpdateStatus();
    }
}
