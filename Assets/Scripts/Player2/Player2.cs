using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2 : MonoBehaviour
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
    [SerializeField] private Image barraVida; // Referência à barra de vida


    // Variáveis para detectar duplo toque
    private float ultimoToqueUpArrow = -1f, ultimoToqueLeftArrow = -1f, ultimoToqueRightArrow = -1f, ultimoToqueDownArrow = -1f;
    private float intervaloDuploToque = 0.5f; // Tempo máximo entre os toques
    private int contadorToquesUpArrow = 0, contadorToquesLeftArrow = 0, contadorToquesRightArrow = 0, contadorToquesDownArrow = 0;

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
        if (!estaVivo) return; // Impede a execução de lógica de movimento se o jogador estiver morto

        DetectarDefesa();
        DetectarDuploToque();
        VerificarMovimento();
        RotacaoContinua();
        AtualizarBarraVida();
    }
    private void AtualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = VidaAtualPercentual / 100f; // A barra é uma porcentagem da vida
        }
    }


    private void DetectarDefesa()
    {
        // Verifica se a tecla "F" está sendo pressionada
        if (Input.GetKey(KeyCode.Keypad0))
        {
            if (!defendendo)
            {
                defendendo = true;
                animator.SetTrigger("Defender");
                Debug.Log("Defesa ativada!");
            }
        }
        else
        {
            if (defendendo)
            {
                defendendo = false;
                animator.SetBool("EstaParado", true); // Volta para a animação de parada
                Debug.Log("Defesa desativada!");
            }
        }
    }


    void DetectarDuploToque()
    {
        // Detecção de duplo toque para cada direção
        DetectarTeclaDupla(KeyCode.UpArrow, ref ultimoToqueUpArrow, ref contadorToquesUpArrow, "RolarParaFrente");
        DetectarTeclaDupla(KeyCode.LeftArrow, ref ultimoToqueLeftArrow, ref contadorToquesLeftArrow, "RolarParaEsquerda");
        DetectarTeclaDupla(KeyCode.RightArrow, ref ultimoToqueRightArrow, ref contadorToquesRightArrow, "RolarParaTras");
        DetectarTeclaDupla(KeyCode.DownArrow, ref ultimoToqueDownArrow, ref contadorToquesDownArrow, "RolarParaDireita");
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("Andar", true);
            animator.SetBool("AndarParaTras", false);
            Walk(1); // Anda para frente
            animator.SetBool("EstaParado", false);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
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
            rotacaoAtual -= 90 * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotacaoAtual += 90 * Time.deltaTime;
        }

        Quaternion targetRotation = Quaternion.Euler(0, rotacaoAtual, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * velocidadeRotacao);
    }

    private void Walk(float direcao)
    {
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
        AtualizarBarraVida(); // Atualiza a barra de vida
        Debug.Log($"Regenerou {vidaParaAdicionar} de vida! Vida atual: {VidaAtualPercentual}%");
    }


    public void AumentarVida(float valor)
    {
        vidaAtual = Mathf.Clamp(vidaAtual + valor, 0, VidaMaxima);
        AtualizarBarraVida(); // Atualiza a barra de vida
    }



    public void TomarDano(float dano)
    {
        if (!estaVivo) return;

        vidaAtual = Mathf.Max(vidaAtual - dano, 0);
        AtualizarBarraVida();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }


    public void ReceberDano(int dano)
    {
        if (vidaAtual > 0)
        {
            // Reduz o dano pela metade se o jogador estiver defendendo
            if (defendendo)
            {
                dano = Mathf.CeilToInt(dano / 2f);
                Debug.Log($"Dano reduzido pela metade: {dano}");
            }

            vidaAtual -= dano;
            vidaAtual = Mathf.Max(vidaAtual, 0); // Garante que a vida não fique negativa
            Debug.Log($"Player recebeu {dano} de dano! Vida atual: {vidaAtual}");

            if (vidaAtual <= 0)
            {
                Morrer();
            }
        }
    }

    private void Morrer()
    {
        estaVivo = false; // Define que o jogador não está mais vivo
        animator.SetTrigger("Morte"); // Ativa a animação de morte
        rb.linearVelocity = Vector3.zero; // Para o movimento
        animator.SetBool("EstaParado", true); // Assegura que o personagem fique parado

        // Desabilita o movimento e qualquer outra ação
        this.enabled = false;

        // Inicia a corrotina para destruir o objeto após a animação de morte
        StartCoroutine(AguardarDestruicao());
    }

    private IEnumerator AguardarDestruicao()
    {
        // Aguarda o tempo da animação de morte (substitua 2f pelo tempo correto da animação)
        yield return new WaitForSeconds(2f);

        // Destrói o GameObject após a animação
        Destroy(gameObject);
    }

}