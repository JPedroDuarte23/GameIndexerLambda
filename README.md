# GameIndexerLambda - Função AWS Lambda para Indexação de Jogos

## 📖 Sobre o Projeto

**GameIndexerLambda** é uma função serverless desenvolvida em .NET 8 para ser executada na AWS Lambda. Sua responsabilidade é atuar como um consumidor de eventos de criação e atualização de jogos, indexando esses dados em um cluster Elasticsearch.

Este componente é crucial para manter o motor de busca e recomendação da plataforma de jogos sempre atualizado, operando de forma desacoplada e assíncrona.

## ✨ Arquitetura e Funcionamento

A função é projetada para funcionar dentro de um ecossistema de microserviços orientado a eventos, seguindo o fluxo abaixo:

1.  **Publicação do Evento**: Quando um jogo é criado ou atualizado no microserviço `fiap-srv-games`, uma mensagem contendo os dados completos do jogo é publicada em um tópico **AWS SNS** (`game-events-topic`).
2.  **Enfileiramento**: Uma fila **AWS SQS** (Simple Queue Service) está inscrita neste tópico SNS, recebendo e armazenando a mensagem de forma segura e durável.
3.  **Ativação da Lambda**: A função `GameIndexerLambda` é acionada (`triggered`) por novas mensagens que chegam à fila SQS.
4.  **Processamento e Indexação**:
      - A Lambda consome a mensagem da fila.
      - Ela extrai e desserializa o objeto `Game` do corpo da mensagem.
      - A função se conecta ao cluster **Elasticsearch** (cujo endpoint é configurado via variáveis de ambiente).
      - Ela envia o objeto do jogo para ser indexado ou atualizado no índice `games`, usando o ID do jogo como `_id` do documento.
      - Logs detalhados sobre o sucesso ou falha da operação são enviados para o Amazon CloudWatch.

Este design garante que a responsabilidade de manter o índice de busca atualizado seja isolada, sem impactar a performance do serviço principal de gerenciamento de jogos.

## 🚀 Tecnologias Utilizadas

  - **.NET 8**: Framework de desenvolvimento da função.
  - **AWS Lambda**: Plataforma de computação serverless onde a função é executada.
  - **AWS SQS (Simple Queue Service)**: Utilizado como gatilho para a função, fornecendo as mensagens de jogos a serem processadas.
  - **AWS SNS (Simple Notification Service)**: Atua como a fonte dos eventos que são enfileirados no SQS.
  - **Elasticsearch**: O motor de busca onde os dados dos jogos são armazenados e indexados.
  - **AWS SDK for .NET**: Para interagir com os serviços da AWS.
  - **Elastic.Clients.Elasticsearch**: Biblioteca cliente para interagir com o Elasticsearch.

## ⚙️ Configuração e Deploy

A função é implantada como um pacote `.zip` na AWS Lambda. A configuração essencial é feita através de variáveis de ambiente.

### Variáveis de Ambiente

  - **`ELASTICSEARCH_ENDPOINT`**: (Obrigatória) A URL do cluster Elasticsearch que a função irá utilizar para indexar os dados.

### Configurações de Deploy

O arquivo `aws-lambda-tools-defaults.json` contém as configurações padrão para o deploy da função:

  - **`function-name`**: `fiap-game-indexer`
  - **`function-handler`**: `GameIndexerLambda::GameIndexerLambda.Function::FunctionHandler`
  - **`function-role`**: `arn:aws:iam::668191888432:role/ElasticSearchGameIndexerRole`. Este IAM Role deve conceder as permissões necessárias para:
      - Ler e deletar mensagens da fila SQS de origem.
      - Escrever logs no CloudWatch.
      - Realizar requisições HTTP para o endpoint do Elasticsearch.

### Passos para o Deploy via CLI

1.  **Instale a ferramenta global da AWS Lambda (se ainda não tiver):**

    ```bash
    dotnet tool install -g Amazon.Lambda.Tools
    ```

2.  **Configure a variável de ambiente `ELASTICSEARCH_ENDPOINT` no console da AWS Lambda após o deploy.**

3.  **Navegue até a pasta do projeto `GameIndexerLambda` e execute o comando de deploy:**

    ```bash
    cd GameIndexerLambda
    dotnet lambda deploy-function
    ```

    A ferramenta da AWS utilizará as configurações do arquivo `aws-lambda-tools-defaults.json` para empacotar e enviar a função para a sua conta da AWS.
