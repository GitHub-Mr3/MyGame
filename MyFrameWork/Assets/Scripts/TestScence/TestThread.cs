using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Threading;
using UnityEngine;


public class TestThread : MonoBehaviour
{
    // Start is called before the first frame update
    public Button Btn;
    private string serverData;
    private bool dataReady = false;

    void Start()
    {
        Btn.onClick.AddListener(() =>
        {
            int range = UnityEngine.Random.Range(1, 100);
            DebugMgr.Instance.Log("rang" + range);
            ThreadPool.QueueUserWorkItem(ProcessServerData, range);
        });
        // Simulate getting data from server in a separate thread

    }

    void Update()
    {
        // Check if data is ready and handle it in the main thread
        if (dataReady)
        {
            Debug.Log("Data received from server: " + serverData);
            // Reset flag after handling data
            dataReady = false;
        }
    }

    private void ProcessServerData(object state)
    {
        // Simulate getting data from server
        serverData = "Server response data: " + state;

        // Set flag to indicate data is ready
        dataReady = true;
    }

}
