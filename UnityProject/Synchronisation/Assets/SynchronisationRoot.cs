using Synchronisation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronisationRoot : MonoBehaviour
{
    private SynchronisationClient _client = new();

    public string ServerHost;
    public int ServerPort;

    // Start is called before the first frame update
    void Start()
    {
        _client.UpdateReceived += OnUpdateReceived;
        _client.ConnectToServer(ServerHost, ServerPort);
    }


    public float _lastSentTime;
    // Update is called once per frame
    void Update()
    {
        if(Time.time - _lastSentTime > 0.1f)
        {
            _client.SendUpdate(BitConverter.GetBytes(Time.time));
            _lastSentTime = Time.time;
        }
    }

    void OnUpdateReceived(ushort clientId, Memory<byte> data)
    {
        var time = BitConverter.ToSingle(data.Span);
        Debug.Log($"Received update from client {clientId} with time {time}");
    }
}
