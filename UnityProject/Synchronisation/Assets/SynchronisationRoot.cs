using Synchronisation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;

public class SynchronisationRoot : MonoBehaviour
{
    private SynchronisationClient _client = null;
    private CancellationTokenSource _cancellationTokenSource;

    public string ServerHost;
    public int ServerPort;
    public Transform SynchronizedObject;
    public GameObject SynchronizedObjectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _client = new SynchronisationClient();
        _cancellationTokenSource = new CancellationTokenSource();
        _client.UpdateReceived += OnUpdateReceived;
        _ = _client.Run(_cancellationTokenSource.Token)
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError($"Error in client: {t.Exception}");
                }
            });
        _client.ConnectToServer(ServerHost, ServerPort);
    }


    public float _lastSentTime;
    private byte[] _sendBuffer = new byte[sizeof(float) * 6];
    // Update is called once per frame
    void Update()
    {
        if (Time.time - _lastSentTime > 0.1f)
        {
            var span = _sendBuffer.AsSpan();
            BitConverter.TryWriteBytes(span[(0 * sizeof(float))..(1 * sizeof(float))], SynchronizedObject.position.x);
            BitConverter.TryWriteBytes(span[(1 * sizeof(float))..(2 * sizeof(float))], SynchronizedObject.position.y);
            BitConverter.TryWriteBytes(span[(2 * sizeof(float))..(3 * sizeof(float))], SynchronizedObject.position.z);
            BitConverter.TryWriteBytes(span[(3 * sizeof(float))..(4 * sizeof(float))], SynchronizedObject.eulerAngles.x);
            BitConverter.TryWriteBytes(span[(4 * sizeof(float))..(5 * sizeof(float))], SynchronizedObject.eulerAngles.y);
            BitConverter.TryWriteBytes(span[(5 * sizeof(float))..(6 * sizeof(float))], SynchronizedObject.eulerAngles.z);

            _client.SendUpdate(_sendBuffer);
            _lastSentTime = Time.time;
        }
    }

    void OnUpdateReceived(ushort clientId, Memory<byte> data)
    {

        var go = GameObject.Find(clientId.ToString());
        if (go == null)
        {
            go = Instantiate(SynchronizedObjectPrefab);
            go.name = clientId.ToString();
        }


        var span = data.Span;
        var transform = go.transform;


        transform.position = new(
            BitConverter.ToSingle(span[(0 * sizeof(float))..(1 * sizeof(float))]),
            BitConverter.ToSingle(span[(1 * sizeof(float))..(2 * sizeof(float))]),
            BitConverter.ToSingle(span[(2 * sizeof(float))..(3 * sizeof(float))])
            );

        transform.eulerAngles = new(
            BitConverter.ToSingle(span[(3 * sizeof(float))..(4 * sizeof(float))]),
            BitConverter.ToSingle(span[(4 * sizeof(float))..(5 * sizeof(float))]),
            BitConverter.ToSingle(span[(5 * sizeof(float))..(6 * sizeof(float))])
            );
    }

    private void OnApplicationQuit()
    {
        _cancellationTokenSource?.Cancel();
        _client?.Dispose();
    }
}
