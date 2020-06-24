using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerakPeluruScript : MonoBehaviour
{
    private Transform myTransform;
    public float waktuTerbangPeluru;

    private TankBehaviourScript tankBehaviour;
    private float _kecAwal;
    private float _sudutTembak;
    private float _sudutMeriam;
    private float _gravitasi;
    private Vector3 _posisiAwal;
    private AudioSource audioSource;

    public GameObject ledakan;
    public AudioClip audioLedakan;

    public GameManagerScript gameManager;
    private bool isLanded = true;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;

        //pastikan tankbehaviourscript ini diambil nilainya dari tankbehaviourscript
        //yang ada di objek torque
        tankBehaviour = GameObject.FindObjectOfType<TankBehaviourScript>();
        _kecAwal = tankBehaviour.kecepatanAwalPeluru;
        _sudutTembak = tankBehaviour.nilaiRotasiY;
        _sudutMeriam = tankBehaviour.sudutMeriam;

        _posisiAwal = myTransform.position;

        //init audiosource
        audioSource = GetComponent<AudioSource>();

        //init gravity
        _gravitasi = GameObject.FindObjectOfType<TankBehaviourScript>().gravity;

        //init gamemanager
        gameManager = GameObject.FindObjectOfType<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //timer
        if (isLanded)
        waktuTerbangPeluru += Time.deltaTime;

        gameManager._lamaWaktuTerbang = this.waktuTerbangPeluru;

        myTransform.position = PosisiTerbangPeluru(_posisiAwal, _kecAwal, waktuTerbangPeluru, _sudutTembak, _sudutMeriam);
    }

    private Vector3 PosisiTerbangPeluru(Vector3 _posisiAwal, float _kecAwal, float _waktu,
        float _sudutTembak, float _sudutMeriam)
    {
        float _x = _posisiAwal.x + (_kecAwal * _waktu * Mathf.Sin(_sudutMeriam * Mathf.PI / 180)); ;
        float _y = _posisiAwal.y + ((_kecAwal * _waktu * Mathf.Sin(_sudutTembak * Mathf.PI / 180)) - (0.5f * _gravitasi * Mathf.Pow(_waktu, 2)));
        float _z = _posisiAwal.z + (_kecAwal * _waktu * Mathf.Cos(_sudutMeriam * Mathf.PI / 180));

        return new Vector3(_x, _y, _z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            //hancurkan peluru
            Destroy(this.gameObject, 2f);

            //munculkan efek ledakan
            GameObject go = Instantiate(ledakan, myTransform.position, Quaternion.identity);
            Destroy(go, 3f);
            //munculkan audio ledakan
            audioSource.PlayOneShot(audioLedakan);

            gameManager._jarakTembak = Vector3.Distance(_posisiAwal, myTransform.position);

            isLanded = false;
        }
    }

}
