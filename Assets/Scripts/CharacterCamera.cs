using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    static int numCameras = 0;
    public int id { get; private set; } = 0;

    GameObject character;
    void Awake() {
        id = numCameras++;
    }
    // Start is called before the first frame update
    void Start() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            if (go.GetComponent<CharacterControls>().id == id) {
                character = go;
            }
        }
        foreach (GameObject cam in GameObject.FindGameObjectsWithTag("MainCamera")) {
            if (cam != gameObject) {
                cam.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        Vector3 newPosition = new Vector3(character.transform.position.x, 0, -10);
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.1f / Time.deltaTime);
    }

    private void OnDestroy() {
        numCameras--;
    }
}
