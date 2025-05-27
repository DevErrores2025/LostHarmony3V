using UnityEngine;

public class CheckPosition : MonoBehaviour
{
    private Transform player;
    private Vector2 bounds;
    private bool nineImage;

    public void Construct(Transform player, bool nineImage)
    {
        this.player = player;
        this.nineImage = nineImage;
        bounds = GetComponent<SpriteRenderer>().sprite.bounds.size;
    }

    public void UpdateStatus()
    {
        var playerPosition = player.transform.position;
        var position = transform.position;

        CheckPositionByX(position, playerPosition);
        if (nineImage)
            CheckPositionByY(position, playerPosition);
    }

    private void CheckPositionByY(Vector3 position, Vector3 playerPosition)
    {
        if (position.y < playerPosition.y - 1.5f * bounds.y)
            transform.position = new Vector2(position.x, position.y + bounds.y * 3);
        else if (position.y > playerPosition.y + bounds.y * 1.5f)
            transform.position = new Vector2(position.x, position.y - bounds.y * 3);
    }

    private void CheckPositionByX(Vector3 position, Vector3 playerPosition)
    {
        if (position.x < playerPosition.x - 1.5f * bounds.x)
            transform.position = new Vector2(position.x + bounds.x * 3, position.y);
        else if (position.x > playerPosition.x + bounds.x * 1.5f)
            transform.position = new Vector2(position.x - bounds.x * 3, position.y);
    }
}

