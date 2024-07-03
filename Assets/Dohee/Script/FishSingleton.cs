using Cinemachine;
using Dohee;
using LeeInHae;
using UnityEngine;

public class FishSingleton : MonoBehaviour
{
    public static FishSingleton Singleton;

    [SerializeField] private CinemachineVirtualCamera VCam;
    private CinemachineConfiner2D con;
    private FishScale scale;

    private float size = 5;
    private float maxSize = 10;

    private void Awake()
    {
        Singleton = this;
        scale = GetComponent<FishScale>();

        SoundManager.Instance.Clear();
        SoundManager.Instance.Play("BGM/IngameBGM", Sound.Bgm);
    }

    private void Update()
    {
        if(!con && VCam) con = VCam.GetComponent<CinemachineConfiner2D>();

        float temp = size;
        size = Mathf.Clamp(scale.Scale * 5f, 0, maxSize);

        if(temp != size || size != VCam.m_Lens.OrthographicSize)
        {
            VCam.m_Lens.OrthographicSize = Mathf.Lerp(VCam.m_Lens.OrthographicSize, size, Time.deltaTime * 0.5f);
            con.InvalidateCache();
        }
    }

    private void OnDestroy()
    {
        Singleton = null;

        UIManager.Instance.RestartPanelOn();
    }

    public void FindCameraBound()
    {
        VCam.Follow = null;
        PolygonCollider2D camBound = GameObject.Find("CamBound").GetComponent<PolygonCollider2D>();
        VCam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = camBound;
        Camera.main.transform.position -= new Vector3(-5, 3);
        VCam.Follow = transform;

        maxSize = camBound.GetComponent<BoundInfo>().MaxSize;
        transform.position = GameObject.Find("PlayerSpawnPos").transform.position;
    }
}
