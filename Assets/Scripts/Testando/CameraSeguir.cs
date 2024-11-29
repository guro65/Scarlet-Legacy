using UnityEngine;

public class CameraSeguirPlayer : MonoBehaviour
{
    [Header("Configura��es de Alinhamento")]
    [Tooltip("O Transform do player que a c�mera deve seguir.")]
    public Transform player;

    [Tooltip("Dist�ncia da c�mera em rela��o ao player.")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("Configura��es de Suaviza��o")]
    [Tooltip("Velocidade de suaviza��o da movimenta��o da c�mera.")]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("O player n�o foi atribu�do � c�mera. Por favor, arraste o Transform do player para o campo correspondente.");
            return;
        }

        // Calcula a posi��o desejada
        Vector3 desiredPosition = player.position + offset;

        // Suaviza a transi��o da c�mera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplica a posi��o suavizada
        transform.position = smoothedPosition;

        // Mant�m a c�mera olhando para o player
        transform.LookAt(player);
    }
}
