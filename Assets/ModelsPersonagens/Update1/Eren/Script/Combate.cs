using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Habilidade
{
    public string nome; // Nome da habilidade
    public Image cooldownImagem; // Imagem de cooldown
    public GameObject prefab; // Prefab associado � habilidade
    public float cooldownTempo = 10f; // Tempo de cooldown
    public int dano = 0; // Dano causado pela habilidade (0 para habilidades como regenera��o)
    public KeyCode teclaAtivacao; // Tecla para ativar a habilidade
    public string animacao; // Nome da anima��o a ser ativada
    public float porcentagemRegeneracao = 0.15f; // Porcentagem de vida que ser� regenerada (15% por padr�o)
    public bool isDespertar = false; // Verifica se a habilidade � de despertar
}

public class Combate : MonoBehaviour
{
    private Animator animator; // Refer�ncia ao Animator do personagem
    private bool[] habilidadesDisponiveis; // Controle de disponibilidade de cada habilidade
    private bool alternarSoco = true; // Controle para alternar entre socos

    [Header("Configura��o das Habilidades")]
    public List<Habilidade> habilidades; // Lista de habilidades

    [Header("Configura��o da C�mera")]
    public Camera cameraPrincipal; // C�mera do jogador
    public Camera cameraAnimacao; // C�mera usada na anima��o de despertar
    public GameObject playerPrefabNovo; // Prefab do player ap�s a transforma��o

    void Start()
    {
        // Obt�m o componente Animator do GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator n�o encontrado! Certifique-se de que o componente Animator est� anexado.");
        }

        // Inicializa as habilidades como dispon�veis
        habilidadesDisponiveis = new bool[habilidades.Count];
        for (int i = 0; i < habilidades.Count; i++)
        {
            habilidadesDisponiveis[i] = true;
            if (habilidades[i].cooldownImagem != null)
            {
                habilidades[i].cooldownImagem.fillAmount = 1f;
            }
        }

        // Garante que a c�mera principal esteja ativa no in�cio
        if (cameraPrincipal != null)
        {
            cameraPrincipal.enabled = true;
        }
        if (cameraAnimacao != null)
        {
            cameraAnimacao.enabled = false; // C�mera de anima��o come�a desativada
        }
    }

    void Update()
    {
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

        // Verifica se uma tecla associada a uma habilidade foi pressionada
        for (int i = 0; i < habilidades.Count; i++)
        {
            if (Input.GetKeyDown(habilidades[i].teclaAtivacao))
            {
                AtivarHabilidade(i);
            }
        }
    }

    public void AtivarHabilidade(int indice)
    {
        if (indice >= 0 && indice < habilidades.Count && habilidadesDisponiveis[indice])
        {
            StartCoroutine(ExecutarHabilidade(indice));
        }
    }

    private IEnumerator ExecutarHabilidade(int indice)
    {
        Habilidade habilidade = habilidades[indice];
        habilidadesDisponiveis[indice] = false;

        // Ativa a anima��o da habilidade
        if (!string.IsNullOrEmpty(habilidade.animacao))
        {
            animator.SetTrigger(habilidade.animacao);
        }

        // Efeitos espec�ficos para cada camada
        switch (indice)
        {
            case 0: // Camada 0 - Espinhos
            case 1: // Camada 1 - Estaca
                if (habilidade.prefab != null)
                {
                    GameObject prefabInstanciado = Instantiate(habilidade.prefab, transform.position, Quaternion.identity);
                    Destroy(prefabInstanciado, 5f); // Destroi o prefab ap�s 5 segundos
                }
                break;

            case 2: // Camada 2 - Regenera��o
                RegenerarVida(habilidade.porcentagemRegeneracao); // Usa a porcentagem definida para regenera��o
                if (habilidade.prefab != null)
                {
                    Instantiate(habilidade.prefab, transform.position, Quaternion.identity);
                }
                break;

            case 3: // Camada 3 - Despertar
                if (habilidade.isDespertar)
                {
                    yield return StartCoroutine(ExecutarDespertar());
                }
                break;
        }

        // Inicia o cooldown visual
        StartCoroutine(AtualizarCooldownVisual(habilidade.cooldownImagem, habilidade.cooldownTempo));

        // Aguarda o tempo de cooldown para liberar a habilidade novamente
        yield return new WaitForSeconds(habilidade.cooldownTempo);
        habilidadesDisponiveis[indice] = true;
    }

    private void RegenerarVida(float porcentagem)
    {
        Player player = GetComponent<Player>(); // Obt�m o script Player
        if (player != null)
        {
            float vidaParaRegenerar = player.VidaMaxima * porcentagem; // Calcula a quantidade a regenerar
            player.AumentarVida(vidaParaRegenerar); // Regenera a vida utilizando o m�todo no script Player
            Debug.Log($"Regenerou {vidaParaRegenerar} de vida!");
        }
    }

    private IEnumerator ExecutarDespertar()
    {
        // Ativa a c�mera de anima��o e desativa a c�mera principal
        if (cameraAnimacao != null && cameraPrincipal != null)
        {
            cameraPrincipal.enabled = false;
            cameraAnimacao.enabled = true;
        }

        // Inicia a anima��o de despertar
        if (animator != null)
        {
            animator.SetTrigger("Despertar"); // Usa o trigger "Despertar"
        }

        // Aguarda a conclus�o da anima��o (ajuste o tempo conforme necess�rio)
        yield return new WaitForSeconds(3f); // Tempo da anima��o

        // Substitui o prefab do player
        if (playerPrefabNovo != null)
        {
            Vector3 posicaoAtual = transform.position;
            Quaternion rotacaoAtual = transform.rotation;
            Destroy(gameObject); // Destroi o player atual
            Instantiate(playerPrefabNovo, posicaoAtual, rotacaoAtual); // Instancia o novo prefab
        }

        // Retorna � c�mera principal e desativa a c�mera de anima��o
        if (cameraAnimacao != null && cameraPrincipal != null)
        {
            cameraAnimacao.enabled = false;
            cameraPrincipal.enabled = true;
        }
    }

    private IEnumerator AtualizarCooldownVisual(Image cooldownImagem, float cooldownTempo)
    {
        if (cooldownImagem != null)
        {
            cooldownImagem.fillAmount = 0f;

            float tempoPassado = 0f;
            while (tempoPassado < cooldownTempo)
            {
                tempoPassado += Time.deltaTime;
                cooldownImagem.fillAmount = tempoPassado / cooldownTempo;
                yield return null;
            }

            cooldownImagem.fillAmount = 1f; // Reseta para cheia ap�s o cooldown
        }
    }
}