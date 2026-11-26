# Arquitetura legada

O projeto começou utilizando a biblioteca [Discord.NET](https://docs.discordnet.dev/index.html) para a construção do bot, porém a mesma mostrou não dar suporte a todas as funcionalidades disponíveis no Discord, principalmente as mais recentes como slash commands e activities personalizadas. Então optou-se por realizar o desenvolvimento da integração direta com o [gateway do discord](https://docs.discord.com/developers/events/gateway).

Dessa forma é possível ter:

- Controle total sobre o funcionamento do bot;
- Acesso a todos os novos recursos disponibilizados pelo gateway;
- Independência de biblioteca de terceiros;

Em contrapartida:

- Realizar a integração bruta adiciona complexidade significativa ao projeto;
- Sem uma biblioteca wrapper, o gerenciamento do ciclo de vida de conexão se torna um preocupação e ponto de foco, tirando um pouco o foco das questões negociais;
- Manter, corrigir e incrementar um projeto com tanta complexidade pode ser difícil a longo prazo, principalmente para um projeto com um único desenvolvedor.

> [!INFO]
> Em cenários corporativos esse nível de complexidade poderia se mostrar desnecessário, principalmente para uma equipe pequena. Atualmente já existem bibliotecas mais atualizadas e que dão maior suporte as funcionalidades do Discord, como a [netcord](https://netcord.dev/), além de que utilizam recursos do C# moderno.
## Sumário

- [Estrutura do projeto]

## Estruturação do projeto

```text
Koba/
├── BotSocket/        # Abstração do WebSocket
├── Core/             # Núcleo da aplicação, responsável por orquestrar tudo
├── DiscordContext/   # Builder de contextos para comandos
├── DiscordEvents/
│   ├── DiscordEvent.cs # Model base para eventos do gateway
│   ├── DiscordEventParser.cs # Parser de eventos de gateway
│   └── HelloEvent.cs # Metadados de HelloEvent
├── Enums/ # Enumeradores
├── EventHandler/ # Disparador de eventos para o bot
├── Heart/ # Gestor de heartbeat
├── Logging/ # Custom logger
├── Models/ # Models de entidades do Discord
│    ├── MessageQueue.cs # Fila de mensagens em memória
│    └── QueueManager.cs # Gerenciador de filas
├── Services/
```