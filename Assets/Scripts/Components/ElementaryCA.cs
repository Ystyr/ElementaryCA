using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a Unity3D implementation of a famous S. Wolfram's work - Elementary Cellular Automaton (ECA)
/// https://en.wikipedia.org/wiki/Elementary_cellular_automaton. 
/// The neat thing about this implementation is that it calculates the next step of CA 
/// based on a rule encoded in a single byte. This way it can implement 256 rules in just a few lines of code via binary operations
/// </summary>
public class ElementaryCA : MonoBehaviour
{
    #pragma warning disable 0649 
    [SerializeField]
    CAVisualizer visualizer;

    [Tooltip("Byte representation of ECA rule")]
    [SerializeField]
    byte rule;

    [Tooltip("Initial state of CA")]
    [SerializeField]
    bool[] initState;

    [Tooltip("Setup pool of rules which you want to explore")]
    [SerializeField]
    byte[] rulesPool;

    [SerializeField]
    PlayMode playMode;

    [Tooltip("Clear and start over (works at editor time)")]
    [SerializeField]
    bool restart;

    [Tooltip("Make a step of CA (works in editor time)")]
    [SerializeField]
    bool refresh;

    [Tooltip("Debug rule bits")]
    [SerializeField]
    bool logRule;

    [Tooltip("Frequency of CA steps per second")]
    [SerializeField]
    float updateRate = .1f;
    #pragma warning restore 0649

    [Serializable]
    public enum PlayMode { 
        PlaySingle, PlayAll, PlayPool 
    }

    public const int longBitsNum = 64;

    public const int maxHistLen = 96;

    List<ulong> history;

    ulong initConfig;

    int currentRule;

    private void OnValidate () 
    {
        if (initState == null || initState.Length != 64) {
            initState = new bool[64];
        }
        for (int i = 0; i < initState.Length; i++) {
            var bit = Convert.ToUInt64(initState[i]);
            initConfig = (initConfig & ~(1ul << i)) | (bit << i); 
        }
        if (restart) {
            restart = false;
            history = null;
            refresh = true;
            Start();
        }
        if (refresh) {        
            if (history == null) {
                history = new List<ulong>();
                history.Add(initConfig);
                RefreshVisualizer();
            }
            else {
                UpdateCA();
                RefreshVisualizer();
            }
            refresh = false;
        }
        if (logRule) {
            logRule = false;
            LogRuleBits();
        }
    }


    private void Start () 
    {
        history = new List<ulong>();
        history.Add(initConfig);
        
        StartCoroutine(Play());
    }

    /// <summary>
    /// The main part is here
    /// </summary>
    private void UpdateCA () 
    {
        var nextState = 0ul;
        var currentState = history[history.Count - 1];
        for (byte i = 1; i < longBitsNum - 1; i++) {
            byte neigbhd = 0;
            ///!@ set 3 bits representing the number of a bit in the rule
            for (sbyte b = -1; b <= 1; b++) {
                if (BitUtils.GetULongBit(currentState, i + b))
                    neigbhd |= (byte)(1 << b + 1);
            }
            ///!@ set i-th bit of the next state
            if (BitUtils.GetShortBit(rule, neigbhd))  
                nextState |= 1ul << i;
        }
        history.Add(nextState);
    }
    
    /// <summary>
    /// Generate data for the output texture
    /// </summary>
    private void RefreshVisualizer () 
    {
        var values = new bool[longBitsNum * maxHistLen];
        var histLength = history.Count;
        for (int i = 0; i < histLength; i++) {
            for (int j = 0; j < longBitsNum - 1; j++) {
                var idx = j + (((histLength - 1) - i) * longBitsNum);
                values[idx] = BitUtils.GetULongBit(history[i], j);
            }
        }
        visualizer.RefreshTex(values);
    }


    private void LogRuleBits () {
        var output = "";
        for (byte i = 0; i < 8; i++) {
            var bit = BitUtils.GetShortBit(rule, i);
            output += bit ? 1 : 0;
        } 
        Debug.Log("rule bits: " + output);
    }

    private void UpdateRule() {
        switch (playMode)
        {
            case PlayMode.PlayAll:
                rule++;
                break;
            case PlayMode.PlayPool:
                rule = rulesPool[currentRule++];
                break;
        }
    }

    private IEnumerator Play ()
    {
        yield return new WaitForSeconds(updateRate);
        if (history.Count == maxHistLen - 1) {
            if (playMode == PlayMode.PlaySingle)
                yield break;
            history = new List<ulong>();
            history.Add(initConfig);
            UpdateRule();
        }
        UpdateCA();
        RefreshVisualizer();
        StartCoroutine(Play());
    }
}
