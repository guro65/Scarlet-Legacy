using UnityEngine;

public class Combate : MonoBehaviour
{
    private Animator animator; // Refer�ncia ao Animator do personagem
    private bool alternarSoco = true; // Controle para alternar entre socos

    void Start()
    {
        // Obt�m o componente Animator do GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator n�o encontrado! Certifique-se de que o componente Animator est� anexado.");
        }
    }

    void Update()
    {
        // Verifica se a tecla "H" foi pressionada
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (alternarSoco)
            {
                // Ativa a anima��o de SocoEsquerdo
                animator.SetTrigger("SocoEsquerdo");
            }
            else
            {
                // Ativa a anima��o de SocoDireito
                animator.SetTrigger("SocoDireito");
            }

            // Alterna para o pr�ximo soco
            alternarSoco = !alternarSoco;
        }
    }
}
