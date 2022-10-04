using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class Testing : NetworkBehaviour
{
    [SerializeField] private Unit unit;
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData
    {
        _int = 56,
        _bool = true,
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            //test rpc << ----------------------------------------------------------------------
            TestServerRpc();


            // test random number << -------------------------------------------------------
           //randomNumber.Value = new MyCustomData
           //{
           //    _int = 10,
           //    _bool= false,
           //    message = "i am who is sending the message",
           //};
        }
        
    }

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("The one who is sending this message is: " + OwnerClientId);
    }
}
