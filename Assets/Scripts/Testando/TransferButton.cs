using UnityEngine;
using UnityEngine.UI;

public class TransferButton : MonoBehaviour
{
    [Header("Nome da Cena para Transferir")]
    public string targetSceneName;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTransferButtonClick);
        }
    }

    private void OnTransferButtonClick()
    {
        CharacterSelection.TransferCharactersToScene(targetSceneName);
    }
}
