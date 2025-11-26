# UrlShortener

Encurtador de URL em **ASP.NET Core Web API**, criado para estudo de arquitetura, performance e evolução incremental de um sistema de alta escala.

Nesta primeira versão, os dados são armazenados **em memória** usando uma estrutura `static`. Em versões futuras, o projeto será evoluído para uso de banco de dados, cache e outros componentes.

---

## 🛠️ Tecnologias

- **Linguagem:** C#
- **Runtime:** .NET (ASP.NET Core Web API)
- **Armazenamento inicial:** estrutura estática em memória.
- **Codificação:** Base62 (apenas 0–9, A–Z, a–z) para gerar códigos curtos

---

## 📂 Estrutura do projeto

Projeto em **uma camada apenas**, organizado por pastas dentro da Web API:

```text
UrlShortener/
 ├─ Controllers/
 │   └─ UrlsController.cs
 ├─ Services/
 │   └─ UrlShorteningService.cs      // regra de negócio (acesso ao repositório, geração de código, etc.)
 ├─ Helpers/
 │   └─ Base62Encoder.cs               // codificação Base62
 ├─ Entities/
 │   └─ Shorten/ShortenEntity.cs         // Entidade de domínio
 ├─ Repositories/
 │   └─ Shorten/ShortenRepository.cs       // acesso a dados (in-memory nesta versão)
 ├─ Program.cs
 └─ README.md

```

---

## 🚀 Endpoints

### 1. Encurtar URL

**POST** `api/shorten`

#### Request

```json
{
  "url": "https://www.sitequalquer.com/algum/endereco/bem/grande"
}
```

#### Response

```json
{
  "shortUrl": "http://localhost:5000/abc123"
}
```

### 2. Redirecionar URL

**GET** `/{shortCode}`

- Redireciona para a URL original associada ao `shortCode`.
- Retorna `404 Not Found` se o código não existir.
- Exemplo: Acessar `http://localhost:5000/abc123` redireciona para `https://www.sitequalquer.com/algum/endereco/bem/grande`.
