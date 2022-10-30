using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// public class Utill{
// public static void Shuffle<T>(this IList<T> List)
// {
//     int n = List.Count;
//     while(n > 1)
//     {
//         n--;
//         int k = rng.Next(n + 1);
//         T value = list[k];
//         list[k] = list[n];
//         list[n] = value;
//     }
// }
// }


[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    AudioSource audioSource;
    public bool isMic;
    public float[] audioSamples;// = new float[512];
    public Transform objParent;
    [SerializeField]GameObject visualPrefab;
    public List<GameObject> visualInstancePool = new List<GameObject>();
    [SerializeField] float amplificationScale;
    string micName;
    [SerializeField] Material mat;
    [SerializeField] string propertyName;

    //private System.Random rng = new System.Random();


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if(isMic)
        {
            foreach (string device in Microphone.devices) {
                micName = device;
            }
            audioSource.clip = Microphone.Start(micName, true, 10, 44100);
        }
    
        audioSource.Play();

        if(objParent == null){
            audioSamples = new float[512];

            if(visualPrefab != null)
                InitializeVisualPrefab(visualInstancePool, visualPrefab, audioSamples.Length);

        }
        else
        {
            for(int i = 0 ; i < objParent.childCount ; i++)
            {
                if(objParent.GetChild(i).gameObject.activeSelf)
                {
                    if(visualInstancePool.Count >= 8192 - 1) break;
                    
                    visualInstancePool.Add(objParent.GetChild(i).gameObject);
                }
                    
            }


            audioSamples = new float[visualInstancePool.Count];

        }

        //StartCoroutine(CoStartBPM());
    }

    // WaitForSeconds bpm = new WaitForSeconds(0.01f); 
    // IEnumerator CoStartBPM()
    // {
    //     while(true)
    //     {
    //         GetSpectrumAudioSource();    

    //         yield return bpm;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource(); 

        mat.SetFloat(propertyName, audioSamples[0] * amplificationScale);
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(audioSamples, 0, FFTWindow.BlackmanHarris);

        //ApplySampleDataToPool(audioSamples, visualInstancePool);
    }

    void InitializeVisualPrefab(List<GameObject> _pool, GameObject _prefab, int num)
    {
        for(int i = 0 ; i < num ; i++){
            GameObject child = Instantiate(_prefab);
            child.transform.localPosition = Vector3.zero;
            child.name = $"SampleObj {i}";
            child.transform.SetParent(this.transform);
            
            this.transform.eulerAngles = new Vector3(0, 0.713f * i, 0);
            child.transform.position = Vector3.forward * 100;
            _pool.Add(child);
        }
    }

    void ApplySampleDataToPool(float[] _audioSamples, List<GameObject> _visualInstancePool){

        //_visualInstancePool.Shuffle();

        //var shuffledPool = _visualInstancePool.OrderBy(a => Guid.NewGuid()).ToList();

        for(int i = 0 ; i < _audioSamples.Length ; i++){
            if(_visualInstancePool == null) break;

            //print("작동중" + _audioSamples[i] * amplificationScale);
            _visualInstancePool[i].transform.localScale = new Vector3(1, (Mathf.Exp(_audioSamples[i]) * amplificationScale) + 2 , 1);
        }
    }



}
