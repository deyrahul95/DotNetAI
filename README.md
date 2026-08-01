# DotNetAI

Implementation of GEN-AI base application using Dotnet Semantic Kernel.

## Prerequisite

- Dotnet SDK (Version 10)
- Ollama (With any one model)

## Development

1. Clone the repo.

```bash
git clone https://github.com/deyrahul95/DotNetAI
```

2. Restore packages

```bash
cd DotNetAI
dotnet restore
```

3. Configured development environment

- Create ```appsettings.Development.json``` file and past the bellow json 

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "OpenAI": {
        "Endpoint": "",
        "ApiKey": "",
        "ModelName": ""
    }
}
```

- Add local ollama endpoint and model name in ```OpenAI``` section.
- Add ApiKey if required for ollama endpoints or else put it ```ollama```.

4. Run the project

```bash
dotnet watch --project DotNetAI.Api
```

5. Open this link ``` http://localhost:5283/scalar ``` in the browser for the Scaler UI.