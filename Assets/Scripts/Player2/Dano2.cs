using UnityEngine;

public class Dano2 : MonoBehaviour
{
    [SerializeField] private float dano = 10f; // Dano causado ao player

    // Método chamado quando há uma colisão com outro GameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que colidiu possui a tag "Player 2"
        if (collision.gameObject.CompareTag("Player 2"))
        {
            // Tenta obter o script Player do objeto que colidiu
            Player2 playerScript = collision.gameObject.GetComponent<Player2>();

            if (playerScript != null)
            {
                // Aplica o dano ao Player chamando o método ReceberDano
                playerScript.ReceberDano((int)dano);
                Debug.Log($"Causou {dano} de dano ao Player!");
            }
            else
            {
                Debug.LogError("O GameObject com a tag 'Player 2' não possui o script 'Player'.");
            }
        }
    }
}
