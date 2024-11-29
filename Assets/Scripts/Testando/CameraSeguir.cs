using UnityEngine;

public class CameraSeguirPlayer : MonoBehaviour
{
    [Header("Configurações de Alinhamento")]
    [Tooltip("O Transform do player que a câmera deve seguir.")]
    public Transform player;

    [Tooltip("Distância da câmera em relação ao player.")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("Configurações de Suavização")]
    [Tooltip("Velocidade de suavização da movimentação da câmera.")]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("O player não foi atribuído à câmera. Por favor, arraste o Transform do player para o campo correspondente.");
            return;
        }

        // Calcula a posição desejada
        Vector3 desiredPosition = player.position + offset;

        // Suaviza a transição da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplica a posição suavizada
        transform.position = smoothedPosition;

        // Mantém a câmera olhando para o player
        transform.LookAt(player);
    }
}
