using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцены
using System.Collections;

public class PaperAirplanePhysics : MonoBehaviour
{
    public float thrust; 
    public float turnSpeed;
    public float targetMass;
    public float windPushForce;
    public float linearDamping;
    public float cloudPushForce; // Добавлена переменная для силы толчка вниз от облака
    public RandomSpawner windRandomSpawner;

    private Rigidbody rb;
    private bool inCloudTrigger = false; // Флаг для проверки нахождения внутри триггера Cloud

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * thrust);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.AddTorque(transform.up * horizontal * turnSpeed);
        rb.AddTorque(transform.right * vertical * turnSpeed);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // Если внутри триггера Cloud, прикладываем толчок вниз
        if (inCloudTrigger)
        {
            rb.AddForce(Vector3.down * cloudPushForce, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wind"))
        {
            rb.AddForce(Vector3.up * windPushForce, ForceMode.Impulse);
            rb.mass = 0.1f;
            windRandomSpawner.RemoveAllSpawnedObjects();
            windRandomSpawner.SpawnObjectsInZones();
            Debug.Log("Толчок вверх от ветра!");
        }

        if (other.CompareTag("Cloud"))
        {
            inCloudTrigger = true; // Устанавливаем флаг нахождения в триггере Cloud
            Debug.Log("Вход в триггер Cloud. Толчки вниз активированы.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cloud"))
        {
            inCloudTrigger = false; // Сбрасываем флаг нахождения в триггере Cloud
            Debug.Log("Выход из триггера Cloud. Толчки вниз деактивированы.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            this.enabled = false;
            Debug.Log("Скрипт отключен из-за столкновения с Untagged объектом!");

            // Запускаем корутину для ожидания и загрузки сцены
            StartCoroutine(LoadMainMenuAfterDelay(3f));
        }
    }

    // Корутину для задержки перед загрузкой сцены
    private IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ждем указанное количество секунд
        SceneManager.LoadScene("MainMenu"); // Загружаем сцену MainMenu
    }
}
