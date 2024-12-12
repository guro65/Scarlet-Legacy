using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Habilidade
{
    public string nome; // Nome da habilidade
    public Image cooldownImagem; // Imagem de cooldown
    public GameObject prefab; // Prefab associado à habilidade
    public float cooldownTempo = 10f; // Tempo de cooldown
    public int dano = 0; // Dano causado pela habilidade (0 para habilidades como regeneração)
    public KeyCode teclaAtivacao; // Tecla para ativar a habilidade
    public string animacao; // Nome da animação a ser ativada
    public float porcentagemRegeneracao = 0.15f; // Porcentagem de vida que será regenerada (15% por padrão)
    public bool isDespertar = false; // Verifica se a habilidade é de despertar
}

public class Combate : MonoBehaviour
{
    private Animator animator; // Referência ao Animator do personagem
    private bool[] habilidadesDisponiveis; // Controle de disponibilidade de cada habilidade
    private bool alternarSoco = true; // Controle para alternar entre socos

    [Header("Configuração das Habilidades")]
    public List<Habilidade> habilidades; // Lista de habilidades

    [Header("Configuração da Câmera")]
    public Camera cameraPrincipal; // Câmera do jogador
    public Camera cameraAnimacao; // Câmera usada na animação de despertar
    public GameObject playerPrefabNovo; // Prefab do player após a transformação

    void Start()
    {
        // Obtém o componente Animator do GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado! Certifique-se de que o componente Animator está anexado.");
        }

        // Inicializa as habilidades como disponíveis
        habilidadesDisponiveis = new bool[habilidades.Count];
        for (int i = 0; i < habilidades.Count; i++)
        {
            habilidadesDisponiveis[i] = true;
            if (habilidades[i].cooldownImagem != null)
            {
                habilidades[i].cooldownImagem.fillAmount = 1f;
            }
        }

        // Garante que a câmera principal esteja ativa no início
        if (cameraPrincipal != null)
        {
            cameraPrincipal.enabled = true;
        }
        if (cameraAnimacao != null)
        {
            cameraAnimacao.enabled = false; // Câmera de animação começa desativada
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (alternarSoco)
            {
                // Ativa a animação de SocoEsquerdo
                animator.SetTrigger("SocoEsquerdo");
            }
            else
            {
                // Ativa a animação de SocoDireito
                animator.SetTrigger("SocoDireito");
            }

            // Alterna para o próximo soco
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

        // Ativa a animação da habilidade
        if (!string.IsNullOrEmpty(habilidade.animacao))
        {
            animator.SetTrigger(habilidade.animacao);
        }

        // Efeitos específicos para cada camada
        switch (indice)
        {
            case 0: // Camada 0 - Espinhos
            case 1: // Camada 1 - Estaca
                if (habilidade.prefab != null)
                {
                    GameObject prefabInstanciado = Instantiate(habilidade.prefab, transform.position, Quaternion.identity);
                    Destroy(prefabInstanciado, 5f); // Destroi o prefab após 5 segundos
                }
                break;

            case 2: // Camada 2 - Regeneração
                RegenerarVida(habilidade.porcentagemRegeneracao); // Usa a porcentagem definida para regeneração
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
        Player player = GetComponent<Player>(); // Obtém o script Player
        if (player != null)
        {
            float vidaParaRegenerar = player.VidaMaxima * porcentagem; // Calcula a quantidade a regenerar
            player.AumentarVida(vidaParaRegenerar); // Regenera a vida utilizando o método no script Player
            Debug.Log($"Regenerou {vidaParaRegenerar} de vida!");
        }
    }

    private IEnumerator ExecutarDespertar()
    {
        // Ativa a câmera de animação e desativa a câmera principal
        if (cameraAnimacao != null && cameraPrincipal != null)
        {
            cameraPrincipal.enabled = false;
            cameraAnimacao.enabled = true;
        }

        // Inicia a animação de despertar
        if (animator != null)
        {
            animator.SetTrigger("Despertar"); // Usa o trigger "Despertar"
        }

        // Aguarda a conclusão da animação (ajuste o tempo conforme necessário)
        yield return new WaitForSeconds(3f); // Tempo da animação

        // Substitui o prefab do player
        if (playerPrefabNovo != null)
        {
            Vector3 posicaoAtual = transform.position;
            Quaternion rotacaoAtual = transform.rotation;
            Destroy(gameObject); // Destroi o player atual
            Instantiate(playerPrefabNovo, posicaoAtual, rotacaoAtual); // Instancia o novo prefab
        }

        // Retorna à câmera principal e desativa a câmera de animação
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

            cooldownImagem.fillAmount = 1f; // Reseta para cheia após o cooldown
        }
    }
}