using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float VidaAtualPercentual { get { return vidaAtual / vidaMaxima * 100; } } // Vida atual como porcentagem
    public float VidaMaxima { get { return vidaMaxima; } }
    [SerializeField] private float vidaAtual = 100f; // Vida atual em valores absolutos
    [SerializeField] private float vidaMaxima = 100f; // Vida máxima
    [SerializeField] private float ataque;
    [SerializeField] private float velocidade;
    [SerializeField] private Animator animator;
    [SerializeField] private bool estaVivo = true;
    [SerializeField] private float velocidadeRotacao = 5f; // Velocidade de suavidade da rotação
    private Rigidbody rb;
    private float rotacaoAtual = 0f; // Rotação do personagem
    private bool defendendo = false;

    // Referência à barra de vida
    //[SerializeField] private Image barraVida;

    // Variáveis para detectar duplo toque
    private float ultimoToqueW = -1f, ultimoToqueA = -1f, ultimoToqueS = -1f, ultimoToqueD = -1f;
    private float intervaloDuploToque = 0.5f; // Tempo máximo entre os toques
    private int contadorToquesW = 0, contadorToquesA = 0, contadorToquesS = 0, contadorToquesD = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        vidaAtual = vidaMaxima; // Inicializa a vida em 100%

        // Verifica se a barra de vida está configurada no Unity
        /*if (barraVida == null)
        {
            Debug.LogError("A barra de vida não está configurada!");
        }*/
    }

    void Update()
    {
        DetectarDuploToque();
        VerificarMovimento();
        RotacaoContinua(); // Chamando o método para rotação contínua

        // Atualiza a barra de vida
        //AtualizarBarraVida();

        // Debug para mostrar a vida atual em porcentagem
        //Debug.Log($"Vida Atual: {VidaAtualPercentual}%");
    }

    // Atualiza a barra de vida conforme a vida atual do jogador
    /*private void AtualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = VidaAtualPercentual / 100f; // A barra é uma porcentagem da vida
        }
    }*/

    void DetectarDuploToque()
    {
        // Detecção de duplo toque para cada direção
        DetectarTeclaDupla(KeyCode.W, ref ultimoToqueW, ref contadorToquesW, "RolarParaFrente");
        DetectarTeclaDupla(KeyCode.A, ref ultimoToqueA, ref contadorToquesA, "RolarParaEsquerda");
        DetectarTeclaDupla(KeyCode.S, ref ultimoToqueS, ref contadorToquesS, "RolarParaTras");
        DetectarTeclaDupla(KeyCode.D, ref ultimoToqueD, ref contadorToquesD, "RolarParaDireita");
    }

    void DetectarTeclaDupla(KeyCode tecla, ref float ultimoToque, ref int contadorToques, string animacao)
    {
        if (Input.GetKeyDown(tecla))
        {
            float tempoAtual = Time.time;

            if (tempoAtual - ultimoToque <= intervaloDuploToque)
            {
                contadorToques++;
                if (contadorToques == 2)
                {
                    // Aciona a animação correspondente
                    Rolar(animacao);
                    contadorToques = 0; // Reseta o contador
                }
            }
            else
            {
                // Reinicia o contador se o tempo exceder o intervalo
                contadorToques = 1;
            }

            ultimoToque = tempoAtual;
        }
    }

    void Rolar(string animacao)
    {
        if (animator != null)
        {
            animator.SetBool(animacao, true); // Ativa a animação
            StartCoroutine(DesativarRolagem(animacao)); // Desativa após um tempo
        }
        else
        {
            Debug.LogWarning("Animator não atribuído no script!");
        }
    }

    IEnumerator DesativarRolagem(string animacao)
    {
        yield return new WaitForSeconds(0.5f); // Tempo da animação de rolamento
        animator.SetBool(animacao, false); // Desativa a animação
    }

    void VerificarMovimento()
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("Andar", true);
            animator.SetBool("AndarParaTras", false);
            Walk(1); // Anda para frente
            animator.SetBool("EstaParado", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("AndarParaTras", true);
            animator.SetBool("Andar", false);
            Walk(-1); // Anda para trás
        }
        else
        {
            animator.SetBool("Andar", false);
            animator.SetBool("AndarParaTras", false);
            animator.SetBool("EstaParado", true);
        }
    }

    void RotacaoContinua()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rotacaoAtual -= 90 * Time.deltaTime; // Decrementa a rotação
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotacaoAtual += 90 * Time.deltaTime; // Incrementa a rotação
        }

        // Aplica a rotação suavemente no personagem
        Quaternion targetRotation = Quaternion.Euler(0, rotacaoAtual, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * velocidadeRotacao);
    }

    private void Walk(float direcao)
    {
        // A direção controla se o movimento será para frente (1) ou para trás (-1)
        float velocidadeMovimento = velocidade * direcao;
        Vector3 moveDirection = transform.forward * velocidadeMovimento;
        Vector3 novaPosicao = rb.position + moveDirection * Time.deltaTime;
        rb.MovePosition(novaPosicao);
    }

    // Método para regenerar a vida em porcentagem
    public void RegenerarVida(float porcentagem)
    {
        if (!estaVivo) return;

        float vidaParaAdicionar = vidaMaxima * porcentagem;
        vidaAtual = Mathf.Min(vidaAtual + vidaParaAdicionar, vidaMaxima);
        Debug.Log($"Regenerou {vidaParaAdicionar} de vida! Vida atual: {VidaAtualPercentual}%");
    }

    public void AumentarVida(float quantidade)
    {
        vidaAtual = Mathf.Min(vidaAtual + quantidade, vidaMaxima); // Garante que não ultrapasse o máximo
    }

    public void TomarDano(float dano)
    {
        vidaAtual = Mathf.Max(vidaAtual - dano, 0); // Garante que a vida não fique abaixo de zero
        Debug.Log($"Tomou {dano} de dano! Vida atual: {VidaAtualPercentual}%");
    }

    public void ReceberDano(int dano)
    {
        if (vidaAtual > 0)
        {
            vidaAtual -= dano;
            vidaAtual = Mathf.Max(vidaAtual, 0); // Garante que a vida não fique negativa
            Debug.Log($"Player recebeu {dano} de dano! Vida atual: {vidaAtual}");

            // Aqui você pode adicionar mais ações, como reproduzir uma animação de dano ou efeito de som
            // Se a vida do jogador chegar a 0, talvez um sistema de morte seja chamado
            if (vidaAtual <= 0)
            {
                Morrer();
            }
        }
    }

    private void Morrer()
    {
        estaVivo = false;
        animator.SetTrigger("Morte"); // Aciona a animação de morte, se existir
                                      // Aqui você pode adicionar outras ações ao morrer, como desativar controles ou exibir uma tela de game over
    }

}
