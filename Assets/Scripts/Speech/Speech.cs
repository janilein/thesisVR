using NAudio.Wave;
using Procurios.Public;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class Speech : MonoBehaviour {
    private KeywordParser keywordParser;

    private const string URL = "http://api.meaningcloud.com/topics-2.0";
    public string APIKeyMeaningCloud;
    private const string lang = "en";
    private const string tt = "ecn"; //named entities, concepts and quantities
    private const string ud = "ThesisVR";
    private string googleOutputText = "";

    private BufferedWaveProvider bwp;
    public string apiKeyGoogle;

    WaveIn waveIn;
    WaveOut waveOut;
    WaveFileWriter writer;
    WaveFileReader reader;
    string output = "audio.raw";

    public Speech() {
        keywordParser = new KeywordParser();
    }

    // Use this for initialization
    void Start () {
        waveOut = new WaveOut();
        waveIn = new WaveIn();

        waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(WaveIn_DataAvailable);
        waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
        bwp = new BufferedWaveProvider(waveIn.WaveFormat);
        bwp.DiscardOnBufferOverflow = true;

        //btnRecordVoice.Enabled = true;
        //btnSave.Enabled = false;
    }

    void WaveIn_DataAvailable(object sender, WaveInEventArgs e) {
        bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);

    }

    public void BtnRecordVoice_Click() {
        if (NAudio.Wave.WaveIn.DeviceCount < 1) {
            Console.WriteLine("No microphone!");
            return;
        }

        waveIn.StartRecording();

        //btnRecordVoice.Enabled = false;
        //btnSave.Enabled = true;

        //btnSpeechInfo.Enabled = false;

    }

    public void BtnSave_Click() {
        waveIn.StopRecording();

        if (File.Exists("audio.raw"))
            File.Delete("audio.raw");

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
        writer.Close();
        writer = null;

        reader = new WaveFileReader("audio.raw"); // (new MemoryStream(bytes));
        waveOut.Init(reader);
        waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(WaveOut_PlaybackStopped);
        waveOut.Play();
    }

    private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e) {
        waveOut.Stop();
        reader.Close();
        reader = null;
    }

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

            var req = (HttpWebRequest)WebRequest.Create("https://speech.googleapis.com/v1/speech:recognize?key=" + apiKeyGoogle);
            req.ContentType = "application/json";
            req.Method = "POST";
            string text = "";
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }

            try {
                var httpResponse = (HttpWebResponse)req.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    text = text + Environment.NewLine + result;
                    googleOutputText = text;
                    Debug.Log(text);

                    bool successParse = true;
                    Hashtable o = (Hashtable)JSON.JsonDecode(googleOutputText, ref successParse);
                    googleOutputText = GetTranscript(o);

                }
            } catch (System.Net.WebException exc) {
                using (var stream = exc.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    text = text + Environment.NewLine + "Error: " + reader.ReadToEnd();
                    Debug.Log(text);
                    googleOutputText = null;
                }
            } catch (Exception exc) {
                text = text + Environment.NewLine + "Other error: " + exc.ToString();
                Debug.Log(text);
                googleOutputText = null;
            }

        } else {
            Debug.Log("Audio File Missing ");
        }
    }

    public void MakeRequest() {

        //for testing
        googleOutputText = "In my street there are 5 houses.";

        if (googleOutputText != null || googleOutputText.Length != 0) {
            var req = (HttpWebRequest)WebRequest.Create("http://api.meaningcloud.com/topics-2.0");
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string body = "key=" + APIKeyMeaningCloud + "&lang=" + lang + "&txt=" + googleOutputText + "&tt=" + tt + "&ud=" + ud;
            using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }

            string meaningCloudOutput = "";
            try {
                Debug.Log("Starting MeaningCloud Request");
                var httpResponse = (HttpWebResponse)req.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    meaningCloudOutput = streamReader.ReadToEnd();
                    Debug.Log(meaningCloudOutput);
                    bool successParse = true;
                    Hashtable o = (Hashtable)JSON.JsonDecode(meaningCloudOutput, ref successParse);
                    keywordParser.ConvertHashtable(o);
                }
            } catch (System.Net.WebException exc) {
                using (var stream = exc.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    string text = "Error: " + reader.ReadToEnd();
                    Debug.Log(text);
                }
            } catch (Exception exc) {
                string text = "Other error: " + exc.ToString();
                Debug.Log(text);
            }
        } else {
            Debug.Log("Nothing to send to MeaningCloud");
        }

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
            }
        }
        return null;
    }
}
