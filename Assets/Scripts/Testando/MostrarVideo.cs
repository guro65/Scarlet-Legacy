using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class MostrarVideo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private VideoPlayer videoPlayer; // Referência ao VideoPlayer
    [SerializeField] private GameObject videoContainer; // GameObject que contém o RawImage para o vídeo

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (videoPlayer != null && videoContainer != null)
        {
            videoContainer.SetActive(true); // Exibe o vídeo
            videoPlayer.Play(); // Reproduz o vídeo
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (videoPlayer != null && videoContainer != null)
        {
            videoPlayer.Stop(); // Para o vídeo
            videoContainer.SetActive(false); // Oculta o vídeo
        }
    }
}
