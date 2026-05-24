using System;
using UnityEngine;

[Serializable]
public class SubtitleLine
{
    [Tooltip("Время в секундах от начала воспроизведения (или от события)")]
    public float time;

    [Tooltip("Текст субтитра, который появится в это время")]
    [TextArea(2, 4)]
    public string text;
}