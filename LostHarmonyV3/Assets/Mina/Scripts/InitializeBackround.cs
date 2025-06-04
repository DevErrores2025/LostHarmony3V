using UnityEngine;

public class InitializeBackround : MonoBehaviour
{
    public Transform Player;
    public BackroundMain BackgroundMain;

    private void Awake() =>
      BackgroundMain.Construct(Player);
}
