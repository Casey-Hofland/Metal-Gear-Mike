using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.Characters.FirstPerson;

public class InputControls : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    [SerializeField] private SkinnedMeshRenderer[] meshes;

    private Gun gun;
    private CinemachineVirtualCamera currentCamera;
    private new Camera camera;
    private CinemachineBrain cinemachineBrain;
    private FirstPersonController controller;
    private MeshRenderer[] birdViewMeshes;

    public bool HasBriefcase { private set; get; } = false;

    private void Awake()
    {
        gun = GetComponentInChildren<Gun>();
        currentCamera = cameras[0];
        camera = GetComponentInChildren<Camera>();
        cinemachineBrain = camera.GetComponent<CinemachineBrain>();
        controller = GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        birdViewMeshes = GameObject.FindGameObjectsWithTag("HiddenInBirdView")
            .Select(g => g.GetComponent<MeshRenderer>())
            .Where(m => m != null)
            .ToArray();

        StartCoroutine(SetMeshesAfter(0, ShadowCastingMode.ShadowsOnly, true));
        StartCoroutine(Intro());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) ChangeCamera();

        if (currentCamera != cameras[0]) return;

        if (Input.GetButton("Fire1"))
        {
            Ray fireRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            gun.Fire(fireRay);
        }
    }

    public void SetBriefcase()
    {
        HasBriefcase = true;
    }

    public void Die()
    {
        Destroy(controller);
        Destroy(this);
        SceneManagerController.instance.LoadNextScene();
    }

    private void ChangeCamera()
    {
        int priority = (cameras[1].Priority <= 9) ? 11 : 9;

        if (priority == 9)
        {
            currentCamera = cameras[0];
            float blendTime = cinemachineBrain.m_CustomBlends.m_CustomBlends[1].m_Blend.m_Time;
            StartCoroutine(ChangePerspective(blendTime / 2, false));
            StartCoroutine(SetMeshesAfter(blendTime, ShadowCastingMode.ShadowsOnly, true));
        }
        else
        {
            currentCamera = cameras[1];
            StartCoroutine(ChangePerspective(0, true));
            StartCoroutine(SetMeshesAfter(0, ShadowCastingMode.On, false));
        }

        cameras[1].Priority = priority;
    }

    IEnumerator SetMeshesAfter(float seconds, ShadowCastingMode shadowCastingMode, bool enableController)
    {
        yield return new WaitForSeconds(seconds);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.shadowCastingMode = shadowCastingMode;
        }

        ShadowCastingMode birdViewShadowCastingMode = (shadowCastingMode == ShadowCastingMode.On) ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
        SetBirdviewMeshes(birdViewShadowCastingMode);

        controller.enabled = enableController;

        yield return null;
    }

    IEnumerator ChangePerspective(float seconds, bool orthographic)
    {
        yield return new WaitForSeconds(seconds);

        camera.orthographic = orthographic;

        yield return null;
    }

    IEnumerator Intro()
    {
        ChangeCamera();

        yield return new WaitForSeconds(1.2f);

        ChangeCamera();

        yield return null;
    }

    private void SetBirdviewMeshes(ShadowCastingMode shadowCastingMode)
    {
        foreach (MeshRenderer birdViewMesh in birdViewMeshes)
        {
            birdViewMesh.shadowCastingMode = shadowCastingMode;
        }
    }
}
