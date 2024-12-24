using UnityEngine;

public class Dano2 : MonoBehaviour
{
    [SerializeField] private float dano = 10f; // Dano causado ao player

    // M�todo chamado quando h� uma colis�o com outro GameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto que colidiu possui a tag "Player 2"
        if (collision.gameObject.CompareTag("Player 2"))
        {
            // Tenta obter o script Player do objeto que colidiu
            Player2 playerScript = collision.gameObject.GetComponent<Player2>();

            if (playerScript != null)
            {
                // Aplica o dano ao Player chamando o m�todo ReceberDano
                playerScript.ReceberDano((int)dano);
                Debug.Log($"Causou {dano} de dano ao Player!");
            }
            else
            {
                Debug.LogError("O GameObject com a tag 'Player 2' n�o possui o script 'Player'.");
            }
        }
    }
}
