using UnityEngine;

public class MejoraChouseController : MonoBehaviour
{
    public void MasPinchos()
    {
        GameManager.Instance.playerSO.maximumPlayablePinchos++;
        GameManager.Instance.LoadScene("NextState");
    }
    public void MasMano()
    {
        GameManager.Instance.playerSO.handSize++;
        GameManager.Instance.LoadScene("NextState");
    }
}
