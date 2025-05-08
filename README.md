# Quod Sprint2 - Sistema de Validação de Documentos e Biometria

Solução completa para validação de documentos e verificação biométrica com detecção de fraudes.

## 🚀 Funcionalidades Principais
- Validação de documentos com extração de metadados
- Análise biométrica com detecção de fraudes
- Notificações em tempo real
- Armazenamento seguro de registros de validação

## ⚙️ Tecnologias Utilizadas
- .NET 8
- MongoDB (armazenamento)
- Docker (containerização)
- PlantUML (documentação arquitetural)

## 📦 Estrutura do Projeto
```
Controllers/
├── BiometryController.cs     # Endpoints biométricos
├── DocumentController.cs     # Endpoints de documentos
Services/
├── MetadataService.cs        # Extração de metadados
├── NotificationService.cs    # Serviço de notificações
Models/                       # Modelos de dados
Repositories/                 # Acesso a dados
```

## 🛠️ Configuração e Execução

### Pré-requisitos
- .NET 8 SDK
- Docker
- MongoDB

### Como Executar
1. Clone o repositório:
```bash
git clone git@github.com:OBuskas/quod-sprint2.git
cd quod-sprint2
```

2. Inicie os containers:
```bash
docker-compose up -d
```

3. Execute a aplicação:
```bash
dotnet run
```

## 📄 Documentação da API (OpenAPI)
Consulte [doc.json](./doc.json) para detalhes completos dos endpoints.
