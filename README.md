
[![author](https://img.shields.io/badge/author-leoccosta-red.svg)](https://www.instagram.com/leoccosta) [![](https://img.shields.io/badge/csharp-9.0+-purple.svg)](https://dotnet.microsoft.com/) [![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](http://perso.crans.org/besson/LICENSE.html) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/leocosta/jornada-do-programador/issues)

<img src="./doc/cover.png" alt="Robô Monitor de Preços"/>

## Contexto

Vamos imaginar que você trabalhe na área de tecnologia de um grande e-commerce. Para evitar que o preço dos produtos da loja fique acima da média do mercado, a empresa decide monitorar de forma automatizada o preço dos produtos da concorrência.

Agora, vamos imaginar que você queira comprar um imóvel, mas para tomar essa decisão, precisa aguardar a melhor oportunidade.

Você vai aprender as principais técnicas de programação para **automatizar tarefas repetitivas** e **criar soluções que ajudam na tomada de decisão**. 
Vamos criar um robô para monitorar o preço de produtos de uma loja virtual. 

Depois de dominar as técnicas que serão mostradas, você poderá aplicá-las para **solucionar diversos problemas do
dia a dia**.

## O robô vai realizar as seguintes tarefas:

1. Varredura do site alvo para minerar dados do produtos
2. Salvar os dados do produto e histórico de variação de preços em banco de dados
3. Gerar um relatório em PDF com gráfico de variação de preços
4. Enviar um alerta por e-mail sempre que houver algum desconto acima de 5%

### Quais técnicas vamos aplicar?

1. Web scraping
2. Armazenamento em banco de dados SQLite com Entity Framework Core 
3. Geração de HTML dinâmico
4. Plotagem de gráficos
5. Geração de documento PDF
6. Envio de e-mail automático

### Quais bibliotecas vamos usar?

1. [HtmlAgilityPack](https://html-agility-pack.net/) para web scraping
2. [EntityFramework Core](https://docs.microsoft.com/pt-br/ef/core/get-started/overview/first-app) com Sqlite para banco de dados
3. [RazorEngine.NetCore](https://antaris.github.io/RazorEngine/) para geração de páginas HTML dinâmicas
4. [XPlot.Plotly](https://fslab.org/XPlot/plotly.html) para plotar gráficos
5. [PuppeteerSharp](https://www.puppeteersharp.com/) para gerar PDF

Você vai aprender as principais técnicas para automatizar tarefas repetitivas e poderá aplicá-las seja no trabalho, na vida pessoal, ou ainda pôr uma ideia em prática para que vire negócio.


### Instalação de ferramentas e bibliotecas

Adicionando pacote **HtmlAgilityPack**
```
dotnet add package HtmlAgilityPack
```

Adicionando pacotes do **Entity Framework para SQLite**
```
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite.Design
```

Adicionando pacote **RazorEngine.NetCore** para gerar HTML dinâmico
```
dotnet add package RazorEngine.NetCore
```

Adicionando pacote **XPlot.Plotly** para plotar gráficos
```
dotnet add package FSharp.Core
dotnet add package XPlot.Plotly
```

Adicionando pacote **PuppeteerSharp** para gerar PDF
```
dotnet add package PuppeteerSharp
```

### Estrutura de Banco de Dados

Instalação do **EF Core Tool**

```
dotnet tool install --global dotnet-ef
```

Adicionando *migrations* do banco de dados
```
dotnet ef migrations add CreateDiscountMonitorDb -o ./Data/Migrations
```

Migrando estrutura do banco de dados
```
dotnet ef database update
```

> NOTA: Este projeto foi desenvolvido no curso [Jornada do Programador](https://growiz.com.br/jornada-do-programador). O objetivo foi demonstrar algumas técnicas de desenvolvimento e ferramentas disponíveis na plataforma .NET.
