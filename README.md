# API Blog Simples

Essa aplicacao disponibiliza uma API RESTful com os alguns ecursos disponíveis para acesso.

*Para organizacao etrutural do codigo foi utilizada uma adaptacao de DDD (Domain-Driven Design)*

## Configurando o Ambiente

Para rodar o programa eh preciso instalar o Docker e o Dotnet 6.0
___
### Primeiro Passo: Docker

Para iniciar o Docker execute no cmd (como administrador) o seguinte comando:

``` 
docker run -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=#Testeploomes23 -p 1433:1433 -d mcr.microsoft.com/mssql/server
```

Execute 
```
docker container ls -a
```
e verifique se o container esta rodando

___

### Segundo Passo: Build
Para buildar a aplicacao abra o terminal na pasta da solucao e execute os seguintes comandos em ordem:
```
dotnet restore
```
```
dotnet build
```

___
### Terceiro Passo: Database
Ainda no mesmo terminal execute o comando para atualizar o banco de dados que esta rodando no docker:
```
dotnet ef database update
```
___

### Quarto Passo: Iniciar Aplicacao
Apos o update da database execute o seguinte comando no mesmo terminal para iniciar a API:
```
dotnet run
```

Obs: Em caso de alteracoes no codigo eh preciso rodar o comando
```
dotnet build
```
Antes de rodar o 
```
dotnet run
```

# Sobre a API
## Métodos
Requisições para a API devem seguir os padrões:
| Método | Descrição |
|---|---|
| `GET` | Retorna informações de um ou mais registros. |
| `POST` | Utilizado para criar um novo registro. |
| `DELETE` | Utilizado para excluir um registro. |

## Respostas

| Código | Descrição |
|---|---|
| `200` | Requisição executada com sucesso (success).|
| `400` | Erros de validação ou os campos informados não existem no sistema.|
| `401` | Dados de acesso inválidos.|
| `403` | Falta de permissao para usuario. |
| `404` | Registro pesquisado não encontrado (Not found).|
| `500` | Erro de logica no sistema.|

## Listar
As ações de `listar` que retornam listas permitem o envio dos seguintes parâmetros extras:

| Parâmetro | Descrição |
|---|---|
| `skip` | Pula `x` objetos. |
| `take` | Informa `y` objetos deseja receber apos `x` |

Exemplo:
    `/post/fetchOwn?skip=5&take=25`

# Autenticacao

Essa API utiliza [JWT](https://jwt.io) como forma de autenticação/autorização.

## Solicitando tokens de acesso

### Obtendo o token

Para testar a API, voce precisa ser cadastrado como a `Role` que deseja testar

Para obter o seu token (valido por 8h) e soh fazer o login pela api numa conta criada.

(Todas as contas criadas tem o padrao de role `Viewer`)

### Utilizando o token

O `token` é do formato JWT e contém informações `username` e `role`.
O mesmo precisa ser enviado no header das requisicoes que necessitem de autenticacao como `Authorization`
Formato: 
```json 
"Authorization":"Bearer <token>"
```
# Recursos
apesar da documentacao estar aqui tambem eh possivel ler a documentacao do swagger acessando `/swagger/index.html`

dentro do programa existem 4 roles que possuem permissoes de acesso diferente, eh aconselhavel a cadastrar um usuario e mudar a role dele para 0 (Admin) direto no banco, depois disso ele consegue controlar toda a aplicacao direto pela API e tem acesso a todas as rotas.

| Nome | Numero |
|------|--------|
|Admin | 0 |
|Editor | 1 |
|Publisher | 2 |
|Viewer | 3 |

## Autenticacao [/auth]

### Login [POST /login]
*Fazer login na API*

+ Request (application/json)

    + Body
        ```json
            {
                "username": "<username>",
                "password": "<password>"
            }
        ```


+ Response 200 (application/json)
+ Response 400 (application/json)
___


### Registro [POST /register]
*Fazer registro de usuario*
+ Request (application/json)
    + Body
        ```json
            {
                "username": "<username>",
                "password": "<password>"
            }
        ```
Obs: Tamanho de `username` no maximo 16 caracteres

+ Response 200 (application/json)
+ Response 400 (application/json)
+ Response 500 (application/json)

## Postagem [/post]

### Listar todas postagens [GET /fetchAll]

+ Roles Permitidas:
  + Admin, Editor
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```


+ Response 200 (application/json)
+ Response 403 (application/json)
___

### Listar postagem unica por id [GET /{id}]

+ Allowed Roles:
  + Admin, Editor, Publisher (Se for uma Postagem propria)
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```


+ Response 200 (application/json)
+ Response 401 (appication/json)
+ Response 403 (application/json)
___

### Listar postagens publicas [GET /fetchPublic]
+ Response 200 (application/json)
___

### Listar postagens proprias [GET /fetchOwn]

+ Allowed Roles:
  + Admin, Editor, Publisher
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```


+ Response 200 (application/json)
+ Response 401 (application/json)
___

### Criar Postagem [POST /create]

+ Allowed Roles:
  + Admin, Editor, Publisher
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```
    + Body
        ```json
        {
            "title": "title",
            "body": "body_text",
            "public": true | false
        }
        ```
Obs: O `Title` deve ter ate 100 caracteres


+ Response 200 (application/json)
+ Response 401 (application/json)
+ Response 500 (application/json)
___

### Excluir Postagem [DELETE {id}/delete]

+ Allowed Roles:
  + Admin, Editor, Publisher (Se for uma postagem propria)
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```
+ Response 200 (application/json)
+ Response 401 (application/json)
+ Response 403 (application/json)
___

### Editar Postagem [POST {id}/edit]

+ Allowed Roles:
  + Admin, Editor, Publisher (Se for uma postagem propria)
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```
    + Body
        ```json
        {
            "title": "title",
            "body": "body_text",
            "public": true | false
        }
        ```
Obs: O `Title` deve ter ate 100 caracteres
+ Response 200 (application/json)
+ Response 401 (application/json)
+ Response 403 (application/json)
___

## Usuarios [/user]

### Listar todos usuarios [GET /fetchAll]

+ Roles Permitidas:
  + Admin
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```


+ Response 200 (application/json)
+ Response 403 (application/json)
___

### Ver usuario por ID [GET /{idUser}]

+ Roles Permitidas:
  + Admin
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```

+ Response 200 (application/json)
+ Response 403 (application/json)
___

### Resetar senha de usuario por ID [POST /{idUser}/resetPassword]

+ Roles Permitidas:
  + Admin
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```

+ Response 200 (application/json)
+ Response 403 (application/json)
___

### Atualizar Role de Usuario [POST /{idUser}/updateToRole?role=1]

+ Roles Permitidas:
  + Admin
+ Request (application/json)

    + Header
        ```json
            {
                "Authentication": "Bearer <token>"
            }
        ```

+ Response 200 (application/json)
+ Response 401 (application/json)
+ Response 403 (application/json)
___

Autor: [Joao Vitor dos Santos Almeida](https://github.com/jvsajv)
