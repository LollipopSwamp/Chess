using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class StockFishApi : MonoBehaviour
{
    public GameObject chessManager;

    public bool bestMoveUpdated = false;
    public Text OutPutText;

    public string outPut = "";
    public string bestMove = "";
    public static Process mProcess;

    [SerializeField] public string FENInput;
    [SerializeField] public string DepthInput;
    [SerializeField] public string TimeInput;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Setup();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>

    public void Setup()
    {
        // since the apk file is archived this code retreives the stockfish binary data and
        // creates a copy of it in the persistantdatapath location.
        #if UNITY_ANDROID
        string filepath = Application.persistentDataPath + "/" + "stockfish-10-armv7";
        if (!File.Exists(filepath))
        {
            WWW executable = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "stockfish-10-armv7");
            while (!executable.isDone)
            {
            }
            File.WriteAllBytes(filepath, executable.bytes);

            //change permissions via plugin
           
        }
        var plugin = new AndroidJavaClass("com.chessbattles.jeyasurya.consoleplugin.AndroidConsole");
            string command = "chmod 777 "+filepath;
            outPut = plugin.CallStatic<string>("ExecuteCommand",command);
            
        #else
        string filepath = Application.streamingAssetsPath+ "/" + "stockfish_10_x64.exe";
        #endif
        // creating the process and communicating with the engine
        mProcess = new Process();
        ProcessStartInfo si = new ProcessStartInfo()
        {
            FileName = filepath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        mProcess.StartInfo = si;
        mProcess.OutputDataReceived += new DataReceivedEventHandler(MProcess_OutputDataReceived);
        mProcess.Start();
        mProcess.BeginErrorReadLine();
        mProcess.BeginOutputReadLine();

        SendLine("uci");
        SendLine("isready");
        UnityEngine.Debug.Log("Stockfish setup done");

    }
    public void SetRating(int rating)
    {
        SendLine("setoption UCI_Elo " + rating.ToString()) ;
        UnityEngine.Debug.Log("Rating set to " + rating.ToString());
    }

    public void GetMove(){
        string Fen = FENInput;
        string DepthValue = DepthInput;
        string processTime = TimeInput;

        if(Fen==null || Fen == ""){
            UnityEngine.Debug.LogError("Enter proper Fen");
            outPut = "Enter proper Fen";
            return;
        }

        SendLine("position fen "+Fen);

        if(processTime != ""){
            SendLine("go movetime "+processTime);
        }
        else{
            SendLine("go depth "+DepthValue);
        }
        UnityEngine.Debug.Log("Stockfish done");
    }


    public void SendLine(string command) {
        mProcess.StandardInput.WriteLine(command);
        mProcess.StandardInput.Flush();
    }

    void MProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {

        //UnityEngine.Debug.Log("Output:"+e.Data);

        outPut = e.Data;
        UnityEngine.Debug.Log(outPut);
        if (outPut.Contains("bestmove"))
        {
            int startIndex = outPut.IndexOf(' ') + 1;
            int count = outPut.IndexOf(' ', startIndex) - startIndex;
            bestMove = outPut.Substring(startIndex, count);
            UnityEngine.Debug.Log("Best Move:" + bestMove);
            bestMoveUpdated = true;
            //chessManager.GetComponent<ChessManager>().MakeBestMove();
        }
    }
}
