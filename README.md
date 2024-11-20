# Trabajo Práctico Final - Testing y Casos de Prueba

**[Documento](https://docs.google.com/document/d/1TrZ_DIOsMb3A-tq-7ddZgGXpnEocvC2q/edit?usp=sharing&ouid=104358231682201099483&rtpof=true&sd=true)**

## 1. Técnicas de Testing

### Técnicas de Testing para el Juego de Plataforma de Mapas de Mosaicos
- **Partición de Equivalencia**:
  - Separar acciones válidas (mover a la izquierda, derecha, saltar, recolectar monedas) de las no válidas (salir del límite del mapa, colisiones incorrectas).
  - Ejemplo: Validar que el personaje no atraviese paredes ni recolecte más de las monedas disponibles.

- **Análisis de Valores Límite**:
  - Probar los límites en variables como vidas y puntuación.
  - Ejemplo:
    - Verificar qué sucede al perder todas las vidas (fin del juego).
    - Completar el juego al recolectar todas las monedas.

- **Testing Negativo**:
  - Intentar acciones inesperadas:
    - Saltar más alto de lo permitido.
    - Caer fuera del mapa sin perder vida.
    - Superar la cantidad de monedas disponibles.

- **Reproducción de Fallos**:
  - Identificar y replicar fallos observados durante la jugabilidad.
  - Ejemplo: Probar diferentes ángulos para recoger una moneda que no se registra correctamente.

- **Árboles de Clasificación**:
  - Diseñar combinaciones de eventos como movimiento, salto y colisiones.
  - Ejemplo: Saltar hacia una pared justo al recoger una moneda y verificar el resultado.

---

## 2. Casos de Prueba

### Casos de Prueba para el Juego de Plataforma

| ID    | Funcionalidad                  | Pasos para reproducir                                                                                                                                      | Resultado esperado                                                                                 | Resultado obtenido                                                              |
|-------|--------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| 0001  | Iniciar Partida                | En el menú de juego presionar el botón Play                                                                                                                | Transición hacia la escena del nivel                                                              | Se cargó el nivel del juego                                                   |
| 0002  | Movimiento Horizontal del jugador | Iniciar el nivel. Presionar las teclas A y D o flechas para mover al personaje                                                                              | El personaje debería moverse hacia la izquierda o derecha                                         | El personaje se mueve en la dirección deseada                                  |
| 0003  | Salto simple del jugador       | Iniciar el nivel. Presionar la barra espaciadora para que el personaje salte                                                                                | El personaje debería saltar una sola vez                                                          | El personaje puede saltar más de una vez                                       |
| 0004  | Perder vida (HUD)              | Iniciar el nivel. Mover el personaje y hacerlo caer en un vacío                                                                                             | En el HUD, se debería descontar una vida y reiniciar el nivel                                      | En el HUD no se descuenta o muestra la pérdida de una vida                     |
| 0005  | Puntaje                        | Iniciar el nivel. Mover el personaje. Colisionar con monedas                                                                                                | El puntaje debería aumentar si el personaje colisiona con las monedas                              | El puntaje aumenta sin inconvenientes                                          |

---

## 3. Implementación de Pruebas Unitarias

Las pruebas se implementaron utilizando **NUnit** y el **Unity Test Framework** para garantizar la calidad y el correcto funcionamiento del juego.

### Juego de Plataforma
A continuación, se describen las pruebas unitarias realizadas.

---

## Tests Implementados

### 1. IncrementCoinCount_IncreasesCoinCount
#### Descripción
Verifica que el método `IncrementCoinCount` en el `GameManager` incremente correctamente el contador de monedas.

```csharp
[UnityTest]
public IEnumerator IncrementCoinCount_IncreasesCoinCount()
{
    // Arrange
    int initialCoinCount = gameManager.coinCount;

    // Act
    gameManager.IncrementCoinCount(); // Llamar al método para incrementar el conteo de monedas
    yield return null; // Esperar un frame para que se actualice

    // Assert
    Assert.AreEqual(initialCoinCount + 1, gameManager.coinCount); // Verificar que el conteo de monedas aumente
}
```

---

### 2. PlayerDeath_DisablesPlayerAndTriggersDeathCoroutine
#### Descripción
Comprueba que, al morir el jugador, su objeto se desactiva y se ejecuta la lógica de muerte.

```csharp
[UnityTest]
public IEnumerator PlayerDeath_DisablesPlayerAndTriggersDeathCoroutine()
{
    // Arrange
    playerController.gameObject.SetActive(true); // Asegurarse de que el jugador esté activo
    Assert.IsTrue(playerController.gameObject.activeSelf); // Verificar que el jugador esté activo

    // Act
    gameManager.Death(); // Llamar al método de muerte
    yield return new WaitForSeconds(1f); // Esperar un poco más que la duración de la coroutine

    // Assert
    Assert.IsFalse(playerController.gameObject.activeSelf); // Verificar que el jugador esté desactivado
}
```

---

### 3. PlayerCollectsCoin_IncrementsCoinCount
#### Descripción
Este test verifica que, al recolectar una moneda, el contador de monedas (`coinCount`) en el `GameManager` se incremente correctamente.

```csharp
[UnityTest]
public IEnumerator PlayerCollectsCoin_IncrementsCoinCount()
{
    // Act
    coin.GetComponent<pickup>().OnTriggerEnter2D(playerObject.GetComponent<BoxCollider2D>()); // Simular la recolección de la moneda
    yield return null; // Esperar un frame para que se actualice

    // Assert
    Assert.AreEqual(1, gameManager.coinCount); // Verificar que el conteo de monedas sea 1

    yield return null; // Pausa para asegurar que se complete el frame
}
```
---

### 4. PlayerMovementOnMovingPlatforms
#### Descripción
Este test verifica que el jugador se mueva correctamente junto con una plataforma móvil. Simula el movimiento de la plataforma desde una posición inicial hasta una posición final y comprueba que el jugador mantiene su posición relativa a la plataforma durante todo el movimiento.

```csharp
[UnityTest]
public IEnumerator PlayerMovementOnMovingPlatforms()
{
    // Act
    for (float t = 0; t <= 1f; t += Time.deltaTime)
    {
        movingPlatform.transform.position = Vector3.Lerp(initialPosition, targetPosition, t); // Simular movimiento de la plataforma
        yield return null; // Esperar un frame
    }

    // Assert
    Assert.AreEqual(movingPlatform.transform.position.x, playerObject.transform.position.x, 0.1f); // Verificar que el jugador sigue la plataforma
}
```
