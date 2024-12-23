using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Combate;

[System.Serializable]
public class Habilidade
{
    public string nome; // Nome da habilidade
    public Image cooldownImagem; // Imagem de cooldown
    public GameObject prefab; // Prefab associado � habilidade
    public float cooldownTempo = 10f; // Tempo de cooldown
    public int dano = 0; // Dano causado pela habilidade
    public KeyCode teclaAtivacao; // Tecla para ativar a habilidade
    public string animacao; // Nome da anima��o a ser ativada
    public float porcentagemRegeneracao = 0.15f; // Porcentagem de vida regenerada
    public bool isDespertar = false; // Verifica se a habilidade � de despertar
    public PontoSpawn pontoSpawn; // Onde o prefab ser� gerado
    public float tempoDeVidaPrefab = 5f; // Tempo de vida do prefab na cena
}


public class Combate : MonoBehaviour
{
    private Animator animator; // Refer�ncia ao Animator do personagem
    private bool[] habilidadesDisponiveis; // Controle de disponibilidade de cada habilidade
    private bool alternarSoco = true; // Controle para alternar entre socos

    [Header("Configura��o das Habilidades")]
    public List<Habilidade> habilidades; // Lista de habilidades
    public enum PontoSpawn
    {
        ProprioPlayer, // Na posi��o do pr�prio player
        OutroPlayer // Na posi��o do oponente
    }

    [Header("Configura��o do Personagem")]
    public GameObject personagemPrefab; // Prefab do personagem atual
    public Transform spawnPonto; // Ponto onde o personagem ser� gerado

    [Header("Configura��o da C�mera")]
    public Camera cameraPrincipal; // C�mera do jogador
    public Camera cameraAnimacao; // C�mera usada na anima��o de despertar

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator n�o encontrado! Certifique-se de que o componente Animator est� anexado.");
        }

        habilidadesDisponiveis = new bool[habilidades.Count];
        for (int i = 0; i < habilidades.Count; i++)
        {
            habilidadesDisponiveis[i] = true;
            if (habilidades[i].cooldownImagem != null)
            {
                habilidades[i].cooldownImagem.fillAmount = 1f;
            }
        }

        if (cameraPrincipal != null)
        {
            cameraPrincipal.enabled = true;
        }
        if (cameraAnimacao != null)
        {
            cameraAnimacao.enabled = false;
        }

        GerarPersonagemComHabilidades();
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
            // Verifica se a tecla foi pressionada e se o cooldown da habilidade terminou
            if (Input.GetKeyDown(habilidades[i].teclaAtivacao) && habilidadesDisponiveis[i])
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
        if (!habilidadesDisponiveis[indice])
            yield break;

        habilidadesDisponiveis[indice] = false;

        // Ativa a anima��o da habilidade
        if (!string.IsNullOrEmpty(habilidade.animacao))
        {
            animator.SetTrigger(habilidade.animacao);
        }

        // Gera o prefab e define sua destrui��o ap�s o tempo configurado
        if (habilidade.prefab != null)
        {
            Vector3 posicaoPrefab = DeterminarPosicaoSpawn(habilidade.pontoSpawn);
            GameObject prefabInstanciado = Instantiate(habilidade.prefab, posicaoPrefab, Quaternion.identity);

            // Verifica se o tempo de vida foi definido
            if (habilidade.tempoDeVidaPrefab > 0f)
            {
                Destroy(prefabInstanciado, habilidade.tempoDeVidaPrefab); // Destroi o prefab ap�s o tempo configurado
            }
        }

        StartCoroutine(AtualizarCooldownVisual(habilidade.cooldownImagem, habilidade.cooldownTempo));
        // Aguarda o tempo de cooldown
        yield return new WaitForSeconds(habilidade.cooldownTempo);
        // Habilidade est� dispon�vel novamente
        habilidadesDisponiveis[indice] = true;
    }


    private Vector3 DeterminarPosicaoSpawn(PontoSpawn pontoSpawn)
    {
        GameObject playerAtual = gameObject;
        GameObject outroPlayer = GameObject.FindWithTag(playerAtual.CompareTag("Player 1") ? "Player 2" : "Player 1");

        if (pontoSpawn == PontoSpawn.ProprioPlayer)
        {
            return playerAtual.transform.position;
        }
        else if (pontoSpawn == PontoSpawn.OutroPlayer && outroPlayer != null)
        {
            return outroPlayer.transform.position;
        }

        Debug.LogWarning("N�o foi poss�vel determinar a posi��o de spawn. Usando posi��o padr�o.");
        return transform.position; // Posi��o padr�o caso algo d� errado
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

    private void GerarPersonagemComHabilidades()
    {
        if (personagemPrefab != null && spawnPonto != null)
        {
            Instantiate(personagemPrefab, spawnPonto.position, spawnPonto.rotation);

            foreach (var habilidade in habilidades)
            {
                if (habilidade.prefab != null)
                {
                    habilidade.prefab = ObterPrefabPorPersonagem(habilidade.nome);
                }
            }
        }
    }

    private GameObject ObterPrefabPorPersonagem(string habilidadeNome)
    {
        // Personalize a l�gica para selecionar o prefab baseado no personagem
        // Exemplo: Use um dicion�rio ou switch-case para retornar o prefab correto
        Debug.Log($"Obtendo prefab para habilidade: {habilidadeNome}");
        return null; // Substitua com a l�gica para retornar o prefab correto
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

            cooldownImagem.fillAmount = 1f;
        }
    }
}
