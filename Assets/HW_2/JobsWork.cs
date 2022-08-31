using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class JobsWork : MonoBehaviour
{
    [SerializeField] private GameObject _prefabObject;
    [SerializeField] private int _objectsCount;
    [SerializeField] private float _spawnRadius;

    private NativeArray<int> _numbersArray;

    [ReadOnly]
    public NativeArray<Vector3> _positions;

    [ReadOnly]
    public NativeArray<Vector3> _velocities;

    [WriteOnly]
    public NativeArray<Vector3> _finalPositions;

    private TransformAccessArray _transformsArray;

    private NativeArray<int> _angles;

    private void Start()
    {
        //StartIJobTask();
        //StartIJobParallelForTask();
        PrepairIJobRotationTask();
    }

    private void Update()
    {
        StartIJobRotationTask();
    }

    private void StartIJobTask()
    {
        _numbersArray = new NativeArray<int>(new int[] { 5, 7, 12, 53, 4, 18, 8 }, Allocator.Persistent);

        JobStruct jobStruct = new JobStruct()
        {
            Array = _numbersArray
        };

        JobHandle jobHandle = jobStruct.Schedule();
        jobHandle.Complete();

        _numbersArray.Dispose();
    }

    private void StartIJobParallelForTask()
    {        
        _positions = new NativeArray<Vector3>(new Vector3[] { new Vector3(2, 3, 4), new Vector3(4, 5, 8) }, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(new Vector3[] { new Vector3(5, 2, 8), new Vector3(5, 3, 1) }, Allocator.Persistent);
        _finalPositions = new NativeArray<Vector3>(_positions.Length, Allocator.Persistent);

        AnotherJobStruct anotherJobStruct = new AnotherJobStruct()
        {
            Positions = _positions,
            Velocities = _velocities,
            FinalPositions = _finalPositions
        };

        JobHandle jobHandle = anotherJobStruct.Schedule(2, 0);
        jobHandle.Complete();

        _positions.Dispose();
        _velocities.Dispose();
        _finalPositions.Dispose();
    }

    private void PrepairIJobRotationTask()
    {
        _angles = new NativeArray<int>(_objectsCount, Allocator.Persistent);
        var transforms = new Transform[_objectsCount];

        for (int i = 0; i < _objectsCount; i++)
        {
            var objectTransform = Instantiate(_prefabObject).transform;
            objectTransform.position = Random.insideUnitSphere * _spawnRadius;
            transforms[i] = objectTransform;
            _angles[i] = Random.Range(0, 180);
        }

        _transformsArray = new TransformAccessArray(transforms);
    }

    private void StartIJobRotationTask()
    {
        RotationJobStruct rotationJobStruct = new RotationJobStruct()
        {
            Angles = _angles
        };

        JobHandle jobHandle = rotationJobStruct.Schedule(_transformsArray);
        jobHandle.Complete();        
    }

    public struct JobStruct : IJob
    {
        public NativeArray<int> Array;

        public void Execute() 
        {
            Debug.Log($"{Array[0]}, {Array[1]}, {Array[2]}, {Array[3]}, {Array[4]}, {Array[5]}, {Array[6]},");

            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] > 10)
                {
                    Array[i] = 0;
                }
            }

            Debug.Log($"{Array[0]}, {Array[1]}, {Array[2]}, {Array[3]}, {Array[4]}, {Array[5]}, {Array[6]},"); 
        }
    }

    public struct AnotherJobStruct : IJobParallelFor
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> FinalPositions;

        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
            Debug.Log(FinalPositions[index]);
        }
    }

    public struct RotationJobStruct : IJobParallelForTransform
    {
        public NativeArray<int> Angles;

        public void Execute(int index, TransformAccess transform)
        {            
            transform.rotation = Quaternion.AngleAxis(Angles[index], Vector3.up);
            Angles[index] = Angles[index] == 180 ? 0 : Angles[index] + 1;
        }
    }

    private void OnDestroy()
    {
        _transformsArray.Dispose();
        _angles.Dispose();
    }

}
