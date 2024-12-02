using UnityEngine;

public class DespertarControl : MonoBehaviour
{
    [Header("Configura��es do Personagem")]
    public GameObject particula; // Part�cula para ativar
    public string nomeAnimacaoDespertar = "Despertar"; // Nome da anima��o
    public GameObject prefabSubstituto; // Prefab para troca ap�s anima��o
    public Transform posicaoTroca; // Posi��o para instanciar o prefab substituto
    public AudioClip somDespertar; // Som a ser reproduzido durante o despertar
    public AudioSource audioSource; // Fonte de �udio para tocar o som

    [Header("Configura��es da C�mera")]
    public Animator cameraAnimator; // Animator da c�mera
    public string nomeAnimacaoCamera = "CameraDespertar"; // Nome da anima��o da c�mera

    private Animator animator;
    private bool estaDespertando = false;

    void Start()
    {
        if (particula != null)
        {
            particula.SetActive(false); // Desativa a part�cula inicialmente
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator n�o encontrado no GameObject do personagem!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource n�o est� atribu�do!");
        }

        if (cameraAnimator == null)
        {
            Debug.LogError("Animator da c�mera n�o est� atribu�do!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !estaDespertando)
        {
            IniciarDespertar();
        }
    }

    void IniciarDespertar()
    {
        estaDespertando = true;

        // Ativa a part�cula
        if (particula != null)
        {
            particula.SetActive(true);
        }

        // Inicia a anima��o do personagem
        if (animator != null)
        {
            animator.SetBool("Despertar", true);
        }

        // Inicia a anima��o da c�mera
        if (cameraAnimator != null)
        {
            cameraAnimator.SetBool(nomeAnimacaoCamera, true);
        }

        // Toca o som do despertar
        if (audioSource != null && somDespertar != null)
        {
            audioSource.PlayOneShot(somDespertar);
        }

        // Verifica o t�rmino da anima��o
        StartCoroutine(EsperarFimDaAnimacao());
    }

    System.Collections.IEnumerator EsperarFimDaAnimacao()
    {
        if (animator != null)
        {
            // Aguarda at� que a anima��o do personagem inicie
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(nomeAnimacaoDespertar))
            {
                yield return null;
            }

            // Aguarda at� o t�rmino da anima��o do personagem
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        // Interrompe a anima��o da c�mera
        if (cameraAnimator != null)
        {
            cameraAnimator.SetBool(nomeAnimacaoCamera, false);
        }

        // Substitui o personagem pelo prefab
        if (prefabSubstituto != null && posicaoTroca != null)
        {
            Instantiate(prefabSubstituto, posicaoTroca.position, posicaoTroca.rotation);
            Destroy(gameObject); // Remove o GameObject atual
        }
    }
}
