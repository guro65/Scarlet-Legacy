using UnityEngine;

public class Combate : MonoBehaviour
{
    private Animator animator; // Referência ao Animator do personagem
    private bool alternarSoco = true; // Controle para alternar entre socos

    void Start()
    {
        // Obtém o componente Animator do GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado! Certifique-se de que o componente Animator está anexado.");
        }
    }

    void Update()
    {
        // Verifica se a tecla "H" foi pressionada
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (alternarSoco)
            {
                // Ativa a animação de SocoEsquerdo
                animator.SetTrigger("SocoEsquerdo");
            }
            else
            {
                // Ativa a animação de SocoDireito
                animator.SetTrigger("SocoDireito");
            }

            // Alterna para o próximo soco
            alternarSoco = !alternarSoco;
        }
    }
}
