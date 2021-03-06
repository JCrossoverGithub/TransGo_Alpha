using DesktopClient.Data;
using DesktopClient.Models;
using Google.Cloud.Speech.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DesktopClient
{
    public class InfiniteStreaming
    {
        private const int SampleRate = 16000;
        private const int ChannelCount = 1;
        private const int BytesPerSample = 2;
        private const int BytesPerSecond = SampleRate * ChannelCount * BytesPerSample;
        private static readonly TimeSpan s_streamTimeLimit = TimeSpan.FromSeconds(290);

        private readonly SpeechClient _client;

        /// <summary>
        /// Microphone chunks that haven't yet been processed at all.
        /// </summary>
        private readonly BlockingCollection<ByteString> _microphoneBuffer = new BlockingCollection<ByteString>();

        /// <summary>
        /// Chunks that have been sent to Cloud Speech, but not yet finalized.
        /// </summary>
        private readonly LinkedList<ByteString> _processingBuffer = new LinkedList<ByteString>();

        /// <summary>
        /// The start time of the processing buffer, in relation to the start of the stream.
        /// </summary>
        private TimeSpan _processingBufferStart;

        /// <summary>
        /// The current RPC stream, if any.
        /// </summary>
        private SpeechClient.StreamingRecognizeStream _rpcStream;

        /// <summary>
        /// The deadline for when we should stop the current stream.
        /// </summary>
        private DateTime _rpcStreamDeadline;

        /// <summary>
        /// The task indicating when the next response is ready, or when we've
        /// reached the end of the stream. (The task will complete in either case, with a result
        /// of True if it's moved to another response, or False at the end of the stream.)
        /// </summary>
        private ValueTask<bool> _serverResponseAvailableTask;

        public Boolean isRunning = false;

        public TranscriptPage _main;

        LectureDbContext db = new LectureDbContext();

        private HubConnection _hubConnection;

        Lecture returnedtranscript = new Lecture();
        private async Task Connect()
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:44396/chathub")
            .Build();
            await _hubConnection.StartAsync();
        }

        public InfiniteStreaming(TranscriptPage main)
        {
            _main = main;
            System.Diagnostics.Debug.WriteLine("infinite streaming constructor created");
            _client = SpeechClient.Create();
        }

        /// <summary>
        /// Runs the main loop until "exit" or "quit" is heard.
        /// </summary>
        private async Task RunAsync()
        {
            //System.Diagnostics.Debug.WriteLine("start of runasync");
            using (var microphone = StartListening())
            {
                System.Diagnostics.Debug.WriteLine("start of using(var mic)");
                while (isRunning)
                {
                    System.Diagnostics.Debug.WriteLine("WHILE ISRUNNING = TRUE");
                    await MaybeStartStreamAsync();
                    // ProcessResponses will return false if it hears "exit" or "quit".
                    if (!ProcessResponses() || !isRunning)
                    {
                        System.Diagnostics.Debug.WriteLine("isrunning is false in runasync()");
                        db.Dispose();
                        System.Diagnostics.Debug.WriteLine("!ProcessResponse() OR isRunning IS FALSE!!!!");
                        return;
                    }
                    await TransferMicrophoneChunkAsync();
                }
            }
        }

        /// <summary>
        /// Starts a new RPC streaming call if necessary. This will be if either it's the first call
        /// (so we don't have a current request) or if the current request will time out soon.
        /// In the latter case, after starting the new request, we copy any chunks we'd already sent
        /// in the previous request which hadn't been included in a "final result".
        /// </summary>
        private async Task MaybeStartStreamAsync()
        {
            var now = DateTime.UtcNow;
            if (_rpcStream != null && now >= _rpcStreamDeadline)
            {
                System.Diagnostics.Debug.WriteLine($"Closing stream before it times out");
                await _rpcStream.WriteCompleteAsync();
                _rpcStream.GrpcCall.Dispose();
                _rpcStream = null;
            }

            // If we have a valid stream at this point, we're fine.
            if (_rpcStream != null)
            {
                return;
            }
            // We need to create a new stream, either because we're just starting or because we've just closed the previous one.
            _rpcStream = _client.StreamingRecognize();
            _rpcStreamDeadline = now + s_streamTimeLimit;
            _processingBufferStart = TimeSpan.Zero;
            _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();
            await _rpcStream.WriteAsync(new StreamingRecognizeRequest
            {
                StreamingConfig = new StreamingRecognitionConfig
                {
                    Config = new RecognitionConfig
                    {
                        Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = SampleRate,
                        LanguageCode = "en-US",
                        MaxAlternatives = 1
                    },
                    InterimResults = true,
                }
            });

            System.Diagnostics.Debug.WriteLine($"Writing {_processingBuffer.Count} chunks into the new stream.");
            foreach (var chunk in _processingBuffer)
            {
                await WriteAudioChunk(chunk);
            }
        }

        /// <summary>
        /// Processes responses received so far from the server,
        /// returning whether "exit" or "quit" have been heard.
        /// </summary>
        private bool ProcessResponses()
        {
            while (_serverResponseAvailableTask.IsCompleted && _serverResponseAvailableTask.Result && isRunning)
            {
                var response = _rpcStream.GetResponseStream().Current;
                _serverResponseAvailableTask = _rpcStream.GetResponseStream().MoveNextAsync();
                // Uncomment this to see the details of interim results.
                //System.Diagnostics.Debug.WriteLine($"Response: {response}");

                // See if one of the results is a "final result". If so, we trim our
                // processing buffer.
                var finalResult = response.Results.FirstOrDefault(r => r.IsFinal);
                string message = response.Results.First().Alternatives[0].Transcript;
                //Print number 1 most likely interim result
                //System.Diagnostics.Debug.WriteLine(message);
                int isfinal;


                if(!response.Results.First().IsFinal)
                {
                    isfinal = 0;
                    _hubConnection.SendAsync("SendGroupMessage", returnedtranscript.Id, isfinal, message);
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { _main.transcript.Text = returnedtranscript.Transcript + " " + message; }); ///
                }

                if (finalResult != null)
                {
                    string transcript = finalResult.Alternatives[0].Transcript;
                    //System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,(ThreadStart)delegate { _main.transcript.Text = transcript; }); ///
                    System.Diagnostics.Debug.WriteLine($"Transcript: {transcript}");

                    if(response.Results.First().IsFinal)
                    {
                        // If isFinal is true, append the new sentence to the transcript.
                        System.Diagnostics.Debug.WriteLine("DONE IS FINAL!!!!!");
                        isfinal = 1;
                        _hubConnection.SendAsync("SendGroupMessage", returnedtranscript.Id, isfinal, message);
                        returnedtranscript.Transcript = returnedtranscript.Transcript + " " + transcript;
                        db.Update(returnedtranscript);
                        db.SaveChangesAsync();
                    }

                    if(!isRunning)
                    {
                        System.Diagnostics.Debug.WriteLine("isrunning is false in processresponse()");
                        return false;
                    }

                    if (transcript.ToLowerInvariant().Contains("exit") ||
                        transcript.ToLowerInvariant().Contains("quit"))
                    {
                        return false;
                    }

                    TimeSpan resultEndTime = finalResult.ResultEndTime.ToTimeSpan();

                    // Rather than explicitly iterate over the list, we just always deal with the first
                    // element, either removing it or stopping.
                    int removed = 0;
                    while (_processingBuffer.First != null)
                    {
                        var sampleDuration = TimeSpan.FromSeconds(_processingBuffer.First.Value.Length / (double)BytesPerSecond);
                        var sampleEnd = _processingBufferStart + sampleDuration;

                        // If the first sample in the buffer ends after the result ended, stop.
                        // Note that part of the sample might have been included in the result, but the samples
                        // are short enough that this shouldn't cause problems.
                        if (sampleEnd > resultEndTime)
                        {
                            break;
                        }
                        _processingBufferStart = sampleEnd;
                        _processingBuffer.RemoveFirst();
                        removed++;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Takes a single sample chunk from the microphone buffer, keeps a local copy
        /// (in case we need to send it again in a new request) and sends it to the server.
        /// </summary>
        /// <returns></returns>
        private async Task TransferMicrophoneChunkAsync()
        {
            // This will block - but only for ~100ms, unless something's really broken.
            var chunk = _microphoneBuffer.Take();
            _processingBuffer.AddLast(chunk);
            await WriteAudioChunk(chunk);
        }

        /// <summary>
        /// Writes a single chunk to the RPC stream.
        /// </summary>
        private Task WriteAudioChunk(ByteString chunk) =>
            _rpcStream.WriteAsync(new StreamingRecognizeRequest { AudioContent = chunk });

        /// <summary>
        /// Starts listening to input device 0, and adds an event handler which simply adds
        /// the sample to the microphone buffer. The returned <see cref="WaveInEvent"/> must
        /// be disposed after we've finished with it.
        /// </summary>
        private WaveInEvent StartListening()
        {
            var waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(SampleRate, ChannelCount)
            };
            waveIn.DataAvailable += (sender, args) =>
            _microphoneBuffer.Add(ByteString.CopyFrom(args.Buffer, 0, args.BytesRecorded));
            waveIn.StartRecording();
            return waveIn;
        }

        /// <summary>
        /// Note: the calling code expects a Task with a return value for consistency with
        /// other samples.
        /// We'll always return a task which eventually faults or returns 0.
        /// </summary>
        public static async Task<int> RecognizeAsync()
        {
            //System.Diagnostics.Debug.WriteLine("Recognize async has been called");
            //var instance = new InfiniteStreaming();
            //await instance.RunAsync();
            return 0;
        }

        public async void BeginTranslate(string _transcriptid)
        {
            isRunning = true;
            //System.Diagnostics.Debug.WriteLine("beginTranslate has been called");
            //await RecognizeAsync();
            await Connect();
            returnedtranscript = db.tblLectures.FirstOrDefault(a => a.Id == _transcriptid); // https://stackoverflow.com/questions/3642371/how-to-update-only-one-field-using-entity-framework
            await RunAsync();
            //System.Diagnostics.Debug.WriteLine("db updated!");
        }



        public async void EndProcess()
        {
            isRunning = false;
            System.Diagnostics.Debug.WriteLine("end process called");
            returnedtranscript.IsComplete = 1;
            returnedtranscript.TimeEnd = DateTime.Now.ToShortTimeString();
            db.Update(returnedtranscript);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            System.Diagnostics.Debug.WriteLine("db is disposed");
        }
    }
}

