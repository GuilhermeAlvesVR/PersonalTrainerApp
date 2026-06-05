# PersonalTrainerWeb

Versao web estatica do app para uso pelo navegador, inclusive em notebooks que nao rodam o app Windows MAUI.

## Como testar localmente

Abra um terminal nesta pasta e rode um servidor estatico, por exemplo:

```powershell
python -m http.server 5500
```

Depois acesse:

```text
http://localhost:5500
```

## API

Por padrao, o site usa:

```text
https://personaltrainerapp-myci.onrender.com
```

Para usar outra API no navegador:

```js
localStorage.setItem('apiBase', 'https://sua-api.onrender.com')
location.reload()
```

## Deploy

Pode publicar esta pasta como site estatico no Render, Netlify ou Vercel.
