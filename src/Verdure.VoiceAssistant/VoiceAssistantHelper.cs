using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;

namespace Verdure.VoiceAssistant;
public class VoiceAssistantHelper
{

    private SpeechRecognizer speechRecognizer;

    public static VoiceAssistantHelper _instance;
    public static VoiceAssistantHelper Instance => _instance ??= new VoiceAssistantHelper();


    /// <summary>
    /// Initialize Speech Recognizer and compile constraints.
    /// </summary>
    /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
    /// <returns>Awaitable task.</returns>
    public async Task InitializeRecognizerAsync()
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_ResultGenerated;
            speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_Completed;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }
        // Create an instance of SpeechRecognizer.
        speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        // Provide feedback to the user about the state of the recognizer.
        speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
        //// 添加语音识别事件处理程序
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_ResultGenerated;
        speechRecognizer.ContinuousRecognitionSession.Completed += Recognizer_Completed;

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

    public async Task StartAsync()
    {
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
    public async Task ReInitializeRecognizerAsync()
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= Recognizer_ResultGenerated;
            speechRecognizer.ContinuousRecognitionSession.Completed -= Recognizer_Completed;

            this.speechRecognizer.Dispose();
            this.speechRecognizer = null;
        }
        // Create an instance of SpeechRecognizer.
        speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        // Provide feedback to the user about the state of the recognizer.
        speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
        //// 添加语音识别事件处理程序
        speechRecognizer.ContinuousRecognitionSession.ResultGenerated += Recognizer_ResultGenerated;
        speechRecognizer.ContinuousRecognitionSession.Completed += Recognizer_Completed;

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

    private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
    {
        Debug.WriteLine($"语音识别会话状态变更--{args.State.ToString()}");

        if(args.State == SpeechRecognizerState.SpeechDetected)
        {
            Debug.WriteLine($"识别结果：时间{DateTime.Now.ToLongTimeString()}");
        }
    }

    private async void Recognizer_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        Debug.WriteLine($"语音识别会话结果--{args.Result.Text}");
        await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
        if (args.Result.Text == "嗨阿广")
        {     
            Debug.WriteLine($"识别结果：时间{DateTime.Now.ToLongTimeString()}");
        }
        else
        {
            await ReInitializeRecognizerAsync();
            //await speechRecognizer.ContinuousRecognitionSession.StartAsync();
        }
    }

    private async void Recognizer_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        Debug.WriteLine($"语音识别会话已停止--{args.Status.ToString()}");

        if(args.Status == SpeechRecognitionResultStatus.Success)
        {
            await ReInitializeRecognizerAsync();
        }
    }

}
