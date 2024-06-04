using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;

namespace Verdure.VoiceAssistant;
public class VoiceAssistantHelper
{

    private SpeechRecognizer speechRecognizer;

    public static VoiceAssistantHelper _instance;

    private readonly MediaPlayer mediaPlayer = new();
    public static VoiceAssistantHelper Instance => _instance ??= new VoiceAssistantHelper();

    public VoiceAssistantHelper()
    {
        mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
    }

    private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        await InitializeDialogueRecognizerAsync();
    }


    /// <summary>
    /// Initialize Speech Recognizer and compile constraints.
    /// </summary>
    /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
    /// <returns>Awaitable task.</returns>
    public async Task InitializeKeywordRecognizerAsync()
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_KeywordStateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_KeywordResultGenerated;
            speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_KeywordCompleted;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }
        // Create an instance of SpeechRecognizer.
        speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        // Provide feedback to the user about the state of the recognizer.
        speechRecognizer.StateChanged += SpeechRecognizer_KeywordStateChanged;
        //// 添加语音识别事件处理程序
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_KeywordResultGenerated;
        speechRecognizer.ContinuousRecognitionSession.Completed += Recognizer_KeywordCompleted;

        //// Compile the dictation topic constraint, which optimizes for dictated speech.
        //var dictationConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "嗨阿广");
        //speechRecognizer.Constraints.Add(dictationConstraint);
        //SpeechRecognitionCompilationResult compilationResult = await speechRecognizer.CompileConstraintsAsync();

        // You could create this array dynamically.
        string[] responses = { "嗨阿广" };

        // Add a list constraint to the recognizer.
        var listConstraint = new SpeechRecognitionListConstraint(responses, "yesOrNo");

        speechRecognizer.Constraints.Add(listConstraint);

        // Compile the constraint.
        await speechRecognizer.CompileConstraintsAsync();

        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
        //if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
        //{
        //}
    }

    /// <summary>
    /// Initialize Speech Recognizer and compile constraints.
    /// </summary>
    /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
    /// <returns>Awaitable task.</returns>
    public async Task InitializeDialogueRecognizerAsync()
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_DialogueStateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_DialogueResultGenerated;
            speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_DialogueCompleted;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }
        // Create an instance of SpeechRecognizer.
        speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        // Provide feedback to the user about the state of the recognizer.
        speechRecognizer.StateChanged += SpeechRecognizer_DialogueStateChanged;
        //// 添加语音识别事件处理程序
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_DialogueResultGenerated;
        speechRecognizer.ContinuousRecognitionSession.Completed += Recognizer_DialogueCompleted;


        var webSearchGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch", "sound");
        //webSearchGrammar.Probability = SpeechRecognitionConstraintProbability.Min;
        speechRecognizer.Constraints.Add(webSearchGrammar);
        await speechRecognizer.CompileConstraintsAsync();

        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
        //if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
        //{
        //}
    }

    /// <summary>
    /// 文本播放声音
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task MediaPlayerTextToSpeechAsync(string text)
    {
        var stream = await TextToSpeechAsync(text);
        mediaPlayer.SetStreamSource(stream);
        mediaPlayer.Play();
    }

    /// <summary>
    /// 文本转语音流
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private async Task<IRandomAccessStream?> TextToSpeechAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                using SpeechSynthesizer synthesizer = new();
                // Create a stream from the text. This will be played using a media element.
                var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                return synthesisStream;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
                // If media player components are unavailable, (eg, using a N SKU of windows), we won't
            }
            catch (Exception)
            {
                return null;
            }
        }
        return null;
    }

    public async Task StartAsync()
    {
        await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        // Check to make sure that the constraints were in a proper format and the recognizer was able to compile it.
        //if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
        //{
        //}
    }
    

    private void SpeechRecognizer_DialogueStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
    {
        Debug.WriteLine($"语音识别会话状态变更--{args.State.ToString()}");

        if (args.State == SpeechRecognizerState.SpeechDetected)
        {
            Debug.WriteLine($"识别结果：时间{DateTime.Now.ToLongTimeString()}");
        }
    }

    private async void Recognizer_DialogueResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        Debug.WriteLine($"Dialogue语音识别会话结果--{args.Result.Text}");
        await MediaPlayerTextToSpeechAsync("我什么都会做,琴棋书画样样精通。");
        await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
        if (args.Result.Text == "嗨阿广")
        {
            if (speechRecognizer != null)
            {
                // cleanup prior to re-initializing this scenario.
                speechRecognizer.StateChanged -= SpeechRecognizer_DialogueStateChanged;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_DialogueResultGenerated;
                speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_DialogueCompleted;

                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }
            await MediaPlayerTextToSpeechAsync("主人我在");
            Debug.WriteLine($"Dialogue识别结果：时间{DateTime.Now.ToLongTimeString()}");
            
        }
        else
        {
            //await ReInitializeRecognizerAsync();
            //await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }
    }

    private async void Recognizer_DialogueCompleted(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        Debug.WriteLine($"Dialogue语音识别会话已停止--{args.Status.ToString()}");
        await InitializeKeywordRecognizerAsync();

        if (args.Status == SpeechRecognitionResultStatus.Success)
        {
            await InitializeKeywordRecognizerAsync();
        }
    }

    private void SpeechRecognizer_KeywordStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
    {
        Debug.WriteLine($"Keyword语音识别会话状态变更--{args.State.ToString()}");

        if (args.State == SpeechRecognizerState.SpeechDetected)
        {
            Debug.WriteLine($"Keyword识别结果：时间{DateTime.Now.ToLongTimeString()}");
        }
    }

    private async void Recognizer_KeywordResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        Debug.WriteLine($"Keyword语音识别会话结果--{args.Result.Text}");
        await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
        if (args.Result.Text == "嗨阿广")
        {
            if (speechRecognizer != null)
            {
                // cleanup prior to re-initializing this scenario.
                speechRecognizer.StateChanged -= SpeechRecognizer_KeywordStateChanged;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_KeywordResultGenerated;
                speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_KeywordCompleted;

                this.speechRecognizer.Dispose();
                this.speechRecognizer = null;
            }
            await MediaPlayerTextToSpeechAsync("主人我在");
            Debug.WriteLine($"Keyword识别结果：时间{DateTime.Now.ToLongTimeString()}");

        }
        else
        {
            await InitializeKeywordRecognizerAsync();
            //await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }
    }

    private async void Recognizer_KeywordCompleted(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        Debug.WriteLine($"Keyword语音识别会话已停止--{args.Status.ToString()}");

        if (args.Status == SpeechRecognitionResultStatus.Success)
        {
            Debug.WriteLine($"Keyword语音识别会话Completed--{args.Status.ToString()}");
            //await InitializeKeywordRecognizerAsync();
        }
    }
}
