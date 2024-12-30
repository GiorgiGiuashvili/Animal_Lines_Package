using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using System.Collections;
using UnityEngine.UI;

public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance { get; private set; }

    public List<GameObject> Set1;
    public List<GameObject> Set2;
    public List<GameObject> Set3;

    [Space]

    public Transform spawnPosition1;
    public Transform spawnPosition2;
    public Transform spawnPosition3;

    private string HappyAnimation = "Happy";
    private int placedObjectCount = 0;

    [Space]

    public GameObject FinishPanel;
    private GameObject lastSpawnedAnimal;

    [Space]
    private List<GameObject> spawnedAnimals = new List<GameObject>();
    public ExampleAnimalsAnimation _ExAnim;

    private AnimalLinesEntryPoint _entryPoint;
    [SerializeField] private Button homeButton;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        homeButton.onClick.AddListener(FinishOnButton);
    }
    private void FinishOnButton()
    {
        _entryPoint.InvokeGameFinished();
    }

    void Start()
    {
        SpawnNextObject();
    }

    public IEnumerator GameFinish()
    {
        _ExAnim.PlayAnimations();
        PlayAnimationOnAnyAnimal();

        yield return new WaitForSeconds(3f);

        FinishPanel.SetActive(true);
    }

    void SpawnNextObject()
    {
        if (Set1.Count == 0 && Set2.Count == 0 && Set3.Count == 0)
        {
            SetFinishForPackage();
/*            StartCoroutine(GameFinish());
*/            return;
        }

        if (Set1.Count > 0)
        {
            lastSpawnedAnimal = SpawnObject(Set1[0], spawnPosition1);
            Set1.RemoveAt(0);
        }

        if (Set2.Count > 0)
        {
            lastSpawnedAnimal = SpawnObject(Set2[0], spawnPosition2);
            Set2.RemoveAt(0);
        }

        if (Set3.Count > 0)
        {
            lastSpawnedAnimal = SpawnObject(Set3[0], spawnPosition3);
            Set3.RemoveAt(0);
        }
    }

    GameObject SpawnObject(GameObject animalPrefab, Transform spawnPosition)
    {
        if (animalPrefab != null && spawnPosition != null)
        {
            GameObject spawnedAnimal = Instantiate(animalPrefab, spawnPosition.position, Quaternion.identity);
            spawnedAnimals.Add(spawnedAnimal); 
            return spawnedAnimal;
        }
        return null;
    }

    void PlayAnimationOnAnyAnimal()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            TagValue tagValue = animal.GetComponent<TagValue>();
            if (tagValue != null && tagValue.Tag == "Animals") 
            {
                AnimalDrag animalDrag = animal.GetComponent<AnimalDrag>();
                if (animalDrag != null)
                {
                    animalDrag.SetSpineAnimation(HappyAnimation, true);
                }

                foreach (Transform child in animal.transform)
                {
                    tagValue = child.GetComponent<TagValue>();
                    if (tagValue != null && tagValue.Tag == "Animals")
                    {
                        animalDrag = child.GetComponent<AnimalDrag>();
                        if (animalDrag != null)
                        {
                            animalDrag.SetSpineAnimation(HappyAnimation, true);
                        }
                    }
                }
            }
        }
    }

    public void ObjectPlaced()
    {
        placedObjectCount++;

        if (placedObjectCount == 3)
        {
            placedObjectCount = 0;
            SpawnNextObject();
        }
    }

    public void SetEntryPoint(AnimalLinesEntryPoint entrypoint)
    {
        _entryPoint = entrypoint;
    }

    private void SetFinishForPackage()
    {
        StartCoroutine(FinishAfterFireworks());
    }

    private IEnumerator FinishAfterFireworks()
    {
        yield return new WaitForSecondsRealtime(5f);
        _entryPoint.InvokeGameFinished();
    }
}
