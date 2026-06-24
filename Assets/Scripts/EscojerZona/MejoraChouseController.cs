using UnityEngine;

public class MejoraChouseController : MonoBehaviour
{

    public AudioClip selectOptions;

    public void MasPinchos()
    {
        GameManager.Instance.PlaySFX(selectOptions);
        GameManager.Instance.playerSO.maximumPlayablePinchos++;
        GameManager.Instance.LoadScene("NextState");
    }
    public void MasMano()
    {
        GameManager.Instance.PlaySFX(selectOptions);
        GameManager.Instance.playerSO.handSize += 2;
        GameManager.Instance.LoadScene("NextState");
    }
}
