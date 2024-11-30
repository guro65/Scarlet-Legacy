using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Prefab a ser exibido")]
    public GameObject prefab;

    [Header("Local de Spawn")]
    public Transform spawnPoint;

    private GameObject spawnedPrefab;

    // Refer�ncias est�ticas para os personagens selecionados
    private static GameObject selectedPlayerPrefab;
    private static GameObject selectedOpponentPrefab;

    public static bool IsSelectionComplete => selectedPlayerPrefab != null && selectedOpponentPrefab != null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mostra o prefab enquanto o mouse est� sobre a imagem
        if (prefab != null && spawnPoint != null && spawnedPrefab == null)
        {
            spawnedPrefab = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Remove o prefab quando o mouse sai da imagem, se ele n�o foi selecionado
        if (spawnedPrefab != null && !IsPrefabSelected(spawnedPrefab))
        {
            Destroy(spawnedPrefab);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Sele��o l�gica
        if (selectedPlayerPrefab == null)
        {
            selectedPlayerPrefab = spawnedPrefab;
            Debug.Log("Player selecionado.");
        }
        else if (selectedOpponentPrefab == null && selectedPlayerPrefab != spawnedPrefab)
        {
            selectedOpponentPrefab = spawnedPrefab;
            Debug.Log("Oponente selecionado.");
        }
        else if (selectedPlayerPrefab == spawnedPrefab)
        {
            // Cancela sele��o do Player
            selectedPlayerPrefab = null;
            Debug.Log("Sele��o do Player cancelada.");
        }
        else if (selectedOpponentPrefab == spawnedPrefab)
        {
            // Cancela sele��o do Oponente
            selectedOpponentPrefab = null;
            Debug.Log("Sele��o do Oponente cancelada.");
        }
    }

    private bool IsPrefabSelected(GameObject prefabToCheck)
    {
        return prefabToCheck == selectedPlayerPrefab || prefabToCheck == selectedOpponentPrefab;
    }

    public static void TransferCharactersToScene(string sceneName)
    {
        if (IsSelectionComplete)
        {
            DontDestroyOnLoad(selectedPlayerPrefab);
            DontDestroyOnLoad(selectedOpponentPrefab);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Ambos os personagens precisam ser selecionados antes de mudar de cena!");
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Encontra os pontos de spawn na pr�xima cena
        Transform spawn1 = GameObject.FindWithTag("Spawn1")?.transform;
        Transform spawn2 = GameObject.FindWithTag("Spawn2")?.transform;

        if (spawn1 != null && selectedPlayerPrefab != null)
        {
            selectedPlayerPrefab.transform.position = spawn1.position;
            selectedPlayerPrefab.transform.rotation = spawn1.rotation;
        }

        if (spawn2 != null && selectedOpponentPrefab != null)
        {
            selectedOpponentPrefab.transform.position = spawn2.position;
            selectedOpponentPrefab.transform.rotation = spawn2.rotation;
        }

        // Desassocia o evento para evitar m�ltiplas chamadas
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
