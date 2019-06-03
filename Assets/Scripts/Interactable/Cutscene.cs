using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cutscene : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] float showFor = 1.5f;
    [SerializeField] Camera camera;
    #pragma warning disable 0649

    private Camera mainCamera;

    private void Start()
    {
        Interactable interactble = GetComponent<Interactable>();
        if (interactble == null) throw new System.Exception("A Cutscene tried to find an Interactable but couldn't.");

        interactble.Subscribe(OnInteract);

        mainCamera = Player.Instance.GetComponentInChildren<Camera>();
    }

    public void OnInteract()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        //Debug.Log($"Main Camera: {mainCamera}\nCutscene Camera: {camera}");

        mainCamera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);

        NavMeshAgent[] navMeshAgents = EnemyManager.Instance.GetComponentsInChildren<NavMeshAgent>();

        foreach(NavMeshAgent agent in navMeshAgents)
        {
            agent.enabled = false;
        }

        yield return new WaitForSeconds(showFor);

        mainCamera.gameObject.SetActive(true);
        camera.gameObject.SetActive(false);

        foreach(NavMeshAgent agent in navMeshAgents)
        {
            if(agent != null) {
                agent.enabled = true;
            }   
        }
    }
}
