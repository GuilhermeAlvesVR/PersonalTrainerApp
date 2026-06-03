# Personal Trainer App

Aplicativo mobile desenvolvido em **.NET MAUI** para gerenciamento de alunos, treinos e avaliações físicas de personal trainers.

## Demonstração

<p align="center">
  <img src="PersonalTrainerApp/images/personal-trainer-demo.gif" alt="Demonstração do Personal Trainer App" width="350">
</p>

## Sobre o Projeto

O Personal Trainer App foi desenvolvido para auxiliar personal trainers no gerenciamento de alunos e acompanhamento de treinos, centralizando informações importantes em uma única aplicação mobile.

O sistema permite cadastrar alunos, criar treinos personalizados, registrar avaliações físicas e acompanhar a evolução dos alunos de forma simples e organizada.

## Tecnologias Utilizadas

* .NET MAUI
* C#
* XAML
* SQLite
* ASP.NET Core Web API
* PostgreSQL
* Entity Framework Core

## Funcionalidades

### Cadastro de Alunos

Permite cadastrar informações dos alunos, como:

* Nome
* Idade
* Peso
* Altura
* Objetivo
* Observações
* Medidas corporais

### Gerenciamento de Treinos

Permite criar e editar treinos personalizados para cada aluno.

Cada treino pode conter:

* Exercícios
* Séries
* Repetições
* Carga
* Observações

Os treinos podem ser organizados em fichas A, B, C e D.

### Treinando Agora

Área destinada aos alunos que estão realizando o treino no momento.

Funcionalidades:

* Iniciar treino
* Finalizar treino
* Controle de alunos ativos
* Visualização rápida da ficha de treino

### Avaliação Física

Registro e acompanhamento da evolução física dos alunos.

Métricas disponíveis:

* Peso
* Peito
* Braço
* Cintura
* Abdômen
* Quadril
* Coxa
* Panturrilha

### Histórico de Treinos

Permite visualizar registros anteriores dos treinos realizados pelos alunos.

### Dashboard de Evolução

Exibe indicadores e gráficos para acompanhamento da evolução física dos alunos.

## Estrutura do Projeto

```text
PersonalTrainerApp
│
├── Models
├── Services
├── Views
├── Data
├── Resources
├── Platforms
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs
```

## Objetivo do Projeto

Durante o desenvolvimento foram aplicados conceitos como:

* Desenvolvimento Mobile com .NET MAUI
* Persistência de Dados com SQLite
* Consumo de APIs REST
* Integração entre Aplicativo e Servidor
* Arquitetura em Camadas
* Modelagem de Dados
* Boas Práticas em C#
* Desenvolvimento de Interfaces Responsivas

## Autor

**Guilherme Alves**

Estudante de Sistemas de Informação com foco em desenvolvimento Back-end e Mobile utilizando C# e .NET.

GitHub: https://github.com/GuilhermeAlvesVR
