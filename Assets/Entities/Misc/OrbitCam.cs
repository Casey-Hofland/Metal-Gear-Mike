using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OrbitCam : MonoBehaviour
{
    [SerializeField] private float turnSpeed;

    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_Heading.m_Bias += turnSpeed * Time.deltaTime;
    }
}
