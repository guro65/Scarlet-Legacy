using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PrefabSpawnerOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Prefab a ser spawnado")]
    public GameObject prefab;

    [Header("Local de Spawn")]
    public Transform spawnPoint;

    [Header("Controle de Spawn")]
    public bool destroyOnExit = true; // Se verdadeiro, destrói o prefab quando o mouse sair da imagem

    private GameObject spawnedPrefab;

    // Chamado quando o mouse entra na Image
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (prefab != null && spawnPoint != null)
        {
            spawnedPrefab = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // Chamado quando o mouse sai da Image
    public void OnPointerExit(PointerEventData eventData)
    {
        if (destroyOnExit && spawnedPrefab != null)
        {
            Destroy(spawnedPrefab);
        }
    }
}
