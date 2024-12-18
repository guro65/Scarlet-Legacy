using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InimigoBoss : MonoBehaviour
{
    [SerializeField] private int vidaInimigo;
    [SerializeField] private GameObject player;
    [SerializeField] private float distanciaAtaque = 5.0f;
    [SerializeField] private float recargaAtaqueEspecial = 5.0f;
    [SerializeField] private float distanciaDeteccao;
    [SerializeField] private float velocidade = 5.0f;
    [SerializeField] private float velocidadeEspecial = 7.0f;
    [SerializeField] private float tempoAntesDoEspecial = 2.0f;
    private bool estaVivo = true;
    private Animator animator;
    private Vector3 ultimaPosicaoConhecida;
    private bool playerNaAreaDeAtaque;
    private bool podeUsarAtaqueEspecial = true;
    private bool ataqueEspecialAtivo = false;
    private bool ataqueNormalAtivo = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        //Não prossegue se o ataque especial estiver ativo
        if (ataqueEspecialAtivo)
        {
            return;
        }

        // Calcula a distância entre o boss e o jogador
        float distanciaAtePlayer = Vector3.Distance(transform.position, player.transform.position);

        if(estaVivo)
        {
            // Faz o boss olhar sempre na direção do jogador
            OlhaParaOPlayer();

            if (distanciaAtePlayer <= distanciaAtaque)
            {
                //Ataque normal se estiver próximo
                playerNaAreaDeAtaque = true;
                Ataque();
            }
            else if (distanciaAtePlayer <= distanciaDeteccao)
            {
                //Player fora do alcance
                playerNaAreaDeAtaque = false;
                ultimaPosicaoConhecida = player.transform.position;
                MoverAteOPlayer();
            }
            else if (!playerNaAreaDeAtaque && podeUsarAtaqueEspecial)
            {
                //Ataque especial se o player estiver fora do alcance
                AtaqueEspecial();
            }
        }
        
    }

    private void OlhaParaOPlayer()
    {
        // Faz o boss olhar sempre na direção do jogador
        Vector3 direcao = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direcao.x, 0, direcao.z));

        // Slerp faz a rotação de forma suave
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void Ataque()
    {
        animator.SetBool("Andar", false);
        StartCoroutine(AtaqueNormalAtivado());
    }

    IEnumerator AtaqueNormalAtivado()
    {
        yield return new WaitForSeconds(3.0f);
     
        animator.SetTrigger("Ataque");

        StopAllCoroutines();
    }

    private void AtaqueEspecial()
    {
        podeUsarAtaqueEspecial = false;
        ataqueEspecialAtivo = true;

        animator.SetBool("Andar", false);
        animator.SetBool("Especial", true);


        // Ataque especial
        StartCoroutine(MovimentoAtaqueEspecial());
    }

    private void MoverAteOPlayer()
    {
        animator.SetBool("Andar", true);
        animator.SetBool("Especial", false);

        // Move o boss até a última posição conhecida do jogador
        Vector3 direcao = (ultimaPosicaoConhecida - transform.position).normalized;

        transform.position += direcao * velocidade * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisão com " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player") && ataqueEspecialAtivo)
        {
            Atacar(collision.gameObject.GetComponent<Player>(), 30);
        }

        if (collision.gameObject.CompareTag("Player") && playerNaAreaDeAtaque)
        {
            Atacar(collision.gameObject.GetComponent<Player>(), 10);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Arma"))
        {
            TomarDano(10);
        }
    }


    public void TomarDano(int dano)
    {
        vidaInimigo -= dano;
        animator.SetTrigger("Hit");
        animator.SetBool("Andar", false);

        if (vidaInimigo <= 0)
        {
            Morrer();
        }
    }

    public void Morrer()
    {   
        animator.SetBool("EstaVivo", false);
        animator.SetBool("MorteAnima", true);
        estaVivo = false;
        Invoke("Destruir", 4.0f);
    }

   

    private void Destruir()
    {
        Destroy(gameObject);
    }

    private void Atacar(Player player, int dano)
    {
        //Ataque();
        //player.TomarDano(dano);
    }

    IEnumerator MovimentoAtaqueEspecial()
    {
        yield return new WaitForSeconds(tempoAntesDoEspecial);

        while (Vector3.Distance(transform.position, ultimaPosicaoConhecida) > 0.1f)
        {
            Vector3 direcao = (ultimaPosicaoConhecida - transform.position).normalized;
            transform.position += direcao * velocidadeEspecial * Time.deltaTime;
            yield return null; // Espera um frame
        }

        ataqueEspecialAtivo = false;
        animator.SetBool("Especial", false);

        StartCoroutine(RecarregarAtaqueEspecial());
    }


    IEnumerator RecarregarAtaqueEspecial()
    {
        yield return new WaitForSeconds(recargaAtaqueEspecial);

        podeUsarAtaqueEspecial = true;
    }
}
