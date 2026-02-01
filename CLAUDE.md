# JumpyPro - Proyecto Unity

## Descripcion del Juego
Clon de "Jumpy" de Facebook. Endless faller donde:
- El jugador (esfera) cae constantemente
- Debe pasar por huecos en las plataformas
- Colision con plataforma = Game Over
- Dificultad aumenta progresivamente (velocidad + huecos mas pequenos)
- Sistema de score y high score persistente

## Estado Actual (30 Enero 2025)
**FUNCIONA:**
- Core del juego funcionando
- Jugador se mueve con A/D o touch izq/derecha
- Plataformas se generan con huecos aleatorios
- Colision detecta Game Over
- Score incrementa al pasar plataformas
- Sistema de estados (Menu, Playing, GameOver)
- UI basica conectada

**PENDIENTE:**
- [ ] Ajustar diseno UI (posiciones, colores, tamanos)
- [ ] Probar en dispositivo Android real
- [ ] Instalar Google Mobile Ads SDK
- [ ] Configurar AdMob (cuando tenga cuenta)
- [ ] Configurar build Android (package name, icons, etc)
- [ ] Pulir gameplay (balanceo de dificultad)

## Estructura de Scripts

```
Assets/Scripts/
├── Core/
│   └── GameManager.cs      # Estados, score, dificultad, singleton
├── Player/
│   └── PlayerController.cs # Movimiento horizontal, colision
├── Platforms/
│   ├── Platform.cs         # Movimiento hacia arriba, score trigger
│   └── PlatformSpawner.cs  # Object pool, genera plataformas
├── UI/
│   └── UIManager.cs        # Paneles Menu/Gameplay/GameOver
└── Ads/
    ├── AdConfig.cs         # ScriptableObject para IDs de AdMob
    └── AdManager.cs        # Banner + Interstitial (placeholder hasta instalar SDK)
```

## Objetos en la Escena (Hierarchy)

```
- Main Camera (pos: 0, 0, -10)
- GameManager (script: GameManager)
- PlatformSpawner (script: PlatformSpawner)
  - Platform Prefab: Assets/Prefabs/Platform
  - Player: referencia al Player
- Player (esfera con script PlayerController, tag: Player)
- AdManager (script: AdManager, con AdConfig asset)
- UIManager (script: UIManager, con todas las referencias UI)
- Canvas
  ├── MenuPanel (TitleText, HighScoreText, PlayButton)
  ├── GameplayPanel (ScoreText)
  └── GameOverPanel (GameOverText, FinalScoreText, FinalHighScoreText, NewRecordObject, RetryButton, MenuButton)
- EventSystem (creado automatico por Unity)
```

## Prefabs

```
Assets/Prefabs/
└── Platform.prefab
    ├── LeftSegment (Cube, tag: Platform)
    └── RightSegment (Cube, tag: Platform)
```

## Tags y Layers Necesarios

**Tags:**
- Player
- Platform

**Layers:**
- Player (layer 8)
- Platform (layer 9)

## Valores de Gameplay (en GameManager)

```
Velocidad caida inicial: 3 u/s
Velocidad caida maxima: 8 u/s
Hueco inicial: 2.0 unidades
Hueco minimo: 1.2 unidades
Score para max dificultad: 100 puntos
```

## AdMob (para cuando se implemente)

**IDs de Test:**
- Banner: `ca-app-pub-3940256099942544/6300978111`
- Interstitial: `ca-app-pub-3940256099942544/1033173712`
- App ID Test: `ca-app-pub-3940256099942544~3347511713`

**Frecuencia interstitial:** cada 3 partidas

**Pasos para activar AdMob:**
1. Descargar Google Mobile Ads Unity Plugin de GitHub
2. Importar el .unitypackage
3. Assets > External Dependency Manager > Android Resolver > Resolve
4. Descomentar el codigo en AdManager.cs (buscar "DESCOMENTAR")
5. Crear cuenta AdMob y reemplazar IDs en AdConfig asset

## Configuracion Android (pendiente)

```
Package Name: com.roferreiradev.jumpypro
Min API: 24 (Android 7.0)
Target API: 34
Scripting Backend: IL2CPP
Arquitecturas: ARM64
Orientacion: Portrait
```

## Problemas Conocidos

1. **Error Burst Compiler**: Si aparece error de Burst, borrar `Library/BurstCache` o desactivar Burst en Jobs > Burst > Enable Compilation

2. **TextMeshPro**: Si hay errores de TMP, importar recursos: Window > TextMeshPro > Import TMP Essential Resources

## Proximos Pasos

1. Abrir Unity y verificar que el juego corre (Play)
2. Ajustar UI para que se vea bien en formato movil (portrait)
3. Build > Android y probar en dispositivo
4. Crear cuenta AdMob e implementar anuncios
5. Preparar assets para Play Store (iconos, screenshots)
