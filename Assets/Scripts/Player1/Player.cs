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
    [SerializeField] private Image barraVida; // Referência à barra de vida


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
        if (!estaVivo) return; // Impede a execução de lógica de movimento se o jogador estiver morto

        DetectarDefesa();
        DetectarDuploToque();
        VerificarMovimento();
        RotacaoContinua();
        AtualizarBarraVida();
    }

   private void DetectarDefesa()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (!defendendo)
            {
                defendendo = true;
                animator.SetTrigger("Defender");
            }
        }
        else
        {
            if (defendendo)
            {
                defendendo = false;
                animator.SetBool("EstaParado", true);
            }
        }
    }


    void DetectarDuploToque()
    {
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
                    Rolar(animacao);
                    contadorToques = 0;
                }
            }
            else
            {
                contadorToques = 1;
            }

            ultimoToque = tempoAtual;
        }
    }

    void Rolar(string animacao)
    {
        if (animator != null)
        {
            animator.SetBool(animacao, true);
            StartCoroutine(DesativarRolagem(animacao));
        }
    }

    IEnumerator DesativarRolagem(string animacao)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(animacao, false);
    }

    void VerificarMovimento()
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("Andar", true);
            Walk(1);
            animator.SetBool("EstaParado", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("AndarParaTras", true);
            Walk(-1);
        }
        else
        {
            animator.SetBool("Andar", false);
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
        Debug.Log($"Regenerou {vidaParaAdicionar} de vida! Vida atual: {VidaAtualPercentual}%");
    }
    private void AtualizarBarraVida()
    {
        if (barraVida != null)
        {
            barraVida.fillAmount = VidaAtualPercentual / 100f;
        }
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