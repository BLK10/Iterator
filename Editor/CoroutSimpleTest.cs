using System;
using System.Collections;

using NUnit.Framework;
using BLK10.Iterator;
using UnityEditor;
using UnityEngine;


namespace BLK10.Test.Iterator
{
    [TestFixture]
    [Category("Corout Simple Tests")]
    public class CoroutSimpleTest
    {

        [Test] // ok
        public void Test()
        {
            var x = 7.525f;
            var y = SimpleRoutine.GetSQRF(x);

            Corout.Create<float>(co => SimpleCoroutine.GetSQRFAsync(x, co))
                  .OnComplete(co =>
                  {                      
                      Assert.AreEqual(co.Result, y);
                      Assert.IsTrue(co.IsDone);
                      Assert.IsTrue(co.IsCompleted);
                      Assert.IsFalse(co.IsFaulted);
                      Assert.IsFalse(co.IsCanceled); 
                  })
                  .Start(ECoroutType.Asynchrone, true);
        }

        [Test] // ok
        public void TestContinuation()
        {
            var x   = 1425.67f;
            int len = SimpleRoutine.GetLength(SimpleRoutine.GetString(x));

            Corout.Create<string>(co => SimpleCoroutine.GetStringAsync(x, co))
                  .ContinueWith<int>(SimpleCoroutine.GetLengthAsync)
                  .OnComplete(co =>
                  {                      
                      Assert.AreEqual(co.Result, len);
                      Assert.IsTrue(co.IsDone);
                      Assert.IsTrue(co.IsCompleted);
                      Assert.IsFalse(co.IsFaulted);
                      Assert.IsFalse(co.IsCanceled);
                  })
                  .Start(ECoroutType.Asynchrone, true);
        }

        [Test] // ok
        public void TestFence()
        {
            int first  = 10;
            int second = 5;
            int third  = 3;

            Corout.Create(() => SimpleCoroutine.NStepVerboseAsync(first))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("First coroutine complete in {0} steps", first));
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.Create(() => SimpleCoroutine.NStepVerboseAsync(second))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("Second coroutine Complete in {0} steps", second));
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("First and second coroutine ends. Start the third..."));

            Corout.Create(() => SimpleCoroutine.NStepVerboseAsync(third))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("Third coroutine complete in {0} steps", third));
                  })
                  .Start(ECoroutType.Asynchrone, true);
        }

        [Test] // ok
        public void TestIsFaulted()
        {            
            Corout.Create(() => SimpleCoroutine.FailAsync())
                  .OnComplete(co =>
                  {
                      Assert.IsTrue(co.IsDone);
                      Assert.IsTrue(co.IsFaulted);
                      Assert.IsFalse(co.IsCompleted);                      
                      Assert.IsFalse(co.IsCanceled);
                  })
                  .ClearError<NotSupportedException>()                  
                  .Start(ECoroutType.Asynchrone, true);
        }

        [Test] // ok
        public void TestCancelOnErrorPreceededByOnComplete()
        {
            Debug.Log("TestCancelOnErrorPreceededByOnComplete for Corout");

            Corout.Create(() => SimpleCoroutine.FailAsync())
                  .OnComplete(co =>
                  {
                      Assert.IsTrue(co.IsFaulted);
                      Debug.Log("- 'OnComplete' arise because cancellation happens after.");
                  })
                  .OnError<NotSupportedException>((ex, token) =>
                  { 
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorPreceededByOnComplete for CoroutContinuation"));
                        
            Corout.Create(() => SimpleCoroutine.FailAsync())
                  .OnComplete(co =>
                  {
                      Assert.IsTrue(co.IsFaulted);
                      Debug.Log("- 'OnComplete' arise because cancellation happens after.");
                  })
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .ContinueWith(() => SimpleCoroutine.NStepVerboseAsync(10))
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorPreceededByOnComplete for Corout<T>"));
                        
            Corout.Create<int>(co => SimpleCoroutine.FailAsync(co))
                  .OnComplete(co =>
                  {
                      Assert.IsTrue(co.IsFaulted);
                      Debug.Log("- 'OnComplete' arise because cancellation happens after.");
                  })
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorPreceededByOnComplete for CoroutContinuation<T>"));
                        
            Corout.Create<int>(co => SimpleCoroutine.FailAsync(co))
                  .OnComplete(co =>
                  {
                      Assert.IsTrue(co.IsFaulted);
                      Debug.Log("- 'OnComplete' arise because cancellation happens after.");
                  })
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .ContinueWith<int>(SimpleCoroutine.GetSQRIAsync)
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");
                  })
                  .Start(ECoroutType.Asynchrone, true);
        }

        [Test] // ok
        public void TestCancelOnErrorFollowedByOnComplete()
        {
            Debug.Log("TestCancelOnErrorFollowedByOnComplete for Corout");
                                   
            Corout.Create(() => SimpleCoroutine.FailAsync())
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })                  
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine was canceled previously");
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorFollowedByOnComplete for CoroutContinuation"));
                        
            Corout.Create(() => SimpleCoroutine.FailAsync())
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");
                  })
                  .ContinueWith(() => SimpleCoroutine.NStepVerboseAsync(10))
                  .OnComplete(co =>
                  {                      
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");                                       
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorFollowedByOnComplete for Corout<T>"));
          
            Corout.Create<int>(co => SimpleCoroutine.FailAsync(co))
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine was canceled previously");
                  })
                  .Start(ECoroutType.Asynchrone, true);

            Corout.AddFence(() => Debug.Log("TestCancelOnErrorFollowedByOnCompleteGeneric for CoroutContinuation<T>"));
                        
            Corout.Create<int>(co => SimpleCoroutine.FailAsync(co))
                  .OnError<NotSupportedException>((ex, token) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      // cancel corout silently
                      token.Cancel();
                      // cancel corout by throwing
                      // token.Cancel(new CoroutCanceledException());
                  })
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");
                  })
                  .ContinueWith<int>(SimpleCoroutine.GetSQRIAsync)
                  .OnComplete(co =>
                  {
                      Assert.Fail("- 'OnComplete' should not arise because coroutine continuation was canceled previously");
                  })
                  .Start(ECoroutType.Asynchrone, true);

        }
        
        [Test] // ok
        public void TestParallelization()
        {            
            var a = 3;
            //var b = SimpleRoutine.GetSQRI(a);

            var c = 4.57f;
            //var d = SimpleRoutine.GetSQRF(c);

            var e = 7;

            var f = 1425.67f;
            //var g = SimpleRoutine.GetString(e);

            var corout01 = Corout.Create<int>(co => SimpleCoroutine.GetSQRIAsync(a, co));
            var corout02 = Corout.Create<float>(co => SimpleCoroutine.GetSQRFAsync(c, co));
            var corout03 = Corout.Create<int>(co => SimpleCoroutine.GetSQRIFaultedAsync(e, co));
            var corout04 = Corout.Create<string>(co => SimpleCoroutine.GetStringAsync(f, co));

            Corout.WhenAll(corout01, corout02, corout03, corout04)
                  .OnError<NullReferenceException>((ex, token) =>
                  {
                      Debug.Log(string.Format("catch {0} error.", ex.Message));
                      //token.Cancel(new CoroutCanceledException(ex.Message));
                  })                  
                  .OnComplete(co =>
                  {                      
                      Debug.Log(co.Result.Item1.Key + ", is completed: " + co.Result.Item1.Value);
                      Debug.Log(co.Result.Item2.Key + ", is completed: " + co.Result.Item2.Value);
                      Debug.Log(co.Result.Item3.Key + ", is completed: " + co.Result.Item3.Value);
                      Debug.Log(co.Result.Item4.Key + ", is completed: " + co.Result.Item4.Value);
                  })
                  .Start(ECoroutType.Asynchrone, true);            
        }

        [Test]
        public void TestCancellation()
        {
            var x = 7.525f;
            //var y = SimpleRoutine.GetSQRF(x);
            
            Corout.Create<float>((co, ct) => SimpleCoroutine.CancelableAsync(x, 10, co, ct))                               
                  .ContinueWith(() => SimpleCoroutine.NStepVerboseAsync(10))
                  .Start();            
            
            Corout.Create<float>((co, ct) => SimpleCoroutine.CancelableAfterAsync(x, 100, co, ct))                  
                  .Start();
        }
       
    }
}
