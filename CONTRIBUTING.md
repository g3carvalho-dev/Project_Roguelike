# Guia de Contribuição — Project Roguelike

Obrigado por contribuir com o Project Roguelike! Para manter o repositório organizado e o histórico de código limpo, seguimos o fluxo de trabalho descrito abaixo. Leia com atenção antes de abrir sua primeira Pull Request.

---

## Índice

1. [Modelo de Branches (GitFlow)](#modelo-de-branches-gitflow)
2. [Regras de Commit](#regras-de-commit)
3. [Processo de Pull Request](#processo-de-pull-request)
4. [Restrições Importantes](#restrições-importantes)

---

## Modelo de Branches (GitFlow)

Utilizamos três tipos principais de branches:

### `main`

- Contém **apenas código estável e pronto para produção**.
- Representa o estado oficial e lançável do projeto.
- **Commits diretos na `main` são estritamente proibidos.**
- Toda alteração na `main` deve chegar exclusivamente via Pull Request aprovada.

### `develop`

- Branch de **integração contínua** onde todas as features concluídas são mescladas.
- Deve estar sempre em estado funcional — código quebrado não deve ser integrado aqui.
- **Commits diretos na `develop` também são proibidos.** Toda alteração deve chegar via Pull Request aprovada.

### `feature/*`

- Criada a partir de `develop` para o **desenvolvimento de novas mecânicas ou funcionalidades**.
- O nome deve ser descritivo e usar kebab-case: `feature/nome-da-funcionalidade`.
- Exemplo: `feature/geracao-procedural-de-masmorras`, `feature/sistema-de-inventario`.
- Ao concluir o desenvolvimento, abra uma Pull Request direcionando a `develop`.

#### Criando uma branch de feature

```bash
# Sempre parta da develop atualizada
git checkout develop
git pull origin develop

# Crie e acesse a nova branch
git checkout -b feature/nome-da-funcionalidade
```

---

## Regras de Commit

- Escreva mensagens de commit claras e no **imperativo** (ex.: `Adiciona sistema de combate`, `Corrige bug na geração de salas`).
- Cada commit deve representar uma unidade lógica de trabalho — evite commits gigantes com múltiplas mudanças não relacionadas.
- Evite mensagens genéricas como `fix`, `update` ou `wip` sem contexto.

---

## Processo de Pull Request

1. Certifique-se de que sua branch está atualizada em relação à `develop`:
   ```bash
   git fetch origin
   git pull origin/develop
   ```
2. Abra uma Pull Request pela interface do GitHub, descrevendo:
   - **O que** foi implementado ou corrigido.
   - **Como** testar a mudança.
   - Qualquer contexto relevante (referências a issues, decisões de design, etc.).
3. Solicite a revisão de **pelo menos um outro membro da equipe**.
4. Aguarde a aprovação antes de realizar o merge.
5. Após a aprovação, o merge pode ser feito pelo autor ou pelo revisor.

> Merges para `main` seguem o mesmo processo, mas exigem que o código já esteja integrado e validado em `develop`.

---

## Restrições Importantes

| Ação | Permitido? |
|---|---|
| Commit direto na `main` | ❌ **Proibido** |
| Commit direto na `develop` | ❌ **Proibido** |
| Merge na `main` sem Pull Request | ❌ **Proibido** |
| Merge na `develop` sem Pull Request | ❌ **Proibido** |
| Merge na `main` ou `develop` sem aprovação de ao menos um membro | ❌ **Proibido** |
| Abertura de branch `feature/*` a partir de `develop` | ✅ Recomendado |
| Pull Request com descrição clara e revisão de par | ✅ Obrigatório |

---

Em caso de dúvidas sobre o fluxo, abra uma issue ou converse com o time antes de começar a implementação.
