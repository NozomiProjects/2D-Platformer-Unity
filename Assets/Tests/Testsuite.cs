using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Testsuite
{
    private GameManager gameManager;
    private PlayerController playerController;
    private GameObject playerObject;

    [SetUp]
    public void SetUp()
    {
        // Crear un objeto de GameManager y simular su inicializaci�n
        GameObject gameManagerObject = new GameObject();
        gameManager = gameManagerObject.AddComponent<GameManager>();

        // Crear un objeto de PlayerController
        playerObject = new GameObject();
        playerController = playerObject.AddComponent<PlayerController>();
        gameManager.playerController = playerController;

        // Inicializar el GameManager
        gameManager.Awake();
    }

    [UnityTest]
    public IEnumerator IncrementCoinCount_IncreasesCoinCount()
    {
        // Arrange
        int initialCoinCount = gameManager.coinCount;

        // Act
        gameManager.IncrementCoinCount(); // Llamar al m�todo para incrementar el conteo de monedas
        yield return null; // Esperar un frame para que se actualice

        // Assert
        Assert.AreEqual(initialCoinCount + 1, gameManager.coinCount); // Verificar que el conteo de monedas aumente
    }

    [UnityTest]
    public IEnumerator PlayerDeath_DisablesPlayerAndTriggersDeathCoroutine()
    {
        // Arrange
        playerController.gameObject.SetActive(true); // Asegurarse de que el jugador est� activo
        Assert.IsTrue(playerController.gameObject.activeSelf); // Verificar que el jugador est� activo

        // Act
        gameManager.Death(); // Llamar al m�todo de muerte
        yield return new WaitForSeconds(1.1f); // Esperar un poco m�s que la duraci�n de la coroutine

        // Assert
        Assert.IsFalse(playerController.gameObject.activeSelf); // Verificar que el jugador est� desactivado
    }

    [UnityTest]
    public IEnumerator PlayerCollectsCoin_IncrementsCoinCount()
    {
        // Arrange
        GameObject coinObject = new GameObject();
        pickup coinPickup = coinObject.AddComponent<pickup>();
        coinPickup.pt = pickup.pickupType.coin;

        // Act
        coinPickup.OnTriggerEnter2D(playerObject.GetComponent<Collider2D>()); // Simular la recolecci�n de la moneda
        yield return null; // Esperar un frame para que se actualice

        // Assert
        Assert.AreEqual(1, gameManager.coinCount); // Verificar que el conteo de monedas sea 1
    }

    [TearDown]
    public void TearDown()
    {
        // Limpiar el entorno de pruebas
        Object.Destroy(gameManager);
        Object.Destroy(playerObject);
    }
}
