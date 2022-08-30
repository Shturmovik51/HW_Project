using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class AsyncAwait : MonoBehaviour
{
    [SerializeField] private int _firstCounDownValue;
    [SerializeField] private int _secondCountDown;

    private CancellationTokenSource _tokenSourceForFirstTask;
    private CancellationTokenSource _tokenSourceForSecondTask;
    private CancellationTokenSource _tokenSourceForCountDown;
    private CancellationToken _tokenForFirstTask;
    private CancellationToken _tokenForSecondTask;
    private CancellationToken _tokenForCountDown;


    private void Start()
    {
        _tokenSourceForFirstTask = new CancellationTokenSource();
        _tokenSourceForSecondTask = new CancellationTokenSource();
        _tokenSourceForCountDown = new CancellationTokenSource();
        _tokenForFirstTask = _tokenSourceForFirstTask.Token;
        _tokenForSecondTask = _tokenSourceForSecondTask.Token;
        _tokenForCountDown = _tokenSourceForCountDown.Token;

        //_ = FirstTask(_tokenForFirstTask);                    // закомментил, чтобы в дебаге не мешались,
        //_ = SecondTask(_tokenForSecondTask);                  // тут и так все понятно

        var countDownTask1 = FirstCountDown(_tokenForCountDown);
        var countDownTask2 = SecondCountDown(_tokenForCountDown);

        _ = WhatTaskFasterAsync(countDownTask1, countDownTask2, _tokenSourceForCountDown);
    }

    private async Task FirstTask(CancellationToken token)
    {
        var secondsCount = 10f;
        while (secondsCount > 0)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("первая задача прервана");
                return;
            }

            await Task.Yield();
            secondsCount -= Time.deltaTime;           
        }

        Debug.Log("первая задача завершена");
    }

    private async Task SecondTask(CancellationToken token)
    {
        var frameCount = 60;
        while (frameCount > 0)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("вторая задача прервана");
                return;
            }

            await Task.Yield();
            frameCount--;
        }
        Debug.Log("вторая задача завершена");
    }

    public async Task WhatTaskFasterAsync(Task<bool> task1, Task<bool> task2, CancellationTokenSource tokenSource)
    {
        await Task.WhenAny(task1, task2);
        tokenSource.Cancel();
        tokenSource.Dispose();
        await Task.WhenAll(task1, task2);
        Debug.Log($"Первый отсчет закончился = {task1.Result}");
        Debug.Log($"Второй отсчет закончился = {task2.Result}");
    }


    private async Task<bool> FirstCountDown(CancellationToken token)
    {
        var maxCount = _firstCounDownValue;
        var isInProcess = true;

        while (isInProcess)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("первый отсчет прерван");
                return false;
            }

            await Task.Yield();
            _firstCounDownValue--;

            if(_firstCounDownValue < 0)
            {
                _firstCounDownValue = maxCount;
                isInProcess = false;
            }
        }
        Debug.Log("первый отсчет закончен");
        return true;        
    }

    private async Task<bool> SecondCountDown(CancellationToken token)
    {
        var maxCount = _secondCountDown;
        var isInProcess = true;

        while (isInProcess)
        {
            if (token.IsCancellationRequested)
            {
                Debug.Log("второй отсчет прерван");
                return false;
            }

            await Task.Yield();
            _secondCountDown--;

            if (_secondCountDown < 0)
            {
                _secondCountDown = maxCount;
                isInProcess = false;
            }
        }
        Debug.Log("второй отсчет закончен");
        return true;
    }

    private void DisposeTokenSource(CancellationTokenSource tokenSource)
    {
        if (_tokenSourceForFirstTask != null)
        {
            tokenSource.Dispose();
        }
    }

    private void OnDestroy()
    {
        DisposeTokenSource(_tokenSourceForFirstTask);
        DisposeTokenSource(_tokenSourceForSecondTask);
        DisposeTokenSource(_tokenSourceForCountDown);
    }
}
