using UnityEngine;

public class DespertarControl : MonoBehaviour
{
    [Header("Configurações do Personagem")]
    public GameObject particula; // Partícula para ativar
    public string nomeAnimacaoDespertar = "Despertar"; // Nome da animação
    public GameObject prefabSubstituto; // Prefab para troca após animação
    public Transform posicaoTroca; // Posição para instanciar o prefab substituto
    public AudioClip somDespertar; // Som a ser reproduzido durante o despertar
    public AudioSource audioSource; // Fonte de áudio para tocar o som

    [Header("Configurações da Câmera")]
    public Animator cameraAnimator; // Animator da câmera
    public string nomeAnimacaoCamera = "CameraDespertar"; // Nome da animação da câmera

    private Animator animator;
    private bool estaDespertando = false;

    void Start()
    {
        if (particula != null)
        {
            particula.SetActive(false); // Desativa a partícula inicialmente
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado no GameObject do personagem!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource não está atribuído!");
        }

        if (cameraAnimator == null)
        {
            Debug.LogError("Animator da câmera não está atribuído!");
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

        // Ativa a partícula
        if (particula != null)
        {
            particula.SetActive(true);
        }

        // Inicia a animação do personagem
        if (animator != null)
        {
            animator.SetBool("Despertar", true);
        }

        // Inicia a animação da câmera
        if (cameraAnimator != null)
        {
            cameraAnimator.SetBool(nomeAnimacaoCamera, true);
        }

        // Toca o som do despertar
        if (audioSource != null && somDespertar != null)
        {
            audioSource.PlayOneShot(somDespertar);
        }

        // Verifica o término da animação
        StartCoroutine(EsperarFimDaAnimacao());
    }

    System.Collections.IEnumerator EsperarFimDaAnimacao()
    {
        if (animator != null)
        {
            // Aguarda até que a animação do personagem inicie
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(nomeAnimacaoDespertar))
            {
                yield return null;
            }

            // Aguarda até o término da animação do personagem
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }

        // Interrompe a animação da câmera
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
