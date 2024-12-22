using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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
    [SerializeField] private float paraDeSeguirDistancia = 3f;
    [SerializeField] private bool estaSeguindo;
    private bool estaVivo = true;
    private Animator animator;
    private Vector3 ultimaPosicaoConhecida;
    private bool playerNaAreaDeAtaque;
    private bool podeUsarAtaqueEspecial = true;
    private bool ataqueEspecialAtivo = false;
    private bool ataqueNormalAtivo = false;
    private bool pausaParaAndar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        animator.SetBool("Andar",true);
        estaSeguindo = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Não prossegue se o ataque especial estiver ativo
        if (ataqueEspecialAtivo)
        {
            return;
        }
         if (estaSeguindo)
        {
            animator.SetBool("EstaParado", false);
            MoverAteOPlayer();
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
                animator.SetBool("Andar", true);
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
        ParadeSeMover();
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
        // Move o boss até a última posição conhecida do jogador
        Vector3 direcao = (ultimaPosicaoConhecida - transform.position).normalized;
        
        if(Vector3.Distance(player.transform.position, transform.position) <= paraDeSeguirDistancia )
        {
        transform.position += direcao * velocidade * Time.deltaTime;
        animator.SetBool("Andar", true);
        animator.SetBool("Especial", false);
        estaSeguindo = false;
        }
        
        
        if (direcao != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            animator.SetBool("Andar", true);
            estaSeguindo = true;
        }
        else
        {
            estaSeguindo = false;
            animator.SetBool("Andar", false);
        }
    }

    private void ParadeSeMover()
    {
        if(pausaParaAndar ==true)
        animator.SetBool("Andar",false);
        animator.SetBool("PodeAtacar",true);
        
    }
    private void VoltarASeguir()
    {
        if(pausaParaAndar == false)
        animator.SetBool("Andar",true);
        animator.SetBool("PodeAtacar",false);   
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisão com " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player") && ataqueEspecialAtivo)
        {
            Atacar(collision.gameObject.GetComponent<Player>(), 30);
            animator.SetBool("Andar", false);
        }

        if (collision.gameObject.CompareTag("Player") && playerNaAreaDeAtaque)
        {
            Atacar(collision.gameObject.GetComponent<Player>(), 10);
            animator.SetBool("Andar", false);
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
        Ataque();
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
