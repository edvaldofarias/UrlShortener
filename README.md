# UrlShortener
URL shortener em .NET e com Angular com foco em alta escala, performance e arquitetura moderna.

## 🎯 Objetivo

- Receber uma URL longa e gerar uma URL curta.
- Redirecionar o usuário, ao acessar a URL curta, para a URL original.
- Usar o projeto como laboratório para experimentar decisões de arquitetura (armazenamento, cache, distribuição, etc.).

---

## 🧩 Requisitos (de estudo)

### Requisitos funcionais

1. **Encurtamento de URL**: dado um URL longo, retornar um URL muito mais curto.
2. **Redirecionamento de URL**: dado um URL curto, redirecionar para o URL original.
3. **Geração de QR Code**: gerar um QR code para a URL encurtada.

### Requisitos não funcionais (cenário alvo para estudo)

1. Suportar até **100 milhões de URLs geradas por dia** (como exercício de dimensionamento).
2. O tamanho da URL encurtada deve ser o mais curto possível.
3. Apenas caracteres `0-9`, `a-z`, `A-Z` (Base62) devem ser usados no código curto.
4. Para cada operação de gravação, estima-se **10 leituras**.
5. Comprimento médio das URLs armazenadas: **100 bytes**.
6. URLs devem ser armazenadas por, no mínimo, **10 anos**.
7. Sistema projetado para alta disponibilidade (**24/7**).
8. Latência máxima aceitável para redirecionamento: **100 ms**.
9. Sistema deve ser escalável horizontalmente.
10. Considerar estratégias para evitar geração de URLs duplicadas.
11. Considerar estratégias para limpeza de URLs expiradas ou não utilizadas.
12. Gerar métricas de uso (número de acessos por URL, URLs mais acessadas, etc.).
13. Implementar logs para monitoramento e auditoria.
14. Gerar Qr codes para as URLs encurtadas.
15. Documentar a API usando Swagger/OpenAPI.

Na primeira versão, esses requisitos são considerados principalmente **conceituais**, servindo de base para as próximas etapas de evolução do projeto.
