using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int VidaAtual { get { return vidaAtual; } }
    public int VidaMaxima { get { return vida; } }
    [SerializeField] private int vidaAtual;
    [SerializeField] private int vida = 100;
    [SerializeField] private float ataque;
    [SerializeField] private float velocidade;
    [SerializeField] private Animator animator;
    [SerializeField] private bool estaVivo = true;
    [SerializeField] private float velocidadeRotacao = 5f; // Velocidade de suavidade da rotação
    private Rigidbody rb;
    private float rotacaoAtual = 0f; // Rotação do personagem
    private bool defendendo = false;

    // Variáveis para detectar duplo toque
    private float ultimoToqueW = -1f, ultimoToqueA = -1f, ultimoToqueS = -1f, ultimoToqueD = -1f;
    private float intervaloDuploToque = 0.5f; // Tempo máximo entre os toques
    private int contadorToquesW = 0, contadorToquesA = 0, contadorToquesS = 0, contadorToquesD = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        vidaAtual = vida;
    }

    void Update()
    {
        DetectarDuploToque();
        VerificarMovimento();
        RotacaoContinua(); // Chamando o método para rotação contínua
    }

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
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("AndarParaTras", true);
            animator.SetBool("Andar", false);
            Walk(-1); // Anda para trás
        }
        else if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("Andar", true);
            Walk(1); // Anda para a esquerda
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("Andar", true);
            Walk(1); // Anda para a direita
        }
        else
        {
            animator.SetBool("Andar", false);
            animator.SetBool("AndarParaTras", false);
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
}
