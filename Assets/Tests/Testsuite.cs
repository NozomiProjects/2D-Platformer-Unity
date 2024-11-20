using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class Testsuite
{
    private GameManager gameManager;
    private PlayerController playerController;

    private GameObject playerObject;
    private Rigidbody2D rbPlayer;

    private GameObject coin;
    private pickup _coinCode;
    private BoxCollider2D collider;

    private GameObject movingPlatform; // Plataforma móvil
    private Vector3 initialPosition;  // Posición inicial de la plataforma
    private Vector3 targetPosition;   // Posición final de la plataforma

    [SetUp]
    public void SetUp()
    {
        // Crear un objeto de GameManager y simular su inicialización
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();

        // Crear un objeto de PlayerController
        playerObject = new GameObject("Player");
        playerController = playerObject.AddComponent<PlayerController>();
        rbPlayer = playerObject.AddComponent<Rigidbody2D>();
        rbPlayer.constraints = RigidbodyConstraints2D.FreezeRotation; // Evitar rotaciones
        CapsuleCollider2D playerCollider = playerObject.AddComponent<CapsuleCollider2D>();
        gameManager.playerController = playerController;

        // Crear el objeto moneda
        coin = new GameObject("Coin");
        _coinCode = coin.AddComponent<pickup>();
        collider = coin.AddComponent<BoxCollider2D>();

        // Crear la plataforma móvil
        movingPlatform = new GameObject("MovingPlatform");
        Rigidbody2D platformRb = movingPlatform.AddComponent<Rigidbody2D>();
        platformRb.bodyType = RigidbodyType2D.Kinematic; // Configurar como kinemático para movimiento controlado
        TilemapCollider2D tilemapCollider = movingPlatform.AddComponent<TilemapCollider2D>();
        CompositeCollider2D compositeCollider = movingPlatform.AddComponent<CompositeCollider2D>();
        tilemapCollider.usedByComposite = true; // Vincular Tilemap al CompositeCollider
        movingPlatform.layer = LayerMask.NameToLayer("ground"); // Asignar layer "Ground"

        // Configurar posiciones de la plataforma
        initialPosition = movingPlatform.transform.position;
        targetPosition = initialPosition + new Vector3(2, 0, 0); // Configurar destino de la plataforma

        // Posicionar al jugador sobre la plataforma
        playerObject.transform.position = new Vector3(0, 1, 0);
        playerObject.transform.SetParent(movingPlatform.transform); // Anclar al jugador a la plataforma

        // Inicializar el GameManager
        gameManager.Awake();
    }

    [TearDown]
    public void TearDown()
    {
        // Limpiar el entorno de pruebas
        Object.Destroy(gameManager);
        Object.Destroy(playerObject);
        Object.Destroy(coin);
        Object.Destroy(movingPlatform);
    }

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

    [UnityTest]
    public IEnumerator PlayerCollectsCoin_IncrementsCoinCount()
    {
        // Act
        coin.GetComponent<pickup>().OnTriggerEnter2D(playerObject.GetComponent<BoxCollider2D>()); // Simular la recolección de la moneda
        yield return null; // Esperar un frame para que se actualice

        // Assert
        Assert.AreEqual(1, gameManager.coinCount); // Verificar que el conteo de monedas sea 1
    }

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
}
