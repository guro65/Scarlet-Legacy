using UnityEngine;

public class SeguirUltimaPosicao : MonoBehaviour
{
    [SerializeField] private float velocidade = 5f; // Velocidade ajustável no editor
    private Vector3 posicaoDestino; // Armazena a posição do "Player 1" no momento em que este GameObject foi criado
    private bool movendo = false; // Controle para iniciar o movimento

    void Start()
    {
        // Encontra o GameObject com a tag "Player 1"
        GameObject player = GameObject.FindGameObjectWithTag("Player 2");
        if (player != null)
        {
            // Armazena a posição atual do "Player 1"
            posicaoDestino = player.transform.position;
            movendo = true; // Inicia o movimento
        }
        else
        {
            Debug.LogError("Nenhum GameObject com a tag 'Player 2' foi encontrado.");
        }
    }

    void Update()
    {
        // Move para a posição armazenada, se necessário
        if (movendo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoDestino, Time.deltaTime * velocidade);

            // Para o movimento ao alcançar a posição
            if (transform.position == posicaoDestino)
            {
                movendo = false;
            }
        }
    }
}
