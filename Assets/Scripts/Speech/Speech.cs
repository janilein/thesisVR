using NAudio.Wave;
using Procurios.Public;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;



public class Speech : MonoBehaviour {
    //private KeywordParser keywordParser;

    private const string URL = "http://api.meaningcloud.com/topics-2.0";
    public string APIKeyMeaningCloud;
    private const string lang = "en";
    private const string tt = "ecnr"; //named entities, concepts, quantities and relations
    private const string ud = "ThesisVR";
    public string googleOutputText = "";
    private string meaningCloudOutput = "";

    private BufferedWaveProvider bwp;
    public string apiKeyGoogle;

    private Text projectionText;
	private InputField menuText;

    WaveIn waveIn;
    //WaveOut waveOut;
    WaveFileWriter writer;
    WaveFileReader reader;
    string output = "audio.raw";

    KeywordParser keywordParser;

    //Used to specify descriptions. E.g. first a house, then specify floor colours
    public static bool specifyDescription = false;

    bool workDone = false;
    private bool automateProcess = false;
    private bool updateProjectorText = false;

    private void Awake()
    {
		GameObject canvasHolder = GameObject.Find ("TheRoom/Projector Screen/CanvasHolder");
		if (canvasHolder) {
			projectionText = canvasHolder.GetComponent<Text>();
			if(projectionText == null)
			{
				Debug.Log("Could not find a projector!");
			}
		} else {
			Debug.LogError ("No Canvasholder found in TheRoom!");
		}
		
		GameObject textGameObject = GameObject.Find("GUI/Canvas/SpeechButtons/InputText");
		if(textGameObject){
			menuText = textGameObject.GetComponent<InputField>();
		} else {
			Debug.LogError("No InputField element found in Speech Buttons!");
		}
    }

    // Use this for initialization
    void Start () {
        keywordParser = new KeywordParser();
        //waveOut = new WaveOut();
        waveIn = new WaveIn();

        waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);
        waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
        bwp = new BufferedWaveProvider(waveIn.WaveFormat);
        bwp.DiscardOnBufferOverflow = true;

        //btnRecordVoice.Enabled = true;
        //btnSave.Enabled = false;
    }

    void Update()
    {
        if (workDone)
        {
            workDone = false;
            UseKeywordParser(meaningCloudOutput);
        }
        if (updateProjectorText)
        {
			
            updateProjectorText = false;
            if (projectionText)
                projectionText.text = googleOutputText;
			
			if(menuText){
				menuText.text = googleOutputText;
			}
        }
    }

    void WaveIn_DataAvailable(object sender, WaveInEventArgs e) {
        bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);

    }

    public void BtnRecordVoice_Click() {
		if (NAudio.Wave.WaveIn.DeviceCount < 1) {
			Console.WriteLine("No microphone!");
			return;
		}

        if(waveIn == null)
        {
            //waveOut = new WaveOut();
            waveIn = new WaveIn();
            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
        }
			
        waveIn.StartRecording();

        //btnRecordVoice.Enabled = false;
        //btnSave.Enabled = true;

        //btnSpeechInfo.Enabled = false;

    }

    public void BtnSave_Click() {
        waveIn.StopRecording();

        if (File.Exists("audio.raw"))
        {            
            File.Delete("audio.raw");
        }


        writer = new WaveFileWriter(output, waveIn.WaveFormat);

        //btnRecordVoice.Enabled = false;
        //btnSave.Enabled = false;

        //btnSpeechInfo.Enabled = true;

        byte[] buffer = new byte[bwp.BufferLength];
        int offset = 0;
        int count = bwp.BufferLength;

        var read = bwp.Read(buffer, offset, count);
        if (count > 0) {
            writer.Write(buffer, offset, read);
        }
        waveIn.Dispose();
        waveIn = null;
        writer.Dispose();
        writer.Close();
        writer = null;

        //BtnSpeechInfo_Click();

        //reader = new WaveFileReader("audio.raw"); // (new MemoryStream(bytes));
        //waveOut.Init(reader);
        //waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(WaveOut_PlaybackStopped);
        //Debug.Log("§§§§§§§§§§§§§§§§§§§§§§§§§§event created");
        //waveOut.Play();
        //Debug.Log("§§§§§§§§§§§§§§§§§§§§§§§§§§reader started");
    }

    //private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e) {
    //    waveOut.Stop();
    //    waveOut = null;
    //    reader.Dispose();
    //    reader.Close();
    //    reader = null;
    //    Debug.Log("§§§§§§§§§§§§§§§§§§§§§§§§§§reader closed");
    //}

    private bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None) {
            for (int i = 0; i < chain.ChainStatus.Length; i++) {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid) {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }

    public void BtnSpeechInfo_Click() {

        //btnRecordVoice.Enabled = true;
        //btnSave.Enabled = false;

        //btnSpeechInfo.Enabled = false;

        if (File.Exists("audio.raw")) {

            string jsonString = "{\"config\" : {";
            //jsonString += "\"encoding\" : \"FLAC\",";
            jsonString += "\"encoding\" : \"LINEAR16\",";
            jsonString += "\"sampleRateHertz\" : 16000,";
            jsonString += "\"languageCode\" : \"en-US\"";
            jsonString += "},";

            jsonString += "\"audio\" : {";
            //jsonString += "\"uri\" :";
            //jsonString += "\"gs://cloud-samples-tests/speech/brooklyn.flac\"";
            jsonString += "\"content\" :";
            byte[] bytes = File.ReadAllBytes("audio.raw");
            String base64String = Convert.ToBase64String(bytes);
            jsonString += "\"" + base64String + "\"";

            jsonString += "}}";

            //textBox1.Text = textBox1.Text + Environment.NewLine + "Json object to string: ";
            //textBox1.Text = textBox1.Text + Environment.NewLine + jsonString;

            //var req = (HttpWebRequest)WebRequest.Create("https://speech.googleapis.com/v1/speech:recognize?key=" + apiKeyGoogle);
            //req.ContentType = "application/json";
            //req.Method = "POST";
            //string text = "";
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            //using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
            //    streamWriter.Write(jsonString);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}

            try {
				using(WebClient wc = new WebClient()){;
	                wc.UploadStringCompleted += new UploadStringCompletedEventHandler(GoogleCallFinished);
	                wc.Headers["Content-Type"] = "application/json";
	                wc.UploadStringAsync(new Uri("https://speech.googleapis.com/v1/speech:recognize?key=" + apiKeyGoogle), "POST", jsonString);
                    
				}

                //var httpResponse = (HttpWebResponse)req.GetResponse();
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                //    string result = streamReader.ReadToEnd();
                //    text = text + Environment.NewLine + result;
                //    googleOutputText = text;
                //    Debug.Log(text);

                //    bool successParse = true;
                //    Hashtable o = (Hashtable)JSON.JsonDecode(googleOutputText, ref successParse);
                //    googleOutputText = GetTranscript(o).ToLower(); ;

                //}
                //} catch (System.Net.WebException exc) {
                //    using (var stream = exc.Response.GetResponseStream())
                //    using (var reader = new StreamReader(stream)) {
                //        text = text + Environment.NewLine + "Error: " + reader.ReadToEnd();
                //        Debug.Log(text);
                //        googleOutputText = null;
                //    }
            } catch (Exception exc) {
                string text = "Other error: " + exc.ToString();
                Debug.Log(text);
                googleOutputText = null;
            }

        } else {
            Debug.Log("Audio File Missing ");
        }
    }

    public void EndAndProcessRecording()
    {
        automateProcess = true;
        BtnSave_Click();
        BtnSpeechInfo_Click();

        //Debug.Log("Main thread PID: " + Thread.CurrentThread.ManagedThreadId);

        //Start a new thread here as the web calls will block the main unity thread
        //if (!threadRunning && !workDone)    //Only allow to create a new thread when the output of the previous one has not been processed yet
        //{
        //    threadRunning = true;
        //    thread = new Thread(ThreadedWork);
        //    thread.Start();
        //}
    }

//    private void ThreadedWork()
//    {
//        Debug.Log("Work thread PID: " + Thread.CurrentThread.ManagedThreadId);
//        BtnSave_Click();
//        BtnSpeechInfo_Click();
//        MakeRequest();
//        threadRunning = false;
//        workDone = true;
//    }

    //OnDisable we should wait for the thread to end as well
    //void OnDisable()
    //{
    //    if (threadRunning)
    //    {
    //        threadRunning = false;
    //        thread.Join();
    //    }
    //}

    public void MakeRequest() {

        //for testing
        //googleOutputText = "I went left.";
        //googleOutputText = "I was walking on a long street.";

        if (googleOutputText != null || googleOutputText.Length != 0) {
            //var req = (HttpWebRequest)WebRequest.Create("http://api.meaningcloud.com/topics-2.0");
            //req.ContentType = "application/x-www-form-urlencoded";
            //req.Method = "POST";
            string body = "key=" + APIKeyMeaningCloud + "&lang=" + lang + "&txt=" + googleOutputText + "&tt=" + tt + "&ud=" + ud;
            //using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
            //    streamWriter.Write(body);
            //    streamWriter.Flush();
            //    streamWriter.Close();
            //}

            try {
                Debug.Log("Starting MeaningCloud Request");
                //var httpResponse = (HttpWebResponse)req.GetResponse();
				using(WebClient wc = new WebClient()){
				//WebClient wc = new WebClient();
	                wc.UploadStringCompleted += new UploadStringCompletedEventHandler(MeaningCloudCallFinished);
	                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";


	                wc.UploadStringAsync(new Uri("http://api.meaningcloud.com/topics-2.0"), "POST", body);
				}

                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                //    meaningCloudOutput = streamReader.ReadToEnd();
                //    SaveToTxt(meaningCloudOutput);
                //    Debug.Log(meaningCloudOutput);
                //    //UseKeywordParser(meaningCloudOutput);
                //}
            //} catch (System.Net.WebException exc) {
            //    using (var stream = exc.Response.GetResponseStream())
            //    using (var reader = new StreamReader(stream)) {
            //        string text = "Error: " + reader.ReadToEnd();
            //        Debug.Log(text);
            //    }
            } catch (Exception exc) {
                string text = "Other error: " + exc.ToString();
                Debug.Log(text);
            }
        } else {
            Debug.Log("Nothing to send to MeaningCloud");
        }

    }

    public void MeaningCloudCallFinished(object sender, UploadStringCompletedEventArgs e)
    {
		
        Debug.Log("Meaningcloud call finished!");
        if (e.Error != null)
        {
            Debug.Log("Error: " + e.Error.Message);
            return;
        }

        meaningCloudOutput = e.Result.ToString();
        Debug.Log("Result");
        Debug.Log(meaningCloudOutput);
        SaveToTxt(meaningCloudOutput);

        if (automateProcess) {
            workDone = true;
            automateProcess = false;
        }
    }

    public void GoogleCallFinished(object sender, UploadStringCompletedEventArgs e)
    {
        Debug.Log("Google call finished!");
        if (e.Error != null)
        {
            googleOutputText = null;
            Debug.LogError("Error: " + e.Error.Message);
            return;
        }

        googleOutputText = e.Result.ToString();

        //Debug.LogError("Net na call");
        bool successParse = false;
        Hashtable o = (Hashtable)JSON.JsonDecode(googleOutputText, ref successParse);
        googleOutputText = GetTranscript(o).ToLower();

        //Check if it contains "one" and convert it to "1"
        googleOutputText = googleOutputText.Replace("one", "1");

        Debug.Log("Result");
        Debug.Log(googleOutputText);

        updateProjectorText = true;

        if (automateProcess)
            MakeRequest();
    }

    private void SaveToTxt(string line) {
        if (File.Exists("Assets/Scripts/Speech/meaningCloudOutput.txt"))
            File.Delete("Assets/Scripts/Speech/meaningCloudOutput.txt");
        System.IO.File.WriteAllText("Assets/Scripts/Speech/meaningCloudOutput.txt", line);
    }

    public void SkipMeaningCloud()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Scripts/Speech");
        try
        {
            FileInfo[] info = dir.GetFiles("meaningCloudOutput.txt");
            FileInfo f = info[0];
            string path = f.FullName;

            //Only read strings that have actual content
            string[] stringArray = System.IO.File.ReadAllLines(path);
            string inputText = stringArray[0];
            Debug.Log("inputText: " + inputText);

            UseKeywordParser(inputText);

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void UseKeywordParser(string meaningCloudOutput)
    {
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(meaningCloudOutput, ref successParse);
		if (keywordParser == null) {
			Debug.LogError ("KeywordParser is null");
			return;
		}
        keywordParser.ConvertHashtable(o);

    }

    public void ToggleSpecification()
    {
        GameObject specifyButton = GameObject.Find("GUI/Canvas/SpeechButtons/SpecifyButton").gameObject;
        specifyDescription = specifyButton.GetComponent<Toggle>().isOn;
        Debug.Log("Toggled specification");

        //If specifyDescription gets disabled, pass this to KeywordParser --> TextToJSON so it can spawn the fully described entity
        if(specifyDescription == false)
        {
            keywordParser.disabledSpecifyDescription();
        }
    }

    public static void SetSpecification(bool value)
    {
        GameObject specifyButton = GameObject.Find("GUI/Canvas/SpeechButtons/SpecifyButton").gameObject;
        specifyButton.GetComponent<Toggle>().isOn = value;
    }

    public static bool GetSpecification()
    {
        return specifyDescription;
    }

    private string GetTranscript(Hashtable googleOutput) {
        foreach(DictionaryEntry entry in googleOutput) {
            Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());


            if (entry.Key.Equals("transcript")) {
                return (string) entry.Value;
            }

            try {
                ArrayList attrList = (ArrayList)entry.Value;
                return GetTranscript((Hashtable)attrList[0]);
            } catch (Exception e) {
                Debug.Log("Could not cast to ArrayList");
                Debug.LogError(e.Message);
            }
        }
        return null;
    }
	
	public void MenuTextChanged(){
		googleOutputText = menuText.text;
	}

	private void OnApplicationQuit(){
		if (waveIn != null) {
			waveIn.Dispose ();
			waveIn = null;
		}
		bwp.ClearBuffer ();
	}
}
