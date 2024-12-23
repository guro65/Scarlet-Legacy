using UnityEngine;

public class SeguirUltimaPosicao2 : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f; // Velocidade ajust�vel no editor
    private Vector3 posicaoDestino; // Armazena a posi��o do "Player" no momento em que este GameObject foi criado
    private bool movendo = false; // Controle para iniciar o movimento

    void Start()
    {
        // Encontra o GameObject com a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player 1");
        if (player != null)
        {
            // Armazena a posi��o atual do "Player 2"
            posicaoDestino = player.transform.position;
            movendo = true; // Inicia o movimento
        }
        else
        {
            Debug.LogError("Nenhum GameObject com a tag 'Player 1' foi encontrado.");
        }
    }

    void Update()
    {
        // Move para a posi��o armazenada, se necess�rio
        if (movendo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoDestino, Time.deltaTime * velocidade);

            // Para o movimento ao alcan�ar a posi��o
            if (transform.position == posicaoDestino)
            {
                movendo = false;
            }
        }
    }
}
